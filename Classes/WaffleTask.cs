using CroffleDataManager.SQLiteDB;

namespace Croffle.Classes
{
    internal class WaffleTask : Tasks
    {

        internal WaffleTask()
        {
            db = new SQLiteDB();
            contents_name = Contents.Name(EContents.eWaffle);
        }
        /// <summary>
        /// DB에서 Waffle Class에서 자동으로 생성한 컨텐츠를 Load합니다.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="contentsID"></param>
        internal WaffleTask(string contentsID)
        {
            contents_name = Contents.Name(EContents.eWaffle);

            LoadOnDB(contentsID);
        }
    }
}
