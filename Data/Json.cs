using Croffle.Data.JsonClasses.Abstract;
using Croffle.Data.JsonClasses.Interface;
using Croffle.Data.JsonClasses.Interface.Implement;
using Newtonsoft.Json.Linq;

namespace Croffle.Data.JsonClasses
{
    //[ComImport, Guid("16E2A046-8290-48D4-A7FB-D16E04CF7067")]
    internal class _Json : JsonImplement { }
    internal class Json : AJson
    {
        //file에 save된 data를 계속 refresh하며 사용
        JObject jobject;
        readonly string path;
        readonly IJsonAnalyzer analyzer;
        readonly IJsonFileManager fileManager;
        readonly IJsonItemManager itemManager;
        //filename = "~.json"

        /// <summary>
        ///file 이름을 지정하여 Initialize 해야함
        /// </summary>
        internal Json(string filename)
        {
            if (filename != null) path = $@"\{filename}";
            jobject = new JObject();
            analyzer = new _Json() as IJsonAnalyzer;
            fileManager = new _Json() as IJsonFileManager;
            itemManager = new _Json() as IJsonItemManager;
        }

        /// <summary>
        ///JObject를 Load함.
        /// </summary>
        
        internal override JObject LoadJObject()
        {
            int result = fileManager.RefreshJobect(path, out jobject);
            if (result == 0) { return new JObject(); }
            return jobject;
        }

        /// <summary>
        ///Key, Value 쌍을 추가함
        /// </summary>
        internal override void AddItem(string key, string value)
        {
            fileManager.RefreshJobect(path, out jobject);
            itemManager.AddItem(key, value, ref jobject);
            fileManager.SaveJson(path, ref jobject);
        }

        /// <summary>
        ///key value쌍을 삭제함
        /// </summary>
        internal override void RemoveItem(string key)
        {
            fileManager.RefreshJobect(path, out jobject);
            itemManager.RemoveItem(key, ref jobject);
            fileManager.SaveJson(path, ref jobject);
        }

        /// <summary>
        ///key, value쌍을 찾아 value를 내보냄
        /// </summary>
        internal override void FindItem(string key, out string value)
        {
            fileManager.RefreshJobect(path, out jobject);
            itemManager.FindItem(key, out value, ref jobject);
        }

        /// <summary>
        ///Json파일 save
        /// </summary>
        internal override void SaveJson(ref JObject jobject)
        {
            fileManager.SaveJson(path, ref jobject);
        }

        /// <summary>
        ///Json File remove 후, 빈 json 생성
        /// </summary>
        internal override void ResetJson()
        {
            fileManager.RemoveJson(path);
            jobject = new JObject();
            fileManager.SaveJson(path, ref jobject);
        }

        /// <summary>
        ///json을 string으로 return
        /// </summary>
        internal override string Serialize(JObject json)
        {
            analyzer.SetstringByJson(out string result, ref json);
            return result;
        }

        /// <summary>
        ///json을 JObject로 return
        /// </summary>
        internal override JObject Deserialize(string json)
        {
            analyzer.LoadJsonByString(json, out JObject result);
            return result;
        }
    }
}
