using Croffle.AnnvAPI.Interface;
using Croffle.AnnvAPI.Interface.Implement;
using Croffle.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Xml;

namespace Croffle.AnnvAPI
{
    internal class _AnniversaryAPI : HttpCon { }

    internal class AnniversaryAPI
    {
        [DllImport("Croffles.dll")]
        public static extern int GetApiKey([MarshalAs(UnmanagedType.IUnknown)] out string apiKey);


        private string apiKey = "";

        private XmlDocument xmlDoc;
        private SQLiteDB sqldb;

        private IHttpCon apiconn;

        internal AnniversaryAPI()
        {
            xmlDoc = new XmlDocument();
            apiconn = new _AnniversaryAPI() as IHttpCon;
            sqldb = new SQLiteDB();

            Console.WriteLine(GetApiKey(out apiKey));

            sqldb.Initialize("annv", AnnvStruct());
        }
        
        internal void SetEveryAnniversaryOnDB(int year, int month)
        {
            apiconn.GetAnniversaryUrl(out string url, apiKey, 1, 10, year, month);
            var response = apiconn.GetResultXML(url, out string result);
            if (response == 1) sqldb.SQL_Reset_Data("annv");
            else return;

            xmlDoc.LoadXml(result);
            SetOnDB();

            apiconn.GetRestDayUrl(out url, apiKey, year, month);
            apiconn.GetResultXML(url, out result);
            xmlDoc.LoadXml(result);
            SetOnDB();
            
            apiconn.GetHolidayUrl(out url, apiKey, year, month);
            apiconn.GetResultXML(url, out result);
            xmlDoc.LoadXml(result);
            SetOnDB();
        }

        internal void GetDailyAnniversary(DateTime locdate, out DataTable table)
        {
            sqldb.SQL_Get_Table("*", "annv", $@"locdate=date('{locdate:yyyy-MM-dd}')", out List<DataTable> tables);
            table = tables[0];
            if (table.Rows == null || table.Rows.Count == 0) table = null;
        }

        private void SetOnDB()
        {
            XmlNodeList xmlist = xmlDoc.GetElementsByTagName("item");
            foreach (XmlNode item in xmlist)
            {
                string xmldate = item["locdate"].InnerText;
                DateTime locdate = DateTime.Parse($@"{xmldate.Substring(0, 4)}-{xmldate.Substring(4, 2)}-{xmldate.Substring(6)}");
                bool isHoliday = item["isHoliday"].InnerXml == "Y" ? true : false;
                string sql = $@"
INSERT INTO annv
SELECT date('{locdate:yyyy-MM-dd}'), {isHoliday}, '{item["dateName"].InnerText}' ";
                sqldb.SQL_Modify(sql);
            }
        }


        private string AnnvStruct()
        {
            string table_struct = @"
CREATE TABLE annv
(locdate date NOT NULL,
isHoliday bool,
dateName text)";
            return table_struct;
        }
    }
}
