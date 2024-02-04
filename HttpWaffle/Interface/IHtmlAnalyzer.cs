using HtmlAgilityPack;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Croffle.HttpWaffle.Interface
{
    internal interface IHtmlAnalyzer
    {
        /// <summary>
        /// node를 선택하여 node의 attribute를 Get
        /// </summary>
        string GetAttrByNode(string node, string target, ref HtmlDocument htmlDoc);

        /// <summary>
        /// node를 선택하여 info 스크랩
        /// </summary>
        string GetInfoByNode(string node, ref HtmlDocument htmlDoc);

        /// <summary>
        /// 참조한 html에서 node를 선택하여 여러 Info 스크랩, list로 out
        /// </summary>
        void GetManyInfo(string node, out List<string> result, ref HtmlDocument htmlDoc);

        /// <summary>
        /// 참조한 html에서 학번을 Get
        /// </summary>
        string Get_frm_onHtml(string id, ref HtmlDocument htmlDoc);
    }
}
