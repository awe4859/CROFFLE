using CroffleDataManager.SQLiteDB;
using System;
using System.Windows.Media;

namespace CROFFLE_WPF.Classes.MainAbstract
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

        public string ContentsID
        {
            get { return contentsID; }
        }

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
            Console.WriteLine(result);
            contentsID = result;
        }

        protected internal void FromARGB(byte a, byte r, byte g, byte b)
        {
            color_argb = (a << 24) | (r << 16) | (g << 8) | b;
        }
        protected internal void FromARGB(Color color)
        {
            color_argb = (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
        }
        protected internal void GetARGB(out byte a, out byte r, out byte g, out byte b)
        {
            a = (byte)((color_argb >> 24) & 0xFF);
            r = (byte)((color_argb >> 16) & 0xFF);
            g = (byte)((color_argb >> 8) & 0xFF);
            b = (byte)(color_argb & 0xFF);
        }
        protected internal void GetARGB(out Color color)
        {
            color = Color.FromArgb((byte)((color_argb >> 24) & 0xFF),
                                   (byte)((color_argb >> 16) & 0xFF),
                                   (byte)((color_argb >> 8) & 0xFF),
                                   (byte)(color_argb & 0xFF));
        }
    }
}
