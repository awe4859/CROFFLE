namespace Croffle.Classes
{
    /// <summary>
    ///Memo, Schedule, Task를 확인하기 위한 Enum
    /// </summary>
    public enum EContents
    {
        eMemo,
        eSchedule,
        eTask,
        eSetting,
        eAccount,
        eWaffle
    }

    /// <summary>
    /// 오타를 줄이기 위해 제공
    /// </summary>
    public class Contents
    {
        /// <summary>
        /// 오타를 줄이기 위해 제공
        /// </summary>
        static public string Name(EContents eContents)
        {
            if (eContents == EContents.eMemo) return "memo";
            else if (eContents == EContents.eSchedule) return "schedule";
            else if (eContents == EContents.eTask) return "task";
            else if (eContents == EContents.eSetting) return "setting";
            else if (eContents == EContents.eAccount) return "account";
            else if (eContents == EContents.eWaffle) return "waffle";
            else return null;
        }
    }
}
