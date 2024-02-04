using System.Data;

namespace Croffle.Data.SQLite.Interface
{
    internal interface IDBItemManager
    {
        ///<summary>
        ///SELECT 연산, 행이름(column)을 comma로 구분, 컨텐츠 이름과 조건식을 사용
        ///DataSet으로 값을 내보냄.
        ///</summary>
        ///<param name="column"> {column name}={values} [{AND/OR} ...] </param>
        int SelectFrom(string column, string table, string where, out DataSet dataSet);

        ///<summary>
        ///INSERT INTO을 수행. table과 value를 이용한다.
        ///넣을 행(values)가 여러개인 경우 values = {values1} UNION {values2}..
        ///</summary>
        int InsertInto(string table, string values);

        ///<summary>
        ///DELETE 수행, WHERE가 없으면 테이블 초기화
        ///</summary>
        int Delete(string table, string where);

        ///<summary>
        ///DROP Table {table} 수행
        ///</summary>
        int DropTable(string table);

        ///<summary>
        ///SQLite 에서 sql command 실행 후 return 된 DataSet을 out - SELECT
        ///</summary>
        int SQL_Set(string sql, out DataSet dataSet);
        ///<summary>
        ///SQLite 에서 sql command 실행 - CREATE, UPDATE, DROP(DELETE)
        ///</summary>
        int SQL_Mod(string sql);

    }
}
