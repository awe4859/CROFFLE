using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace Croffle.Data.SQLite.Abstract
{
    internal abstract class ASQLiteDB
    {
        //날짜를 주면, 해당 날짜에 있는 과업, 일정을 1개의 테이블로 돌려줌. (테이블: ID, 제목, 시간) - 시간순으로 정렬.
        internal abstract void GetDailyProperty(DateTime date, out List<DataTable> contents);
        
        //테이블 1개를 참조, 열을 선택하여 해당 열의 값을 string List로 out한다.
        internal abstract void GetValueList(ref DataTable table, string column, out List<string> values);
        
        //열(항목), table(컨텐츠(소문자)), 조건을 받아 DB에 SELECT 연산을 실행후, 결과로 나온 Table(여러개일 수 있음)을 out 
        internal abstract void SQL_Get_Table(string column, string table, string where, out List<DataTable> tables);

       
        //SQL INSERT 연산 시 memo 클래스에서 값을 받아 DB에 저장하기 위해 SQL문법에 맞춰 string으로 return
        internal abstract string Get_SQL_String(string contentsID, DateTime memo_day, string title, int color);
        
        //SQL INSERT 연산 시 task 클래스에서 값을 받아 DB에 저장하기 위해 SQL 문법에 맞춰 string으로 return
        internal abstract string Get_SQL_String(string contentsID, DateTime deadline, string title, int color, string place, bool alarm, bool done);
        
        //위와 같음. schedule 클래스에서 값을 받음.
        internal abstract string Get_SQL_String(string contentsID, DateTime s_date, DateTime e_date, string title, int color, string place, bool alarm, bool done, bool canceled);
        

        //Data를 DB에 저장한다.
        internal abstract int SQL_Set_Data(string table, string contentsID, string value_sql);
        
        //DELETE 연산을 진행한다. table(컨텐츠)에서 ID를 찾아 해당 ROW를 DELETE
        internal abstract int SQL_Del_Data(string table, string id);
        
        //특정 컨텐츠 table을 reset한다. - DELETE FROM {table} (WHERE 조건문이 없어 ROWS 삭제)
        internal abstract int SQL_Reset_Data(string table);


        //SQL문법을 직접작성하기 위함. Modify는 Data수정, Search는 연산(data가 return)
        internal abstract void SQL_Modify(string sql);
        internal abstract void SQL_Search(string sql, out DataSet dataSet);
    }
}
