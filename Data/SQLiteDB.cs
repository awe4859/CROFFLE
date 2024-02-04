using Croffle.Data.SQLite.Abstract;
using Croffle.Data.SQLite.Interface;
using Croffle.Data.SQLite.Interface.Implement;
using System;
using System.Collections.Generic;
using System.Data;

namespace Croffle.Data.SQLite
{
    internal class _SQLiteDB : DBManagerImplement { }

    /// <summary>
    /// SQLite DBMS의 반복적인 기능 제공
    /// </summary>
    internal class SQLiteDB : ASQLiteDB
    {
        IDataSetAnalyzer _analyzer;
        IDBFileManager _dbFileManager;
        IDBItemManager _itemManager;

        internal SQLiteDB()
        {
            _analyzer = new _SQLiteDB() as IDataSetAnalyzer;
            _dbFileManager = new _SQLiteDB() as IDBFileManager;
            _itemManager = new _SQLiteDB() as IDBItemManager;
            Initialize();
        }

        ///<summary>
        ///파일 위치 지정 및 생성    
        ///</summary>
        private void Initialize()
        {
            _dbFileManager.SetPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "sqlitedb");
            _dbFileManager.CreateDB();
        }

        /// <summary>
        /// 파일 위치 지정 및 테이블의 존재 확인 후 있으면 1반환. 없으면 생성 후 0반환
        /// </summary>
        /// <param name="table"></param>
        /// <param name="table_struct"></param>
        internal int Initialize(string table, string table_struct)
        {
            //Initialize();

            string set = $@"SELECT COUNT(*) FROM sqlite_master WHERE name='{table}'";
            _itemManager.SQL_Set(set, out DataSet dataSet);
            var count = dataSet.Tables[0].Rows[0][dataSet.Tables[0].Columns[0]].ToString();

            if(Convert.ToInt32(count) == 0)
                _itemManager.SQL_Mod(table_struct);

            return Convert.ToInt32(count);
        }

        ///<summary>
        ///날짜를 주면, 해당 날짜에 있는 컨텐츠를 일정과 메모로 나누어 2개의 테이블로 돌려줌. (테이블: ID, 제목, 시간) - 시간순으로 정렬.
        ///</summary>
        ///<param name="contents">[0] = 일정, [1] = memo ID들</param>
        ///<exception cref="ConstraintException">tables 값이 여러개임</exception>"
        internal override void GetDailyProperty(DateTime date, out List<DataTable> contents)
        {
            contents = new List<DataTable>();
            string sql = $@"
SELECT contentsID, title, start_time as time, color, done FROM
(SELECT contentsID, title, dead_line as start_time, color, done FROM task WHERE task_date=date('{date:yyyy-MM-dd HH:mm}')
UNION
SELECT contentsID, title, start_time, color, done FROM schedule WHERE sche_date=date('{date:yyyy-MM-dd HH:mm}')
UNION
SELECT contentsID, title, dead_line as start_time, color, done FROM waffle WHERE task_date=date('{date:yyyy-MM-dd HH:mm}'))
ORDER BY start_time ASC";

            _itemManager.SQL_Set(sql, out DataSet dataSet);
            _analyzer.GetTablesOnDataSet(ref dataSet, out List<DataTable> tables);
            if (tables.Count > 1) throw new ConstraintException(nameof(tables));
            DataTable temp = tables[0];
            contents.Add(temp);

            sql = $@"
SELECT contentsID, title, color
FROM memo
WHERE memo_day=date('{date:yyyy-MM-dd}')
ORDER BY contentsID ASC";
            _itemManager.SQL_Set(sql, out dataSet);
            _analyzer.GetTablesOnDataSet(ref dataSet, out tables);
            if (tables.Count > 1) throw new ConstraintException(nameof(tables));
            temp = tables[0];
            contents.Add(temp);
        }

        internal void GetDailyProperty(DateTime date, bool done, bool canceled, out List<DataTable> contents)
        {
            contents = new List<DataTable>();
            string task_where = $@"task_date=date('{date:yyyy-MM-dd HH:mm}') ";
            string sche_where = $@"sche_date=date('{date:yyyy-MM-dd HH:mm}') ";
            if (!canceled) sche_where += $@"AND canceled=false ";
            if (!done)
            {
                task_where += $@"AND done=false ";
                sche_where += $@"AND done=false ";
            }
            
            string sql = $@"
SELECT contentsID, title, start_time as time, color FROM
(SELECT contentsID, title, dead_line as start_time, color FROM task WHERE {task_where}
UNION
SELECT contentsID, title, start_time, color FROM schedule WHERE {sche_where}
UNION
SELECT contentsID, title, dead_line as start_time, color FROM waffle WHERE {task_where})
ORDER BY start_time ASC";

            _itemManager.SQL_Set(sql, out DataSet dataSet);
            _analyzer.GetTablesOnDataSet(ref dataSet, out List<DataTable> tables);
            if (tables.Count > 1) throw new ConstraintException(nameof(tables));
            DataTable temp = tables[0];
            contents.Add(temp);

            _itemManager.SelectFrom("contentsID, title", "memo", $@"memo_day=date('{date}')", out dataSet);
            _analyzer.GetTablesOnDataSet(ref dataSet, out tables);
            if (tables.Count > 1) throw new ConstraintException(nameof(tables));
            temp = tables[0];
            contents.Add(temp);
        }

