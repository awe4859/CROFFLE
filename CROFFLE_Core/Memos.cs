using CROFFLE_WPF.Classes.MainAbstract;
using CroffleDataManager.SQLiteDB;
using System;
using System.Collections.Generic;
using System.Data;

namespace CROFFLE_WPF.Classes
{
    /// <summary>
    ///메모 클래스
    /// </summary>
    internal class Memos : AMemos
    {
        /// <summary>
        /// Initializing - DB의 memotable과 Json 클래스 초기화
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal Memos() {
            db = new SQLiteDB();
            contents_name = Contents.Name(EContents.eMemo);
        } // Memos

        /// <summary>
        /// 생성과 함께 데이터를 Load
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal Memos(string contentsID)
        {
            db = new SQLiteDB();
            contents_name = Contents.Name(EContents.eMemo);

            LoadOnDB(contentsID);
        } // Memos

        /// <summary>
        /// 데이터를 로드
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal override void LoadOnDB(string contentsID)
        {
            Console.WriteLine($@"[Memo] Load: loading data from DB");

            db.LoadOnDB(contents_name, contentsID, out DataTable table);
            // 데이터가 없을 경우
            if(table.Rows.Count == 0)
            {
                Console.WriteLine($@"> [Load] no data found for ID: {contentsID}");
                throw new ArgumentNullException("No data found for ID");
            }
            // 데이터가 여러개일 경우
            if(table.Rows.Count > 1)
            {
                Console.WriteLine($@"> [Load] too many data found for ID: {contentsID}");
                throw new ArgumentOutOfRangeException("Too many data found for ID");
            }

            var values = table.Rows[0];
            var fail_list = new List<string>();

            // ContentsID
            this.contentsID = contentsID;
            // whens
            var parse = DateTime.TryParse(values["memo_date"].ToString(), out whens);
            if (!parse) fail_list.Add($@"whens: {values["memo_date"].ToString()}");
            // title
            title = values["title"].ToString() ?? "";
            // color
            parse = int.TryParse(values["color"].ToString(), out color_argb);
            if (!parse) fail_list.Add($@"color: {values["color"].ToString()}");
            // detailText
            detailText = values["contents"].ToString() ?? "";

            // failed to parse
            if (fail_list.Count > 0)
            {
                Console.WriteLine($@"> [Load] failed to parse: ");
                foreach (var fail in fail_list)
                {
                    Console.WriteLine($@"  - {fail}");
                }
                throw new FormatException("Failed to parse");
            }
        } // LoadOnDB

        /// <summary>
        /// 데이터를 저장
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal override void SaveOnDB()
        {
            Console.WriteLine($@"[Memo] SaveData: saving data to DB");
            
            if (contentsID == "" || contentsID == null) GenerateID();
            string values = $@"'{contentsID}', date('{whens:yyyy-MM-dd}'),"
                + $@" datetime('{DateTime.Now:yyyy-MM-dd HH:mm:ss}'), '{title}', {color_argb}, "
                + $@"'{detailText}'";
            db.SaveOnDB(contents_name, values);
        } // SaveOnDB

        /// </summary>
        ///Memo 내용 Modify후 save
        /// <summary>
        internal override void ModifyDetailText(string newDetailText)
        {
            detailText = newDetailText;
            SaveOnDB();
        } // ModifyDetailText

        internal override void DeleteOnDB()
        {
            db.DeleteOnDB(contents_name, contentsID);
        } // DeleteOnDB
    } // Memos
} // namespace Croffle.Classes