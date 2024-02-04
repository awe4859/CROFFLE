using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace Croffle.Data.JsonClasses.Interface
{
    //[Guid("3F731A76-3AF9-4CB5-90AC-9835A86C1183"),
    // InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]

    ///<summary>
    ///json 파일 분석 클래스
    ///</summary>
    internal interface IJsonAnalyzer
    {
        //[PreserveSig]
        ///<summary>
        ///string으로 받은 json을 Parsing
        ///</summary>
        int LoadJsonByString(string json, out JObject jobject);

        //[PreserveSig]
        ///<summary>
        ///json을 string으로 serialize
        ///</summary>
        int SetstringByJson(out string json, ref JObject jobject);
    }
}