        ///<summary>
        ///테이블 1개를 참조, 열을 선택하여 해당 열의 값을 string List로 out한다.
        ///</summary>
        internal override void GetValueList(ref DataTable table, string column, out List<string> values)
        {
            _analyzer.GetListOnTable(ref table, column, out values);
        }

        ///<summary>
        ///열(항목), table(컨텐츠(소문자)), 조건을 받아 DB에 SELECT 연산을 실행후, 결과로 나온 Table(여러개일 수 있음)을 out 
        ///</summary>
        internal override void SQL_Get_Table(string column, string table, string where, out List<DataTable> tables)
        {
            if (_itemManager.SelectFrom(column, table, where, out DataSet dataSet) == 1)
                _analyzer.GetTablesOnDataSet(ref dataSet, out tables);
            else tables = null;
        }

        ///<summary>
        ///SQL INSERT 연산 시 memo 클래스에서 값을 받아 DB에 저장하기 위해 SQL문법에 맞춰 string으로 return
        ///</summary>
        internal override string Get_SQL_String(string contentsID,DateTime memo_day, string title, int color)
        {
            string result = $@"SELECT '{contentsID}', date('{memo_day:yyyy-MM-dd HH:mm}'), datetime('{DateTime.Now:yyyy-MM-dd HH:mm}'), '{title}', '{color}'";
            return result;
        }

        ///<summary>
        ///SQL INSERT 연산 시 task 클래스에서 값을 받아 DB에 저장하기 위해 SQL 문법에 맞춰 string으로 return
        ///</summary>
        internal override string Get_SQL_String(string contentsID, DateTime deadline, string title, int color, string place, bool alarm, bool done)
        {
            string result = $@"SELECT '{contentsID}', date('{deadline:yyyy-MM-dd HH:mm}'), datetime('{deadline:yyyy-MM-dd HH:mm}'), datetime('{DateTime.Now:yyyy-MM-dd HH:mm}'), '{title}', {color}, '{place}', {alarm}, {done}";
            return result;
        }

        ///<summary>
        ///SQL INSERT 연산 시 schedule 클래스에서 값을 받아 DB에 저장하기 위해 SQL 문법에 맞춰 string으로 return
        ///</summary>
        internal override string Get_SQL_String(string contentsID, DateTime s_date, DateTime e_date, string title, int color, string place, bool alarm, bool done, bool canceled)
        {
            string result = $@"SELECT '{contentsID}', date('{s_date:yyyy-MM-dd HH:mm}'), datetime('{s_date:yyyy-MM-dd HH:mm}'), datetime('{e_date:yyyy-MM-dd HH:mm}'), datetime('{DateTime.Now:yyyy-MM-dd HH:mm}'), '{title}', {color}, '{place}', {alarm}, {done}, {canceled}";
            return result;
        }

        ///<summary>
        ///Data를 DB에 저장한다.
        ///</summary>
        internal override int SQL_Set_Data(string table, string contentsID, string value_sql)
        {
            _itemManager.Delete(table, $@"contentsID='{contentsID}'");
            var result = _itemManager.InsertInto(table, value_sql);
            return result;
        }

        ///<summary>
        ///DELETE 연산을 진행한다. table(컨텐츠)에서 ID를 찾아 해당 ROW를 DELETE
        ///</summary>
        internal override int SQL_Del_Data(string table, string id)
        {
            return _itemManager.Delete(table, $@"contentsID='{id}'");
        }

        ///<summary>
        ///특정 컨텐츠 table을 reset한다. - DELETE FROM {table} (WHERE 조건문이 없어 ROWS 삭제)
        ///</summary>
        internal override int SQL_Reset_Data(string table)
        {
            return _itemManager.Delete(table, "");
        }

        ///<summary>
        ///SQL문법을 직접작성하기 위함. Modify는 Data수정, Search는 연산(data가 return)
        ///</summary>
        internal override void SQL_Modify(string sql)
        {
            _itemManager.SQL_Mod(sql);
        }

        ///<summary>
        ///SQL문법을 직접작성하기 위함. Modify는 Data수정, Search는 연산(data가 return)
        ///</summary>
        internal override void SQL_Search(string sql, out DataSet dataSet)
        {
            _itemManager.SQL_Set(sql, out dataSet);
        }
    }
}
