using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace Croffle.Data.JsonClasses.Interface
{
    //[Guid("EE6181BD-F7CA-4C8D-8D2C-85F02FE14A5E"),
       // InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IJsonItemManager
    {
        ///<summary>
        ///참조한 JObject에 {Key, Value}의 Tuple로 추가/수정.
        ///</summary>
        void AddItem(string key, string value, ref JObject jobject);

        ///<summary>
        ///Item을 삭제 return이 1이면 성공, 0이면 이미 없음.
        ///</summary>
        int RemoveItem(string key, ref JObject jobject);

        ///<summary>
        ///key값에 해당하는 value를 out. 1이면 성공, 0 이면 없음
        ///</summary>
        int FindItem(string key, out string value, ref JObject jObject);
    }
}
