using Croffle.Classes.MainAbstract;
using System;
using System.Data;
using System.Linq;
using CroffleDataManager.SQLiteDB;

namespace Croffle.Classes
{
    internal class Alarm
    {
        string title;
        DateTime alarmtime;

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
    }
}
