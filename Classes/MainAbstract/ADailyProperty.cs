using Croffle.Data.SQLite;
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

        protected abstract void GenerateID();
    }
}
