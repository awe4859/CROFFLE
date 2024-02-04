using Croffle.Data.SQLite;

namespace Croffle.Classes.MainAbstract
{
    internal abstract class AMemos : ADailyProperty
    {
        /// <summary>
        /// Memo의 내용을 store
        /// </summary>
        internal string detailText = null;
        internal readonly int detailMaxByte = 50;

        internal abstract void Load(string contentsID);

        internal abstract void SaveData();

        internal abstract void DeleteMemo();

        //내용 수정 (후 저장)
        internal abstract void ModifyDetailText(string newDetailText);
    }
}
