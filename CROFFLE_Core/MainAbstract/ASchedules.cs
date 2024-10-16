using System;

namespace CROFFLE_WPF.Classes.MainAbstract
{
    internal abstract class ASchedules : ADailyProperty
    {
        /// <summary>
     /// 해당 일정이 진행되는 장소
     /// </summary>
        internal string place = null;
        internal string transp = null;
        internal string etc = null;

        /// <summary>
        /// 일정 시작 시각
        /// </summary>
        internal DateTime startTime;

        /// <summary>
        /// 일정 종료 시각
        /// </summary>
        internal DateTime endTime;

        /// <summary>
        /// 알람 켜짐 여부
        /// </summary>
        internal bool bAlarm = false;

        /// <summary>
        /// 스케줄 완료 여부
        /// </summary>
        internal bool bDone = false;

        /// <summary>
        /// 반복, 취소나 연기
        /// </summary>
        internal bool bRepeat = false, bCanceled = false;
        internal bool bAllDay = false;
    }
}
