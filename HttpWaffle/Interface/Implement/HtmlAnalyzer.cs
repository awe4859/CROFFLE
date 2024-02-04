using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace Croffle.HttpWaffle.Interface.Implement
{
    internal class HtmlAnalyzer : IHtmlAnalyzer
    {
        //node를 지정하고 node의 Attribute값을 가져옴
        string IHtmlAnalyzer.GetAttrByNode(string node, string target, ref HtmlDocument htmlDoc)
        {
            string result = GetAttrOnNode(node, target, ref htmlDoc);
            return result;
        }

        //Node 내의 값을 return
        string IHtmlAnalyzer.GetInfoByNode(string node, ref HtmlDocument htmlDoc)
        {
            HtmlNode htmlNode = htmlDoc.DocumentNode.SelectSingleNode(node);
            string result = htmlNode.InnerText;
            return result;
        }

        //한번에 많은 값을 list로 return
        void IHtmlAnalyzer.GetManyInfo(string node, out List<string> result, ref HtmlDocument htmlDoc)
        {
            HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes(node);
            result = new List<string>();
            if (htmlNodes != null)
            {
                for (int i = 0; i < htmlNodes.Count; i++)
                {
                    result.Add(htmlNodes[i].InnerHtml);
                }
            }
        }

        //SNO(학번)을 return (WAFFLE 전용)
        string IHtmlAnalyzer.Get_frm_onHtml(string id, ref HtmlDocument htmlDoc)
        {
            string node = $@"//*[@id=""{id}""]";
            string target = "value";
            string result = GetAttrOnNode(node, target, ref htmlDoc);
            
            return result;
        }

        //attrnode와 get sno의 동일기능을 private method로 구현
        private string GetAttrOnNode(string node, string target, ref HtmlDocument htmlDoc)
        {
            HtmlNode htmlNode = htmlDoc.DocumentNode.SelectSingleNode(node);
            string result = htmlNode.GetAttributeValue(target, "");
            return result;
        }
    }
}
