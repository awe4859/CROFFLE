using Croffle.Data.SQLite;
using System;

namespace Croffle.Classes.MainAbstract
{
    internal abstract class ATasks : ADailyProperty
    {
        /// <summary>
        /// 해당 일정이 진행되는 장소
        /// </summary>
        internal string place = null;
        
        /// <summary>
        /// 일정 시작 시간
        /// </summary>
        internal DateTime scheduleTime;
        
        /// <summary>
        /// 알람 켜짐 여부
        /// </summary>
        internal bool whether_Alarm = false;

        /// <summary>
        /// 스케줄 완료 여부
        /// </summary>
        internal bool done = false;

        internal abstract void SaveOnDB();
        internal abstract void LoadOnDB(string contentsID);
        internal abstract void DeleteOnDB();
    }
}
