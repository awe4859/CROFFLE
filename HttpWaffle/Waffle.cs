using Croffle.HttpWaffle.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using Croffle.HttpWaffle.Interface.Implement;
using Croffle.Data.SQLite;
using Croffle.HttpWaffle.Abstract;
using System.Data;
using System.Drawing;
using Croffle.Classes;
using System.Windows.Media;

namespace Croffle.HttpWaffle
{
    
    
    internal class _HtmlAnayzer : HtmlAnalyzer { }

    /// <summary>
    /// Waffle 기능 제공
    /// </summary>
    internal class Waffle : AWaffle
    {
        private SQLiteDB sql;
        protected WkuWebReq req;
        protected IHtmlAnalyzer htmlA;
        private string sql_key;
        internal string username;

        /// <summary>
        /// Initializing
        /// </summary>
        ///
        internal Waffle()
        {
            sql = new SQLiteDB();
            htmlA = new _HtmlAnayzer() as IHtmlAnalyzer;
            req = new WkuWebReq();
            sql.Initialize("waffle", Waffle_Struct());
        }

        /// <summary>
        ///이니셜라이징
        /// </summary>
        internal override int InitailizeWaffle(string userid, string passwd)
        {
            var result = req.GetWKULoginSession(userid, passwd, out _realCookie);

            if (result == 1)
            {
                req.GetHtmlGET(EPage.eReturnJSP, null, "https://auth.wku.ac.kr/", out htmlDoc, ref _realCookie);
                req.GetHtmlGET(EPage.eMain, null, out htmlDoc, ref _realCookie);

                try
                {
                    sNo = htmlA.Get_frm_onHtml("g_verifyno", ref htmlDoc);
                    sql_key = htmlA.Get_frm_onHtml("g_secure", ref htmlDoc);
                    username = htmlA.Get_frm_onHtml("g_verifynm", ref htmlDoc);
                }
                catch (Exception) { return 0; }
            }
            return result;
        }

        /// <summary>
        /// 강의 과제을 DB에 저장합니다.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal override int SetLectureTask()
        {
            sql.SQL_Reset_Data(Contents.Name(EContents.eWaffle));
            var list = GetLectureList();
            foreach(var item in list)
            {
                SetLectureStat(item);
            }
            return 1;
        }

        /// <summary>
        ///특정 강의의 진행 현황 중 과제나 영상만을 DB로 save
        /// </summary>
        private int SetLectureStat(string lectureID)
        {
            string query = $@"lctr_mngno={lectureID}";
            //강의실 페이지, 쿼리로는 강의 mngno와 cookie를 참조로 넘기고, html document를 받음
            req.GetHtmlGET(EPage.eAttend, query, out htmlDoc, ref _realCookie);
            string node = $@"//*[@id=""frm""]/div/div/div/div/div/div/div[3]/div/table/tbody/tr"; //강의 테이블 노드 설정

            //노드를 통해 table의 행 내의 html를 string형태의 list로 가져옴.
            htmlA.GetManyInfo(node, out List<string> result, ref htmlDoc);
            HtmlDocument temp = new HtmlDocument();

            string table_state; //테이블에서 해당강의의 상태 확인을 위해 설정

            if (result.Count == 0) return 0;


            for (int i = 0; i < result.Count; i++) //1개의 행씩 string 형태의 html을 html document로 Load
            {
                temp.LoadHtml(result[i]);

                // td[2]/i에는 강의에 과제/강의가 있는지를 표기함. 과제 행에는 <i>가 없음
                //  => 과제/강의 행에서는 <i>노드가 없기 때문에 NullReferenceException이 발생함.
                try { table_state = htmlA.GetInfoByNode("td[2]/i", ref temp); }
                catch (NullReferenceException) //즉, Exception이 발생한 행은 과제/강의의 정보가 있는 행임을 알 수 있음.
                {
                    string id = GenerateID();
                    DateTime deadline = DateTime.Parse(htmlA.GetInfoByNode("td[8]", ref temp));
                    string title = $@"({htmlA.GetInfoByNode("td[4]", ref temp)}){htmlA.GetInfoByNode("td[5]", ref temp)}";
                    string status;
                    try { status = htmlA.GetInfoByNode("td[9]/i", ref temp); }
                    catch (NullReferenceException) { status = "block"; }
                    bool done;
                    int color_argb = ToArgb(Colors.RoyalBlue);
                    if (status == "verified") { done = true; color_argb = ToArgb(Colors.LimeGreen); }
                    else
                    {
                        if (deadline < DateTime.Now) color_argb = ToArgb(Colors.OrangeRed);
                        done = false;
                    }
                    
                    var value = sql.Get_SQL_String(id, deadline, title, color_argb, "waffle", true, done);
                    sql.SQL_Set_Data("waffle", id, value);
                }
            }

            return 1;
        }

        /// <summary>
        /// 새로운 ID를 생성함. 형식: W{00000000}
        /// </summary>
        internal string GenerateID()
        {
            string search = $@"
SELECT contentsID
FROM waffle
ORDER BY contentsID DESC";

            sql.SQL_Search(search, out DataSet dataSet);

            int count;
            if (dataSet.Tables != null)
            {
                if (dataSet.Tables[0].Rows.Count > 0)
                    count = Convert.ToInt32(Convert.ToString(dataSet.Tables[0].Rows[0][0]).Substring(1));
                else count = -1;
            }
            else count = -1;
            string result = $@"W{count + 1:00000000}";
            return result;
        }

        /// <summary>
        ///강의 ID를 List로 return
        /// </summary>
        private List<string> GetLectureList()
        {
            //Json을 받아옴
            req.GetJsonPOST(GetMainPostData(EPost.eMain), ref _realCookie, out string json);

            //JObject로 파싱
            JObject temp = JObject.Parse(json);

            //ds_oparam(강의목록)만을 파싱하여 jobject에 store함.
            JArray jArray = JArray.Parse(temp["ds_oparam"].ToString());

            //강의 리스트 생성
            List<string> lectures = new List<string>();
            for (int i = 0; i < jArray.Count; i++)
            {
                lectures.Add(jArray[i]["lctr_mngno"].ToString());
            }

            return lectures;
        }

        private int ToArgb(int a, int r, int g, int b)
        {
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        private int ToArgb(Color color)
        {
            return ToArgb(color.A, color.R, color.G, color.B);
        }


        /// <summary>
        ///Main에서 POSTData의 형태를 잡아줌.
        /// </summary>
        private string GetMainPostData(EPost ePost)
        {
            WafflePage wp = new WafflePage();
            var result = $@"_SQL_ID={wp[ePost]}&verifyno={sNo}&teach=0&cmplt_yn=0&close_yn=0&_SECURE_KEY={sql_key}";
            return result;
        }

        /// <summary>
        /// 구조는 tasks와 동일함
        /// </summary>
        /// <returns></returns>
        private string Waffle_Struct()
        {
            string table_struct = @"
CREATE TABLE waffle
(contentsID varchar(9) NOT NULL PRIMARY key,
 task_date date,
 dead_line datetime,
 added_time datetime DEFAULT (date('now')),
 title varchar(50),
 color varchar(15),
 place varchar(20),
 alarm bool DEFAULT false,
 done bool DEFAULT false,
 CHECK (contentsID LIKE 'W%'))";
            return table_struct;
        }
    }
}
