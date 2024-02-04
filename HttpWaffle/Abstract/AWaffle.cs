using System.Net;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;
using System.Collections.Generic;
using Croffle.Data.SQLite;

namespace Croffle.HttpWaffle.Abstract
{
    internal abstract class AWaffle
    {
        //WAFFLE 동작을 모두 control 할 Class
        static protected CookieContainer _realCookie;
        protected HtmlDocument htmlDoc;
        protected string sNo;

        //Login, get sno
        internal abstract int InitailizeWaffle(string userid, string passwd);

        //강의실 페이지 수업 진행 현황에서 완료/미완료된 수업을 가져옴.
        //DB에 자동 추가
        internal abstract int SetLectureTask();
    }
}
