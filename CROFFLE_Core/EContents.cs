using System;
using System.Linq;

namespace CROFFLE_WPF.Classes
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

        static public string Name(string contentID)
        {
            if (contentID == null) return null;
            var type = contentID.First();
            if (type == 'M') return "memo";
            else if (type == 'S') return "schedule";
            else if (type == 'T') return "task";
            else if (type == 'W') return "waffle";
            else return null;
        }

        static public EContents Type(string contentID)
        {
            if (contentID == null) return EContents.eMemo;
            var type = contentID.Substring(0, 1);
            if (type == "M") return EContents.eMemo;
            else if (type == "S") return EContents.eSchedule;
            else if (type == "T") return EContents.eTask;
            else if (type == "W") return EContents.eWaffle;
            else return EContents.eMemo;
        }
    }
}
