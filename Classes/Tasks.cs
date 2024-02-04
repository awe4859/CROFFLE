using Croffle.Classes.MainAbstract;
using Croffle.Data.SQLite;
using System.Collections.Generic;
using System;
using System.Data;

namespace Croffle.Classes
{
    /// <summary>
    /// 과업
    /// </summary>
    internal class Tasks : ATasks
    {
        private SQLiteDB sql;

        /// <summary>
        /// Initializing
        /// </summary>
        internal Tasks()
        {
            sql = new SQLiteDB();
            contents_name = Contents.Name(EContents.eTask);
            sql.Initialize(contents_name, Task_Struct());
        }

        /// <summary>
        /// 생성과 함께 DB에서 데이터를 Load
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal Tasks(string contentsID)
        {
            sql = new SQLiteDB();
            contents_name = Contents.Name(EContents.eTask);
            sql.Initialize(contents_name, Task_Struct());
            LoadOnDB(contentsID);
        }


        /// <summary>
        /// 새로운 ID를 생성함. 형식: T{yyMMdd}{00}
        /// </summary>
        protected override void GenerateID()
        {
            string search = $@"
SELECT contentsID
FROM {contents_name}
ORDER BY contentsID DESC";

            sql.SQL_Search(search, out DataSet dataSet);

            string ID;
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
            string result = $@"T{DateTime.Now:yyMMdd}{count + 1:00}";
            contentsID = result;
        }

        /// <summary>
        /// DB에서 데이터를 Load
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal override void LoadOnDB(string contentsID)
        {
            sql.SQL_Get_Table("*", contents_name, $@"contentsID='{contentsID}'", out List<DataTable> tables);
            var values = tables[0].Rows[0];

            this.contentsID = contentsID;
            whens = DateTime.Parse(values["task_date"].ToString());
            scheduleTime = DateTime.Parse(values["dead_line"].ToString());
            title = values["title"].ToString();
            color_argb = Convert.ToInt32(values["color"].ToString());
            place = values["place"].ToString();
            whether_Alarm = Convert.ToBoolean(Convert.ToInt32(values["alarm"]));
            done = Convert.ToBoolean(Convert.ToInt32(values["done"]));
        }

        /// <summary>
        /// DB에 현 객체의 데이터를 저장(덮어쓰기)
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal override void SaveOnDB()
        {
            if (contentsID == "" || contentsID == null) GenerateID();
            var sql_string = sql.Get_SQL_String(contentsID, scheduleTime, title, color_argb, place, whether_Alarm, done);
            sql.SQL_Set_Data(contents_name, contentsID, sql_string);
        }

        /// <summary>
        /// 현재 Object를 DB에서 삭제
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal override void DeleteOnDB()
        {
            sql.SQL_Del_Data(contents_name, contentsID);
        }

        /// <summary>
        /// DB에 task테이블을 생성하기위한 sql을 반환합니다.
        /// </summary>
        /// <returns></returns>
        private string Task_Struct()
        {

           string table_struct = @"
CREATE TABLE task
(contentsID varchar(9) NOT NULL PRIMARY KEY,
 task_date date,
 dead_line datetime,
 added_time datetime DEFAULT (date('now')),
 title varchar(50),
 color integer,
 place varchar(20),
 alarm bool DEFAULT true,
 done bool DEFAULT false,
 CHECK (contentsID LIKE 'T%'))";
            return table_struct;
        }
    }
}
