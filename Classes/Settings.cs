using Croffle.Classes.MainAbstract;
using Croffle.Classes.MainInterface;
using Croffle.Classes.MainInterface.Implement;
using Croffle.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Croffle.Classes
{
    internal enum ModeDES
    {
        Encrypt,
        Decrypt,
    }
    
    internal class _Setting : SettingImplement { }

    /// <summary>
    /// 세팅값을 관리하는 Class
    /// </summary>
    internal class Settings : ASettings
    {
        SQLiteDB sql;

        ISetting _Setting = new _Setting() as ISetting;
        internal bool logged_in = false;
        private byte[] pkey = new byte[8];
        private string waffleid, wafflepw;

        /// <summary>
        /// Initializing - Setting을 Load하고 Table확인.
        /// </summary>
        internal Settings()
        {
            sql = new SQLiteDB();

            sql.Initialize(Contents.Name(EContents.eAccount), Account_Struct());
            sql.SQL_Get_Table("*", Contents.Name(EContents.eAccount), "", out List<DataTable> tables);
            var res = tables[0].Rows.Count;

            //계정 정보 있음
            if (res != 0) { LoadAccount(); }
            //없음
            else { logged_in = false; }

            res = sql.Initialize(Contents.Name(EContents.eSetting), Setting_Struct());
            if (res == 0) SaveSetting(); //기본 세팅값을 생성하여 저장

            LoadSetting();
        }

        /// <summary>
        /// 계정을 암/복호화하기 위한 키를 생성합니다.
        /// </summary>
        /// <returns></returns>
        internal override int GenerateKey()
        {
            var result = _Setting.GeneratePKEY(out pkey);
            return result;
        }

        internal void SetAccount(string ID, string PW)
        {
            waffleid = ID;
            wafflepw = PW;
            GenerateKey();
        }

        internal void GetAccount(out string userid, out string userpw)
        {
            userid = waffleid;
            userpw = wafflepw;
        }

        /// <summary>
        /// 계정 정보를 DB에 저장합니다.
        /// </summary>
        internal override int SaveAccount()
        {
            _Setting.DES_((int)ModeDES.Encrypt, ref waffleid, ref wafflepw, ref pkey, out string en_id, out string en_pw);
            string table = Contents.Name(EContents.eAccount);
            var res1 = sql.SQL_Set_Data(table, "userid", $@"SELECT 'userid', '{en_id}'");
            var res2 = sql.SQL_Set_Data(table, "userpw", $@"SELECT 'userpw', '{en_pw}'");
            var res3 = sql.SQL_Set_Data(table, "pkey", $@"SELECT 'pkey', '{Convert.ToBase64String(pkey)}'");

            return res1 * res2 * res3;
        }

        /// <summary>
        /// 계정 정보를 DB에서 Load합니다
        /// </summary>
        internal override int LoadAccount()
        {
            string table = Contents.Name(EContents.eAccount);
            int result;

            try
            {
                sql.SQL_Get_Table($@"value", table, "contentsID='userid'", out List<DataTable> tables);
                string en_id = tables[0].Rows[0][0].ToString();

                sql.SQL_Get_Table($@"value", table, "contentsID='userpw'", out tables);
                string en_pw = tables[0].Rows[0][0].ToString();

                sql.SQL_Get_Table($@"value", table, "contentsID='pkey'", out tables);
                pkey = Convert.FromBase64String(tables[0].Rows[0][0].ToString());

                result = _Setting.DES_((int)ModeDES.Decrypt, ref en_id, ref en_pw, ref pkey, out waffleid, out wafflepw);
            }
            catch (ArgumentNullException) { return 0; }
            catch (Exception) { return 0; }
            logged_in = true;

            return result;
        }

        internal int RemoveAccount()
        {
            string table = Contents.Name(EContents.eAccount);
            return sql.SQL_Reset_Data(table);
        }

        /// <summary>
        /// 세팅 값을 DB에서 가져옵니다.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal override int LoadSetting()
        {
            string table = Contents.Name(EContents.eSetting);
            var fields = typeof(ASettings).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(bool))
                {
                    List<DataTable> tables;
                    sql.SQL_Get_Table($@"value", table, $@"contentsID='{field.Name}'", out tables);

                    string value;
                    if (tables != null)
                    {
                        if (tables[0].Rows.Count > 0)
                        {
                            value = tables[0].Rows[0][0].ToString();
                        }
                        else value = null;
                    }
                    else value = null;

                    if (value != null)
                    {
                        try { field.SetValue(this, Convert.ToBoolean(value)); }
                        catch (FormatException) { return 0; }
                    }
                    else return 0;
                }
            }
            return 1;
        }

        /// <summary>
        /// 세팅값을 DB에 저장합니다. 오류 발생 시 0을 반환합니다.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal override int SaveSetting()
        {
            string table = Contents.Name(EContents.eSetting);
            var fields = typeof(ASettings).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(bool))
                {
                    var value = Convert.ToBoolean(field.GetValue(this));
                    string sql_value = $@"SELECT '{field.Name}', {value}";

                    var result = sql.SQL_Set_Data(table, field.Name, sql_value);
                    if (result == 0) return 0;
                }

                if (field.FieldType == typeof(int))
                {

                    int value = Convert.ToInt32(field.GetValue(this));
                    string sql_value = $@"SELECT '{field.Name}', {value}";

                    var result = sql.SQL_Set_Data(table, field.Name, sql_value);
                    if (result == 0) return 0;
                }
                else return 0;
            }
            return 1;
        }

        /// <summary>
        /// DB에서 setting 테이블의 구조를 선언하기 위한 sql명령을 문자열로 반환합니다.
        /// </summary>
        /// <returns></returns>
        private string Setting_Struct()
        {
            string table_struct = @"
CREATE TABLE setting
(contentsID text NOT NULL PRIMARY KEY,
value boolean NOT NULL DEFAULT false)";
            return table_struct;
        }

        /// <summary>
        /// DB에서 account 테이블의 구조를 선언하기 위한 sql명령을 문자열로 반환합니다.
        /// </summary>
        /// <returns></returns>
        private string Account_Struct()
        {
            string table_struct = $@"
CREATE TABLE account
(contentsID text NOT NULL PRIMARY KEY,
 value text NOT NULL)";
            return table_struct;
        }
    }
}