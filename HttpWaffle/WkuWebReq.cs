using Croffle.HttpWaffle.Abstract;
using Croffle.HttpWaffle.Interface;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using Croffle.HttpWaffle.Interface.Implement;

namespace Croffle.HttpWaffle
{
    //[ComImport, Guid("9BCC8A89-D739-4E5B-9CEA-84FCFC1F7F9C")]
    internal class _WebSession : WebSession { }

    /// <summary>
    /// 원광대에 관련된 웹 페이지들의 Request를 제공
    /// </summary>
    internal class WkuWebReq : AWkuWebReq
    {
        internal HttpWebResponse resp;
        IWebSession webSession;

        /// <summary>
        /// Initializing
        /// </summary>
        internal WkuWebReq()
        {
            //interface의 기능을 사용
            webSession = new _WebSession() as IWebSession;
        }

        /// <summary>
        /// Abstract에서 선언한 Method 구현, ID, password를 넣으면 쿠키를 out으로 넘기고, 성공 시 1, 실패 시 0을 반환.
        /// </summary>
        protected internal override int GetWKULoginSession(string id, string passwd, out CookieContainer cookie)
        {
            string url = "https://auth.wku.ac.kr/Cert/User/Login/login.jsp";
            string post_data = webSession.GetLoginPostData(id, passwd);
            string host = "auth.wku.ac.kr";
            string referer = "https://waffle.wku.ac.kr/";
            string content_type = "application/x-www-form-urlencoded; charset=utf-8";

            int result = webSession.GetLoginSession(url, post_data, host, referer, content_type, out resp, out cookie);

            return result;
        }

        /// <summary>
        /// postdata를 문자열 형태로, 쿠키는 객체를 직접 참조 하여 Json을 문자열로 out한다. 실패시 0반환.
        /// Tip: string으로 받은 현재 솔루션 JsonClasses.Json 클래스의 Deserialize(string json)을 통해 JObject 객체로 전환할 수 있다.
        /// </summary>
        protected internal override int GetJsonPOST(string post_data, ref CookieContainer cookie, out string json)
        {
            string url = "https://waffle.wku.ac.kr/lms/FSPServlet";
            string referer = "https://waffle.wku.ac.kr/lms/myclass/index.jsp";
            string host = "waffle.wku.ac.kr";
            string content_type = "application/x-www-form-urlencoded; charset=utf-8";
            int result = webSession.PostMethod(url, post_data, host, referer, content_type, out resp, ref cookie);

            json = string.Empty;

            if (result != 1) return result;

            using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8, true))
            {
                json = sr.ReadToEnd();
            }

            return 1;
        }

        /// <summary>
        /// POST를 이용해 나오는 결과값이 HTML형태 일때 사용 HTMLDocument로 내보낸다.
        /// </summary>
        protected internal override void GetHtmlPOST(string post_data, out HtmlDocument htmlDoc, ref CookieContainer cookie)
        {
            WafflePage wafflePage = new WafflePage();
            string url = "https://waffle.wku.ac.kr/lms/FSPServlet";
            string host = "waffle.wku.ac.kr";
            string referer = wafflePage[EPage.eMyClass];
            string content_type = "application/x-www-form-urlencoded; charset=utf-8";
            webSession.PostMethod(url, post_data, host, referer, content_type, out resp, ref cookie);

            htmlDoc = new HtmlDocument();

            using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8, true))
            {
                string html = sr.ReadToEnd();
                htmlDoc.Load(html);
            }
        }

        /// <summary>
        /// GET을 이용해 HTML 파일을 내보낸다.
        /// </summary>
        protected internal override void GetHtmlGET(EPage ePage, string query, out HtmlDocument htmlDoc, ref CookieContainer cookie)
        {
            WafflePage wafflePage = new WafflePage();
            string url = wafflePage[ePage];
            if (query != null) url += $@"?{query}";
            string host = "waffle.wku.ac.kr";
            string referer = wafflePage[EPage.eMyClass];
            webSession.GETMethod(url, host, referer, out resp, ref cookie);

            htmlDoc = new HtmlDocument();

            using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8 , true))
            {
                string html = sr.ReadToEnd();
                htmlDoc.LoadHtml(html);
            }
        }

        protected internal void GetHtmlGET(EPage ePage, string query, string referer, out HtmlDocument htmlDoc, ref CookieContainer cookie)
        {
            WafflePage wafflePage = new WafflePage();
            string url = wafflePage[ePage];
            if (query != null) url += $@"?{query}";
            string host = "waffle.wku.ac.kr";
            webSession.GETMethod(url, host, referer, out resp, ref cookie);

            htmlDoc = new HtmlDocument();

            using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8, true))
            {
                string html = sr.ReadToEnd();
                htmlDoc.LoadHtml(html);
            }
        }

    }
}
