using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Croffle.AnnvAPI.Interface
{
    internal interface IHttpCon
    {
        /// <summary>
        /// API에 요청을 보내고 응답을 받아 XML을 문자로 내보냅니다.
        /// </summary>
        int GetResultXML(string url, out string resultXML);

        /// <summary>
        /// 기념일 정보를 가져오기 위한 URL을 조합하여 내보냅니다.
        /// </summary>
        int GetAnniversaryUrl(out string url, string serviceKey, int page, int rows, int year, int month);

        /// <summary>
        /// 공휴일 정보를 가져오기 위한 URL을 조합하여 내보냅니다.
        /// </summary>
        int GetRestDayUrl(out string url, string serviceKey, int year, int month);

        /// <summary>
        /// 국경일 정보를 가져오기 위한 URL을 조합하여 내보냅니다.
        /// </summary>
        int GetHolidayUrl(out string url, string serviceKey, int year, int month);
    }
}
