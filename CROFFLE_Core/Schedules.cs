using CROFFLE_WPF.Classes.MainAbstract;
using CroffleDataManager.SQLiteDB;
using System;
using System.Collections.Generic;
using System.Data;

namespace CROFFLE_WPF.Classes
{
    internal class Schedules : ASchedules
    {
        /// <summary>
        /// Initializing
        /// </summary>
        internal Schedules()
        {
            db = new SQLiteDB();
            contents_name = Contents.Name(EContents.eSchedule);
        } // Schedules

        /// <summary>
        /// 생성과 함께 DB에서 데이터를 Load
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal Schedules(string contentsID)
        {
            db = new SQLiteDB();
            contents_name = Contents.Name(EContents.eSchedule);

            LoadOnDB(contentsID);
        } // Schedules

        /// <summary>
        /// DB에서 데이터를 Load
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal override void LoadOnDB(string contentsID)
        {
            db.LoadOnDB(contents_name, contentsID, out DataTable table);
            if( table.Rows.Count == 0)
            {
                throw new ArgumentNullException("No data found for ID");
            }
            if( table.Rows.Count > 1)
            {
                throw new ArgumentOutOfRangeException("Too many data found for ID");
            }

            var values = table.Rows[0];
            var fail_list = new List<string>();

            // contentsID
            this.contentsID = contentsID;
            // whens
            var parse = DateTime.TryParse(values["sche_date"].ToString(), out whens);
            if(!parse) fail_list.Add($@"sche_date: {values["sche_date"].ToString()}");
            // startTime
            parse = DateTime.TryParse(values["start_time"].ToString(), out startTime);
            if(!parse) fail_list.Add($@"start_time: {values["start_time"].ToString()}");
            // endTime
            parse = DateTime.TryParse(values["end_time"].ToString(), out endTime);
            if(!parse) fail_list.Add($@"end_time: {values["end_time"].ToString()}");
            // title
            title = values["title"].ToString() ?? "";
            // color
            parse = int.TryParse(values["color"].ToString(), out color_argb);
            if(!parse) fail_list.Add($@"color: {values["color"].ToString()}");
            // place
            place = values["place"].ToString() ?? "";
            transp = values["transp"].ToString() ?? "";
            etc = values["etc"].ToString() ?? "";
            // alarm
            parse = bool.TryParse(values["alarm"].ToString(), out bAlarm);
            if(!parse) fail_list.Add($@"alarm: {values["alarm"].ToString()}");
            // done
            parse = bool.TryParse(values["done"].ToString(), out bDone);
            if(!parse) fail_list.Add($@"done: {values["done"].ToString()}");
            // repeat
            parse = bool.TryParse(values["repeat"].ToString(), out bRepeat);
            if(!parse) fail_list.Add($@"repeat: {values["repeat"].ToString()}");
            // all_day
            parse = bool.TryParse(values["all_day"].ToString(), out bAllDay);
            if(!parse) fail_list.Add($@"all_day: {values["all_day"].ToString()}");
            // canceled
            parse = bool.TryParse(values["canceled"].ToString(), out bCanceled);
            if(!parse) fail_list.Add($@"canceled: {values["canceled"].ToString()}");

            // failed to parse
            if (fail_list.Count > 0)
            {
                Console.WriteLine($@"[Task] Load: failed to parse data");
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
            if(contentsID == "" || contentsID == null) GenerateID();
            var value = $@"'{contentsID}', date('{whens:yyyy-MM-dd}'), datetime('{startTime:yyyy-MM-dd HH:mm}'), "
                + $@"datetime('{endTime:yyyy-MM-dd HH:mm}'), datetime('{DateTime.Now:yyyy-MM-dd HH:mm}'), "
                + $@"'{title}', {color_argb}, '{place}', '{transp}', '{etc}', {bAlarm}, {bDone}, {bRepeat}, {bAllDay}, {bCanceled}";

            db.SaveOnDB(contents_name, value);
        } // SaveOnDB

        /// <summary>
        /// 현재 Object를 DB에서 삭제
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        internal override void DeleteOnDB()
        {
            db.DeleteOnDB(contents_name, contentsID);
        } // DeleteOnDB
    } // Schedules
} // namespace Croffle.Classes
