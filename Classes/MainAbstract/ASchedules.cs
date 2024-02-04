using System;

namespace Croffle.Classes.MainAbstract
{
    internal abstract class ASchedules : ATasks
    {
        //종료 시각
        internal DateTime endTime;
        //반복, 취소나 연기
        internal bool whether_Repeat = false, canceled = false;
    }
}
