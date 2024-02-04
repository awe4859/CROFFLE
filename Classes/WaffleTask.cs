using Croffle.Data.SQLite;

namespace Croffle.Classes
{
    internal class WaffleTask : Tasks
    {
        private SQLiteDB sql;

        internal WaffleTask()
        {
            sql = new SQLiteDB();
            contents_name = Contents.Name(EContents.eWaffle);
            sql.Initialize(contents_name, Task_Struct());
        }
        /// <summary>
        /// DB에서 Waffle Class에서 자동으로 생성한 컨텐츠를 Load합니다.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="contentsID"></param>
        internal WaffleTask(string contentsID)
        {
            contents_name = Contents.Name(EContents.eWaffle);
            sql.Initialize(contents_name, Task_Struct());
            LoadOnDB(contentsID);
        }

        private string Task_Struct()
        {
            string table_struct = @"
CREATE TABLE waffle
(contentsID varchar(9) NOT NULL PRIMARY KEY,
 task_date date,
 dead_line datetime,
 added_time datetime DEFAULT (date('now')),
 title varchar(50),
 color integer,
 place varchar(20),
 alarm bool DEFAULT true,
 done bool DEFAULT false,
 CHECK (contentsID LIKE 'W%'))";
            return table_struct;
        }
    }
}
