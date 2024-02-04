using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Croffle.Data.JsonClasses.Interface.Implement
{
    public class JsonImplement : IJsonAnalyzer, IJsonFileManager, IJsonItemManager
    {
        //현재 경로 지정
        static string _path = Environment.CurrentDirectory + "\\json";
        DirectoryInfo dir_info = new DirectoryInfo(_path);

        //Json
        //각 Class에서 Serialize를 구현해야함.
        //문자열을 Json으로 Deserialize
        int IJsonAnalyzer.LoadJsonByString(string json, out JObject jobject)
        {
            int result = 1;
            JObject temp = new JObject();

            //string 타입 json을 Parsing(Exception 발생 가능)
            try { temp = JObject.Parse(json); }
            //Exception 발생 시 result = 0(실패)를 반환
            catch (JsonReaderException) { result = 0; }
            finally { jobject = temp; }

            return result;
        }

        int IJsonAnalyzer.SetstringByJson(out string json, ref JObject jobject)
        {
            json = jobject.ToString();
            return 1;
        }

        //참조한 JObject에 {Key, Value}의 Tuple로 추가/수정.
        void IJsonItemManager.AddItem(string key, string value, ref JObject jobject)
        {
            if (jobject.ContainsKey(key))
            {
                try { jobject[key] = JObject.Parse(value); }
                catch(JsonReaderException) { jobject[key] = value; }
            }
            else
            {
                AddMethod(key, value, ref jobject);
            }
        }
        
        int IJsonItemManager.RemoveItem(string key, ref JObject jobject)
        {
            if (jobject.ContainsKey(key))
            {
                jobject.Remove(key);
            }
            else return 0;
            return 1;
        }

        int IJsonItemManager.FindItem(string key, out string value, ref JObject jObject)
        {
            if (jObject.ContainsKey(key))
            {
                value = jObject[key].ToString();
            }
            else { value = null; return 0; }
            return 1;
        }

        //File
        //Json파일을 지정된 경로로 저장
        //함수의 path는 "/~.json" 으로 적는다.
        int IJsonFileManager.SaveJson(string savePath, ref JObject jobject)
        {
            string path = file_check(savePath);
            int result = InputJson(path, ref jobject);
            return result;
        }
        //json 파일 삭제
        int IJsonFileManager.RemoveJson(string savePath)
        {
            if (dir_info.Exists)
            {
                string path = _path + savePath;
                if (File.Exists(path)) { try { File.Delete(path); } catch (Exception) { return 0; } }
            }
            return 1;
        }
        //json파일을 Deserialize하여 Load
        int IJsonFileManager.RefreshJobect(string savePath, out JObject jobject)
        {
            string path = file_check(savePath);
            if (path == null) { jobject = new JObject(); return 0; }

            int result = LoadJson(path, out jobject);
            if(result == 0) jobject = new JObject();
            return result;
        }


        //함수구간
        //File exists를 check후, 없으면 Create, 그 후 전체 Path를 Return
        public string file_check(string savePath)
        {
            string path = dir_info.ToString() + savePath;

            if (!dir_info.Exists) { try { dir_info.Create(); } catch (IOException) { return null; } }
            if (!File.Exists(path)) { try { File.Create(path); } catch (Exception) { return null; } }

            return path;
        }
        //실제로 Json file을 Save하는 Method
        public int InputJson(string path, ref JObject jobejct)
        {
            if (!File.Exists(path))
                try { File.Create(path); } catch (Exception) { return 0; }
            try { File.WriteAllText(path, jobejct.ToString()); } catch(Exception) { return 0; }
            return 1;
        }
        //실제로 Json file을 Read하여 Deserialize하는 Method
        public int LoadJson(string path, out JObject jobject)
        {
            string json = string.Empty;
            try { json = File.ReadAllText(path); }
            catch (IOException) { jobject = new JObject(); return 0; }
            catch (Exception) { jobject = new JObject(); return 0; }

            try { jobject = JObject.Parse(json); }
            catch (JsonReaderException) { jobject = new JObject(); return 0; }
            return 1;
        }

        public void AddMethod(string key, string value, ref JObject jobject)
        {
            //이미 id값으로 Set된 Query가 있는지 Check
            if (jobject.ContainsKey(key))
            {
                try { jobject[key] = JObject.Parse(value); }
                catch (JsonReaderException) { jobject[key] = value; }
            }
            else
            {
                try { jobject.Add(key, JObject.Parse(value)); }
                catch (JsonReaderException) { jobject[key] += value; }//없으면 id와 값을 추가
            }
        }

    }
}
