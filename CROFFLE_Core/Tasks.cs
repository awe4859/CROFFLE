using CROFFLE_WPF.Classes.MainAbstract;
using System;
using System.Data;
using CroffleDataManager.SQLiteDB;

namespace CROFFLE_WPF.Classes
{
    /// <summary>
    /// 과업
    /// </summary>
    internal class Tasks : ATasks
    {
        /// <summary>
        /// Initializing
        /// </summary>
        internal Tasks()
        {
            db = new SQLiteDB();
            contents_name = Contents.Name(EContents.eTask);
        } // Tasks

        /// <summary>
        /// 생성과 함께 DB에서 데이터를 Load
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal Tasks(string contentsID)
        {
            db = new SQLiteDB();
            contents_name = Contents.Name(EContents.eTask);

            LoadOnDB(contentsID);
        } // Tasks

        /// <summary>
        /// DB에서 데이터를 Load
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal override void LoadOnDB(string contentsID)
        {
            Console.WriteLine($@"[Task] Load: loading data from DB");
            db.LoadOnDB(contents_name, contentsID, out DataTable table);
            if( table.Rows.Count == 0)
            {
                Console.WriteLine($@"> [Load] no data found for ID: {contentsID}");
                throw new ArgumentNullException("No data found for ID");
            }
            if( table.Rows.Count > 1)
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
            if(!parse) fail_list.Add($@"task_date: {values["task_date"].ToString()}");
            // deadline
            parse = DateTime.TryParse(values["dead_line"].ToString(), out deadline);
            if(!parse) fail_list.Add($@"dead_line: {values["dead_line"].ToString()}");
            // title
            title = values["title"].ToString() ?? "";
            // color
            parse = int.TryParse(values["color"].ToString(), out color_argb);
            if(!parse) fail_list.Add($@"color: {values["color"].ToString()}");
            // place
            place = values["place"].ToString() ?? "";
            //etc   
            etc = values["etc"].ToString() ?? "";
            // alarm
            parse = bool.TryParse(values["alarm"].ToString(), out bAlarm);
            if(!parse) fail_list.Add($@"alarm: {values["alarm"].ToString()}");
            // done
            parse = bool.TryParse(values["done"].ToString(), out bDone);
            if(!parse) fail_list.Add($@"done: {values["done"].ToString()}");

            if(fail_list.Count > 0)
            {
                Console.WriteLine($@"[Task] Load: failed to parse data");
                foreach(var fail in fail_list)
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
            Console.WriteLine($@"[Task] Save: saving data to DB");
            if (contentsID == "" || contentsID == null) GenerateID();

            string values = $@"'{contentsID}', date('{whens:yyyy-MM-dd}'), datetime('{deadline:yyyy-MM-dd HH:mm}'), "
                +$@"datetime('{DateTime.Now:yyyy-MM-dd HH:mm}'), '{title}', {color_argb}, '{place}','{etc}', {bAlarm}, {bDone}";

            db.SaveOnDB(contents_name, values);
        } // SaveOnDB

        /// <summary>
        /// 현재 Object를 DB에서 삭제
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal override void DeleteOnDB()
        {
            db.DeleteOnDB(contents_name, contentsID);
        } // DeleteOnDB
    } // Tasks
} // namespace Croffle.Classes
