using System;
using System.Collections.Generic;
using System.Data;

namespace Croffle.Data.SQLite.Interface
{
    internal interface IDataSetAnalyzer
    {
        ///<summary>
        ///DataSet에서 DataTable을 List로 내보냄
        ///</summary>
        ///<exception cref="NullReferenceException">DataSet은 null 일 수 없음</exception>
        ///<exception cref="ArgumentOutOfRangeException">dataSet에 테이블이 없음</exception>
        int GetTablesOnDataSet(ref DataSet dataSet, out List<DataTable> result);

        ///<summary>
        ///DataTable에서 특정 열(Column)의 값들을 리스트로 반환
        ///</summary>
        ///<exception cref="NullReferenceException">dataTable을 null 일 수 없음</exception>
        int GetListOnTable(ref DataTable dataTable, string columnName, out List<string> result);
    }
}
