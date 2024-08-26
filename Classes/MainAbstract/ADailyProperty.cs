using CroffleDataManager.SQLiteDB;
using System;

namespace Croffle.Classes.MainAbstract
{
    /// <summary>
    /// Memo, Schedule, Task 가 상속 받을 Class
    /// </summary>
    internal abstract class ADailyProperty
    {
        /// <summary>
        /// 컨텐츠(DB Table) 이름
        /// </summary>
        protected string contents_name;

        /// <summary>
        /// 컨텐츠 구별 ID
        /// </summary>
        protected string contentsID;

        /// <summary>
        /// 컨텐츠가 포함된 날짜
        /// </summary>
        internal DateTime whens;


        /// <summary>
        ///속성 이름(Memo, schedule, Task 이름)
        /// </summary>
        internal string title;

        /// <summary>
        /// ARGB 값을 INTEGER로 저장
        /// </summary>
        internal int color_argb;

        internal abstract void SaveOnDB();
        internal abstract void LoadOnDB(string contentsID);
        internal abstract void DeleteOnDB();

        protected internal SQLiteDB db;

        protected internal void GenerateID()
        {
            int count = db.Get_ID_Count(contents_name, DateTime.Today);
            string result = $@"{contents_name.ToUpper()[0]}{DateTime.Now:yyMMdd}{count:D4}";
            contentsID = result;
        }
    }
}
