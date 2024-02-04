using System.Net;
using HtmlAgilityPack;

namespace Croffle.HttpWaffle.Abstract
{
    internal abstract class AWkuWebReq
    {
        //원광대 Login 후, Session을 받아옴
        abstract protected internal int GetWKULoginSession(string id, string passwd, out CookieContainer cookie);

        //Main 페이지에 POST후 JSON을 받아옴
        abstract protected internal int GetJsonPOST(string sNo, ref CookieContainer cookie, out string json);

        //POST 후 HTML을 OUT
        abstract protected internal void GetHtmlPOST(string post_data, out HtmlDocument htmlDoc, ref CookieContainer cookie);

        //GET 후 HTML을 OUT
        abstract protected internal void GetHtmlGET(EPage ePage, string query, out HtmlDocument htmlDoc, ref CookieContainer cookie);
    }
}
