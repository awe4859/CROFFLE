using System;
using System.IO;
using System.Net;

namespace Croffle.AnnvAPI.Interface.Implement
{
    internal class HttpCon : IHttpCon
    {
        string baseUrl = "http://apis.data.go.kr/B090041/openapi/service/SpcdeInfoService/";
        int IHttpCon.GetAnniversaryUrl(out string url, string serviceKey, int page, int rows, int year, int month)
        {
            url = $@"{baseUrl}getAnniversaryInfo?ServiceKey={serviceKey}&pageNo={page}&numOfRows={rows}&solYear={year:0000}&solMonth={month:00}";
            return 1;
        }

        int IHttpCon.GetHolidayUrl(out string url, string serviceKey, int year, int month)
        {
            url = $@"{baseUrl}getHoliDeInfo?ServiceKey={serviceKey}&solYear={year:0000}&solMonth={month:00}";
            return 1;
        }

        int IHttpCon.GetRestDayUrl(out string url, string serviceKey, int year, int month)
        {
            url = $@"{baseUrl}getRestDeInfo?ServiceKey={serviceKey}&solYear={year:0000}&solMonth={month:00}";
            return 1;
        }

        int IHttpCon.GetResultXML(string url, out string resultXML)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            resultXML = string.Empty;
            HttpWebResponse response;
            try
            {
                using (response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    resultXML = sr.ReadToEnd();
                }
            }
            catch (Exception) { return 0; }
            return 1;
        }
    }
}
