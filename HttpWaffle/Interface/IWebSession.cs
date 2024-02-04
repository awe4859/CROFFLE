using System.Net;
using System.Runtime.InteropServices;

namespace Croffle.HttpWaffle.Interface
{
    //[Guid("9BCC8A89-D739-4E5B-9CEA-84FCFC1F7F9C"),
       // InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IWebSession
    {
        //[PreserveSig]
        /// <summary>
        /// waffle id와 password를 받아 PostData 문자열로 return
        /// </summary>
        string GetLoginPostData(string userid, string password);

        //[PreserveSig]
        /// <summary>
        /// waffle에 Login페이지에 Post요청 후 response가 정상인지 int으로 return.
        /// </summary>
        int GetLoginSession(string url,string post_data, string host,string referer, string content_type, out HttpWebResponse resp, out CookieContainer _cookies);

        //[PreserveSig]
        /// <summary>
        /// HttpWebRequest의 Method를 "POST"로하고 response를 받아옴
        /// </summary>
        int PostMethod(string url, string post_data, string host, string referer, string content_type, out HttpWebResponse resp, ref CookieContainer _cookies);

        //[PreserveSig]
        /// <summary>
        /// HttpWebRequest의 Method를 "GET"으로하고 response를 받아옴
        /// </summary>
        int GETMethod(string url, string host, string referer, out HttpWebResponse resp, ref CookieContainer _cookies);
    }
}
