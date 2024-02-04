using Croffle.Classes.MainAbstract;
using Croffle.Data.JsonClasses;
using Croffle.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Data;

namespace Croffle.Classes
{
    /// <summary>
    ///메모 클래스
    /// </summary>
    internal class Memos : AMemos
    {
        private readonly Json json;
        SQLiteDB sql;

        /// <summary>
        /// Initializing - DB의 memotable과 Json 클래스 초기화
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal Memos() {
            sql = new SQLiteDB();
            contents_name = Contents.Name(EContents.eMemo);
            sql.Initialize(contents_name, Memo_Struct());
            json = new Json("memos.json");
        }

        /// <summary>
        /// 생성과 함께 데이터를 Load
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal Memos(string contentsID)
        {
            sql = new SQLiteDB();
            contents_name = Contents.Name(EContents.eMemo);
            sql.Initialize(contents_name, Memo_Struct());
            json = new Json("memos.json");

            Load(contentsID);
        }

        /// <summary>
        /// ID를 새로 생성
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        protected override void GenerateID()
        {
            string search = $@"
SELECT contentsID
FROM {contents_name}
ORDER BY contentsID DESC";

            sql.SQL_Search(search, out DataSet dataSet); string ID;
            int count;
            if (dataSet != null)
            {
                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        ID = dataSet.Tables[0].Rows[0][0].ToString();
                        count = Convert.ToInt32(ID.Substring(7));
                    }
                    else count = -1;
                }
                else count = -1;
            }
            else count = -1;

            string result = $@"M{DateTime.Now:yyMMdd}{count + 1:00}";
            contentsID = result;
        }

        /// <summary>
        /// 데이터를 로드
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal override void Load(string contentsID)
        {
            sql = new SQLiteDB();
            json.LoadJObject();

            sql.SQL_Get_Table("*", contents_name, $@"contentsID='{contentsID}'", out List<DataTable> tables);
            var values = tables[0].Rows[0];
            this.contentsID = contentsID;
            whens = DateTime.Parse(values["memo_day"].ToString());
            title = values["title"].ToString();
            color_argb = Convert.ToInt32(values["color"].ToString());

            json.FindItem(contentsID, out detailText);
        }

        /// <summary>
        /// 데이터를 저장
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal override void SaveData()
        {
            if (contentsID == "" || contentsID == null) GenerateID();
            string select = sql.Get_SQL_String(contentsID, whens, title, color_argb);
            sql.SQL_Set_Data(contents_name, contentsID, select);
            Json json = new Json("memos.json");
            json.AddItem(contentsID, detailText);
        }

        /// </summary>
        ///Memo 내용 Modify후 save
        /// <summary>
        internal override void ModifyDetailText(string newDetailText)
        {
            detailText = newDetailText;
            json.AddItem(contentsID, detailText);
        }

        internal override void DeleteMemo()
        {
            sql.SQL_Del_Data(Contents.Name(EContents.eMemo), contentsID);
            Json json = new Json("memos.json");
            json.RemoveItem(contentsID);
        }

        internal string Memo_Struct()
        {
            string table_struct = @"
CREATE TABLE memo
(contentsID varchar(9) NOT NULL PRIMARY KEY,
memo_day date,
added_time datetime DEFAULT (date('now')),
title varchar(50),
color integer,
CHECK (contentsID LIKE 'M%'))";
            return table_struct;
        }
    }
}