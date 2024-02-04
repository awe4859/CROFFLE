using Newtonsoft.Json.Linq;

namespace Croffle.Data.JsonClasses.Abstract
{
    internal abstract class AJson
    {
        //(역)직렬화
        internal abstract JObject Deserialize(string json);
        internal abstract string Serialize(JObject jobject);

        //json을 불러와 JObject로 내보냄
        internal abstract JObject LoadJObject();
        //item을 추가하고 save함.
        internal abstract void AddItem(string key, string value);
        //item을 지우고 save함
        internal abstract void RemoveItem(string key);

        internal abstract void FindItem(string key, out string value);

        //Json에 참조된 Jobject 저장
        internal abstract void SaveJson(ref JObject jobject);
        //Json파일 Reset
        internal abstract void ResetJson();
    }
}
