using CROFFLE_WPF.Classes.MainAbstract;
using System;
using System.Linq;

namespace CROFFLE_WPF.Classes
{
    internal class Alarm
    {
        EContents type;
        string title;
        DateTime alarmtime;

        internal EContents TYPE { get => type; }
        internal string TITLE { get => title; }
        internal DateTime ALARMTIME { get => alarmtime; }


        /// <summary>
        /// ContentsID 값으로 알람에 필요한 값을 로드한다.
        /// </summary>
        /// <param name="sql">SQLiteDB 객체</param>
        /// <param name="contentsID">컨텐츠 ID</param>
        internal void LoadData(string contentsID)
        {
            Console.WriteLine(contentsID);
            type = Contents.Type(contentsID);
            ADailyProperty? content = null;
            if(type == EContents.eTask) content = new Tasks(contentsID);
            if(type == EContents.eSchedule) content = new Schedules(contentsID);
            if(type == EContents.eWaffle) content = new WaffleTask(contentsID);
            if (content == null) throw new Exception("Invalid ContentsID");
            
            title = content.title;
            alarmtime = content.whens;
        }
    }
}
