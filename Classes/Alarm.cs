using Croffle.Classes.MainAbstract;
using Croffle.Data.SQLite;
using System;
using System.Data;
using System.Linq;

namespace Croffle.Classes
{
    internal class Alarm : AAlarm
    {

        /// <summary>
        /// {hours} 시간 이내의 알림 시간을 확인함. 시간 순 정렬, 없으면 0을 반환
        /// </summary>
        internal override int GetAlarm_Hours(int hours, out DataTable result)
        {
            GetAlarmTime(0, hours, out DataSet dataSet);
            if (dataSet.Tables != null)
            {
                if (dataSet.Tables[0].Rows.Count > 0)
                    result = dataSet.Tables[0];
                else { result = null; return 0; }
            }
            else { result = null; return 0; }
            return 1;
        }

        /// <summary>
        /// 현 시간 이후 {start_hours} ~ {end_hours} 내의 알림 시간 확인 없으면 0을 반환
        /// </summary>
        internal override int GetAlarm_Hours(int start_hours, int end_hours, out DataTable result)
        {
            GetAlarmTime(start_hours, end_hours, out DataSet dataSet);
            if (dataSet.Tables != null)
            {
                if (dataSet.Tables[0].Rows.Count > 0)
                    result = dataSet.Tables[0];
                else { result = null; return 0; }
            }
            else { result = null; return 0; }
            return 1;
        }

        /// <summary>
        /// 24시간 내 미완료 업무에 대한 알림 시간 정보를 가져옴. 없으면 0을 반환
        /// </summary>
        /// <param name="only_waffle">와플과제만 가져옴</param>
        internal override int Get_Task_Warning(int minute, bool only_waffle, out DataTable result)
        {
            SQLiteDB sql = new SQLiteDB();
            string sql_string = $@"
SELECT DISTINCT time FROM
(SELECT dead_line AS time FROM {Contents.Name(EContents.eWaffle)}
 WHERE dead_line > datetime('now', 'localtime') AND deadline < date('now', 'localtime', '+1 days') AND alarm=true AND done = false";
            if (only_waffle) sql_string += $@")
ORDER BY time ASC";
            else sql_string += $@"
 UNION
 SELECT dead_line AS time FROM {Contents.Name(EContents.eTask)}
 WHERE dead_line > datetime('now', 'localtime') AND deadline < date('now', 'localtime', '+1 days') AND alarm=true AND done = false)
ORDER BY time ASC";

            sql.SQL_Search(sql_string, out DataSet dataSet);

            if (dataSet.Tables.Count > 0)
                result = dataSet.Tables[0];
            else { result = null; return 0; }

            return 1;
        }

        /// <summary>
        /// 해당 시간에 알림이 활성화된 컨텐츠 ID 목록을 가져옵니다.
        /// </summary>
        internal int GetAlarms_Info(bool only_waffle, DateTime time, out DataTable result)
        {
            SQLiteDB sql = new SQLiteDB();
            string sql_string = $@"
SELECT contentsID FROM
(SELECT contentsID FROM {Contents.Name(EContents.eWaffle)}
WHERE dead_line=datetime('{time:yyyy-MM-dd HH:mm}') AND alarm=true AND done=false";
            if (only_waffle) sql_string += $@")";
            else sql_string += $@"
UNION
SELECT contentsID FROM {Contents.Name(EContents.eTask)}
WHERE dead_line=datetime('{time:yyyy-MM-dd HH:mm}') AND alarm=true AND done=false)";

            sql.SQL_Search(sql_string, out DataSet dataSet);

            if (dataSet.Tables[0].Rows.Count > 0)
                result = dataSet.Tables[0];
            else { result = null; return 0; }
            return 1;
        }

        /// <summary>
        /// ContentsID 값으로 알람에 필요한 값을 로드한다.
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        /// <param name="contentsID">컨텐츠 ID</param>
        internal void LoadData(string contentsID)
        {
            var type = contentsID.First();
            if(type == 'T')
            {
                Tasks task = new Tasks(contentsID);
                title = task.title;
                alarmtime = task.whens;
            }
            if(type == 'S')
            {
                Schedules schedule = new Schedules(contentsID);
                title = schedule.title;
                alarmtime = schedule.whens;
            }
            if(type == 'W')
            {
                WaffleTask waffle = new WaffleTask(contentsID);
                title = waffle.title;
                alarmtime = waffle.whens;
            }
        }

        /// <summary>
        /// start와 end 시간 사이의 일정을 시간 오름차순으로 정렬하여 내보냄
        /// </summary>
        /// <param name="start">시작시간</param>
        /// <param name="end">끝 시간</param>
        /// <param name="dataSet">내보내는 DataSet</param>
        private void GetAlarmTime(int start, int end, out DataSet dataSet)
        {
            DateTime time = DateTime.Now;
            SQLiteDB sql = new SQLiteDB();
            string sql_string = $@"
SELECT DISTINCT time FROM
(SELECT dead_line AS time FROM {Contents.Name(EContents.eTask)}
 WHERE dead_line > datetime('{time.AddHours(start):yyyy-MM-dd HH:mm}') AND dead_line < datetime('{time.AddHours(end):yyyy-MM-dd HH:mm}') AND alarm=true AND done=false
 UNION
 SELECT start_time AS time FROM {Contents.Name(EContents.eSchedule)}
 WHERE start_time > datetime('{time.AddHours(start):yyyy-MM-dd HH:mm}') AND start_time < datetime('{time.AddHours(end):yyyy-MM-dd HH:mm}') AND alarm=true AND done=false AND canceled=false
 UNION
 SELECT dead_line AS time FROM {Contents.Name(EContents.eWaffle)}
 WHERE dead_line > datetime('{time.AddHours(start):yyyy-MM-dd HH:mm}') AND dead_line < datetime('{time.AddHours(end):yyyy-MM-dd HH:mm}') AND alarm=true AND done=false)
ORDER BY time ASC";
            sql.SQL_Search(sql_string, out dataSet);
        }
    }
}
