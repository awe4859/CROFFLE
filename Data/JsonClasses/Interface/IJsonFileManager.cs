using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace Croffle.Data.JsonClasses.Interface
{
    //[Guid("80CC51F3-B1EF-4DBA-BC3C-712CD4EBF231"),
       // InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IJsonFileManager
    {
        //[PreserveSig]
        ///<summary>
        ///json 파일을 open 후 string 객체에 store
        ///</summary>
        int RefreshJobect(string savePath, out JObject jobject);

        //[PreserveSig]
        ///<summary>
        ///Json 파일 save
        ///</summary>
        int SaveJson(string savePath, ref JObject jobject);

        //[PreserveSig]
        ///<summary>
        ///Json 파일 remove
        ///</summary>
        int RemoveJson(string savePath);
    }
}
