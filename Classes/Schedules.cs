using Croffle.Classes.MainAbstract;
using Croffle.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Data;

namespace Croffle.Classes
{
    internal class Schedules : ASchedules
    {
        SQLiteDB sql;

        /// <summary>
        /// Initializing
        /// </summary>
        internal Schedules()
        {
            sql = new SQLiteDB();
            contents_name = Contents.Name(EContents.eSchedule);
            sql.Initialize(contents_name, Schedule_Struct());
        }

        /// <summary>
        /// 생성과 함께 DB에서 데이터를 Load
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal Schedules(string contentsID)
        {
            sql = new SQLiteDB();
            contents_name = Contents.Name(EContents.eSchedule);
            LoadOnDB(contentsID);
            sql.Initialize(contents_name, Schedule_Struct());
        }

        /// <summary>
        /// 새로운 ID를 생성함. 형식: S{yyMMdd}{00}
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
            string result = $@"S{DateTime.Now:yyMMdd}{count + 1:00}";
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
            whens = DateTime.Parse(values["sche_date"].ToString());
            scheduleTime = DateTime.Parse(values["start_time"].ToString());
            endTime = DateTime.Parse(values["end_time"].ToString());
            title = values["title"].ToString();
            color_argb = Convert.ToInt32(values["color"].ToString());
            place = values["place"].ToString();
            whether_Alarm = Convert.ToBoolean(Convert.ToInt32(values["alarm"]));
            done = Convert.ToBoolean(Convert.ToInt32(values["done"]));
            canceled = Convert.ToBoolean(Convert.ToInt32(values["canceled"]));
        }

        /// <summary>
        /// DB에 현 객체의 데이터를 저장(덮어쓰기)
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal override void SaveOnDB()
        {
            if(contentsID == "" || contentsID == null) GenerateID();
            var sql_string = sql.Get_SQL_String(contentsID, scheduleTime, endTime, title, color_argb, place, whether_Alarm, done, canceled);
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
        /// DB에서 schedule 테이블을 생성하기 위한 sql 문장을 반환합니다.
        /// </summary>
        /// <returns></returns>
        private string Schedule_Struct()
        {
            string table_struct = @"
CREATE TABLE schedule
(contentsID varchar(9) NOT NULL PRIMARY KEY,
 sche_date date,
 start_time datetime,
 end_time datetime,
 added_time datetime DEFAULT (date('now')),
 title varchar(50),
 color integer,
 place varchar(20),
 alarm bool DEFAULT false,
 done bool DEFAULT false,
 canceled bool DEFAULT false,
 CHECK (contentsID LIKE 'S%'),
 CHECK (end_time >= start_time) )";
            return table_struct;
        }
    }
}
/*
CREATE TABLE schedule
(contentsID varchar(9) NOT NULL PRIMARY KEY,
 sche_date date,
 start_time datetime,
 end_time datetime,
 added_time datetime DEFAULT (date('now')),
 title varchar(50),
 color integer,
 place varchar(20),
 alarm bool DEFAULT false,
 done bool DEFAULT false,
 canceled bool DEFAULT false,
 CHECK (contentsID LIKE 'S%'),
 CHECK (end_time >= start_time) );
 */
