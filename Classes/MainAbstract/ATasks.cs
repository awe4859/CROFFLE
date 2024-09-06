using System;

namespace CROFFLE_WPF.Classes.MainAbstract
{
    internal abstract class ATasks : ADailyProperty
    {
        /// <summary>
        /// 해당 일정이 진행되는 장소
        /// </summary>
        internal string place = null;
        internal string etc = null;

        /// <summary>
        /// 일정 시작 시간
        /// </summary>
        internal DateTime deadline;
        
        /// <summary>
        /// 알람 켜짐 여부
        /// </summary>
        internal bool bAlarm = false;

        /// <summary>
        /// 스케줄 완료 여부
        /// </summary>
        internal bool bDone = false;
    }
}
