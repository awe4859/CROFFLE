using System;
using System.IO;
using System.Net;
using System.Text;

namespace Croffle.HttpWaffle.Interface.Implement
{
    internal class WebSession : IWebSession
    {
        //Login 시 필요한 POSTData를 string으로 return
        string IWebSession.GetLoginPostData(string userid, string password)
        {
            string post_data = $@"userid={userid}&passwd={password}";
            return post_data;
        }

        //Login 후 Session의 Cookie를 out
        int IWebSession.GetLoginSession(string url, string post_data, string host, string referer, string content_type, out HttpWebResponse resp, out CookieContainer _cookies)
        {
            _cookies = new CookieContainer();
            //login도 POST임. Cookie는 새로 생성
            int result = MethodPOST(url, post_data, host, referer, content_type, out resp, ref _cookies);
            _cookies.Add(resp.Cookies);
            return result;
        }

        //cookie를 참조하여 POST 후 response out
        int IWebSession.PostMethod(string url, string post_data, string host, string referer, string content_type, out HttpWebResponse resp, ref CookieContainer _cookies)
        {
            int result = MethodPOST(url, post_data, host, referer, content_type, out resp, ref _cookies);
            return result;
        }

        //실제 POST를 수행하는 Method
        private int MethodPOST(string url, string post_data, string host, string referer, string content_type, out HttpWebResponse resp, ref CookieContainer _cookies)
        {
            //request 설정
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Host = host;
            request.Referer = referer;
            request.ContentType = content_type;
            request.KeepAlive = true;
            request.CookieContainer = _cookies;

            //post data가 null이 아니면 byte로 encoding 후 write
            if (post_data != null)
            {
                byte[] sendData = Encoding.UTF8.GetBytes(post_data);
                using (Stream dataStream = request.GetRequestStream())
                    dataStream.Write(sendData, 0, sendData.Length);
            }

            HttpWebResponse response;

            //response를 받아옴
            try { response = (HttpWebResponse)request.GetResponse(); }
            catch (WebException e) { response = (HttpWebResponse)e.Response; }
            catch (Exception) { response = null; }

            //response를 받은 뒤 상태 확인
            HttpStatusCode status = response.StatusCode;

            resp = response;

            //정상이면 1, 리소스가 없으면 0, 그 외 오류는 -1
            if (status == HttpStatusCode.OK) return 1;
            else if (status == HttpStatusCode.NotFound) return 0;
            else return -1;
        }

        //Cookie를 참조하여 GET 후 response를 out
        int IWebSession.GETMethod(string url, string host, string referer, out HttpWebResponse resp, ref CookieContainer _cookies)
        {
            int result = MethodGET(url, host, referer, out resp, ref _cookies);
            return result;
        }
        //실제 GET을 수행하는 Method
        private int MethodGET(string url, string host, string referer, out HttpWebResponse resp, ref CookieContainer _cookies)
        {
            //request 설정. GET은 POSTData가 없다.(Query는 url의 뒤에 붙는다)
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Host = host;
            request.KeepAlive = true;
            request.Referer = referer;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36";
            request.CookieContainer = _cookies;

            HttpWebResponse response;

            //response 받기
            try { response = (HttpWebResponse)request.GetResponse(); }
            catch (WebException e) { response = (HttpWebResponse)e.Response; }
            catch (Exception) { response = null; }

            HttpStatusCode state = response.StatusCode;
            resp = response;
            
            //상태코드에 따라 결과 return
            if (state == HttpStatusCode.OK) return 1;
            else if (state == HttpStatusCode.NotFound) return 0;
            else return -1;
        }
    }
}
