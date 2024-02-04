using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Croffle.Data.SQLite.Interface.Implement
{
    internal class DBManagerImplement : IDBFileManager, IDBItemManager, IDataSetAnalyzer
    {
        //db파일 위치: 실행File 위치\DB\$(name).db
        static string db_path;
        static string db_filename;
        static string dataSource;
        SQLiteDataAdapter adapter;


        //SQLite 에서 sql command 실행 후 return 된 DataSet을 out - SELECT
        private int SQLDSET(string sql, out DataSet dataSet)
        {
            dataSet = new DataSet();

            //SQLiteDataAdapter 설정
            adapter = new SQLiteDataAdapter(sql, dataSource);

            //result 값 DataSet에 Fill
            //adapter.Fill(dataSet);
            try { adapter.Fill(dataSet); }
            //sql command Error
            catch (SQLiteException e) { dataSet = null; Console.WriteLine("SQLite Error\n" + e.Message + "\n" + e.StackTrace); }

            //value exists
            if (dataSet != null) { if (dataSet.Tables.Count > 0) return 1; }
            return 0;
        }

        //SQLite 에서 sql command 실행 - CREATE, UPDATE, DROP(DELETE)
        private int SQLMOD(string sql)
        {
            int result = 1;
            SQLiteConnection conn = new SQLiteConnection(dataSource);
            conn.Open();

            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = sql;
            try { cmd.ExecuteNonQuery(); }
            catch (SQLiteException e) { Console.WriteLine("SQLite Error\n" + e.Message + "\n" + e.StackTrace + "\n" + sql); result = 0; }
            return result;
        }

        int IDBFileManager.SetPath(string execute_path, string filename)
        {
            db_path = Path.Combine(execute_path, $@"Croffle\DB");
            db_filename = filename;
            string full_path = Path.Combine(db_path, $@"{db_filename}.sqlite");
            dataSource = $@"Data Source={full_path}";

            return 1;
        }

        int IDBFileManager.CreateDB()
        {
            DirectoryInfo dir_info = new DirectoryInfo(db_path);
            if(!dir_info.Exists) dir_info.Create();
            string full_path = Path.Combine(db_path, $@"{db_filename}.sqlite");
            if (!File.Exists(full_path)) SQLiteConnection.CreateFile(full_path);

            var result = File.Exists(full_path) ? 1 : 0;
            return result;
        }

        int IDBFileManager.RemoveDB()
        {
            string full_path = $@"{db_path}\{db_filename}.sqlite";
            if (!File.Exists(full_path))
            {
                File.Delete(full_path);
            }
            var result = !File.Exists(full_path) ? 1 : 0;

            return result;
        }

        int IDBItemManager.SelectFrom(string column, string table, string where, out DataSet dataSet)
        {
            string sql = $@"SELECT {column} FROM {table}";
            if (where != "") sql += $@" WHERE {where}";
            var result = SQLDSET(sql, out dataSet);
            return result;
        }

        int IDBItemManager.InsertInto(string table, string values)
        {
            string sql = $@"INSERT INTO {table} {values}";
            var result = SQLMOD(sql);

            return result;
        }

        int IDBItemManager.Delete(string table, string where)
        {
            string sql = $@"DELETE FROM {table}";
            if (where != "")
                sql += $@" WHERE {where}";
            var result = SQLMOD(sql);

            return result;
        }

        int IDBItemManager.DropTable(string table)
        {
            string sql = $@"DROP TABLE {table}";
            var result = SQLMOD(sql);

            return result;
        }

        int IDBItemManager.SQL_Mod(string sql) { return SQLMOD(sql); }
        int IDBItemManager.SQL_Set(string sql, out DataSet dataSet) { return SQLDSET(sql, out dataSet); }


        int IDataSetAnalyzer.GetTablesOnDataSet(ref DataSet dataSet, out List<DataTable> result)
        {
            if (dataSet == null) throw new NullReferenceException(nameof(dataSet));
            if (dataSet.Tables.Count == 0) throw new ArgumentOutOfRangeException(nameof(dataSet));

            result = new List<DataTable>();
            var tables = dataSet.Tables;
            for (int i = 0; i < tables.Count; i++) result.Add(tables[i]);

            return 1;
        }

        int IDataSetAnalyzer.GetListOnTable(ref DataTable table, string columnName, out List<string> result)
        {
            if (table == null) throw new NullReferenceException(nameof(table));
            var clmns = table.Columns[columnName];
            var rows = table.Rows;
            result = new List<string>();
            for (int i = 0; i < rows.Count; i++) result.Add(rows[i][clmns].ToString());

            return 1;
        }
    }
}
