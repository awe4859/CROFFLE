using CROFFLE_WPF.Classes.MainAbstract;
using CroffleDataManager.SQLiteDB;

namespace CROFFLE_WPF.Classes
{
    internal class WaffleTask : AWaffleTask
    {
        internal WaffleTask()
        {
            db = new SQLiteDB();
            contents_name = Contents.Name(EContents.eWaffle);
        }
        /// <summary>
        /// DB에서 Waffle Class에서 자동으로 생성한 컨텐츠를 Load합니다.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="contentsID"></param>
        internal WaffleTask(string contentsID)
        {
            db = new SQLiteDB();
            contents_name = Contents.Name(EContents.eWaffle);

            LoadOnDB(contentsID);
        }
        internal override void LoadOnDB(string contentsID)
        {
            Console.WriteLine($@"[WaffleTask] Load: loading data from DB");
            Console.WriteLine($@"content_type: {contents_name}, id: {contentsID}");
            db.LoadOnDB(contents_name, contentsID, out var table);
            if (table.Rows.Count == 0)
            {
                Console.WriteLine($@"> [Load] no data found for ID: {contentsID}");
                throw new ArgumentNullException("No data found for ID");
            }
            if (table.Rows.Count > 1)
            {
                Console.WriteLine($@"> [Load] too many data found for ID: {contentsID}");
                throw new ArgumentOutOfRangeException("Too many data found for ID");
            }

            var values = table.Rows[0];
            var fail_list = new List<string>();

            // contentsID
            this.contentsID = contentsID;
            // whens
            var parse = DateTime.TryParse(values["task_date"].ToString(), out whens);
            if (!parse) fail_list.Add($@"task_date: {values["task_date"].ToString()}");
            // deadline
            parse = DateTime.TryParse(values["dead_line"].ToString(), out deadline);
            if (!parse) fail_list.Add($@"dead_line: {values["dead_line"].ToString()}");
            // title
            title = values["title"].ToString() ?? "";
            // color
            parse = int.TryParse(values["color"].ToString(), out color_argb);
            if (!parse) fail_list.Add($@"color: {values["color"].ToString()}");
            // place
            place = values["place"].ToString() ?? "";
            //etc
            etc = values["etc"].ToString() ?? "";
            //type
            type = values["type"].ToString() ?? "";
            // alarm
            parse = bool.TryParse(values["alarm"].ToString() ?? "true", out bAlarm);
            if (!parse) fail_list.Add($@"alarm: {values["alarm"].ToString()}");
            // done
            parse = bool.TryParse(values["done"].ToString() ?? "false", out bDone);
            if (!parse) fail_list.Add($@"done: {values["done"].ToString()}");

            if (fail_list.Count > 0)
            {
                Console.WriteLine($@"[WaffleTask] Load: failed to parse data");
                foreach (var fail in fail_list)
                {
                    Console.WriteLine($@"  - {fail}");
                }
                throw new FormatException("Failed to parse data");
            }
        } // LoadOnDB

        /// <summary>
        /// DB에 현 객체의 데이터를 저장(덮어쓰기)
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal override void SaveOnDB()
        {
            throw new InvalidOperationException("Waffle은 수정할 수 없습니다.");
        } // SaveOnDB

        /// <summary>
        /// 현재 Object를 DB에서 삭제
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal override void DeleteOnDB()
        {
            throw new InvalidOperationException("Waffle은 삭제할 수 없습니다.");
        } // DeleteOnDB
    }
}
