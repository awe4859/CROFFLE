namespace Croffle.Data.SQLite.Interface
{
    internal interface IDBFileManager
    {
        ///<summary>
        ///path 지정. 실행파일/지정위치와 File의 이름을 지정
        ///</summary>
        int SetPath(string exe_path, string filename);

        ///<summary>
        ///DB File의 유무 확인 후 생성
        ///</summary>
        int CreateDB();

        ///<summary>
        ///DB File의 whether확인 후 Remove
        ///</summary>
        int RemoveDB();
    }
}
