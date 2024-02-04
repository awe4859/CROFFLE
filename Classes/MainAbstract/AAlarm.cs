using System;
using System.Data;

namespace Croffle.Classes.MainAbstract
{
    internal abstract class AAlarm
    {
        /// <summary>
        /// 알람시간
        /// </summary>
        internal DateTime alarmtime;

        /// <summary>
        /// 알람제목
        /// </summary>
        internal string title;

        /// <summary>
        /// {hours} 시간 이내의 알림을 확인함. 시간 순 정렬
        /// </summary>
        internal abstract int GetAlarm_Hours(int hours, out DataTable result);

        /// <summary>
        /// 현 시간 이후 {start_hours} ~ {end_hours} 내의 알림확인
        /// </summary>
        internal abstract int GetAlarm_Hours(int start_hours, int end_hours, out DataTable result);

        /// <summary>
        /// 미완료 업무에 대한 알림 정보를 가져옴. 없으면 0을 반환
        /// </summary>
        /// <param name="only_waffle">와플과제만 가져옴</param>
        internal abstract int Get_Task_Warning(int minute, bool only_waffle, out DataTable result);
    }
}
