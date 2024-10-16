using CROFFLE_WPF.Classes.MainAbstract;
using CROFFLE_WPF.Classes.MainInterface;
using CROFFLE_WPF.Classes.MainInterface.Implement;
using CroffleDataManager.SQLiteDB;
using Microsoft.Win32;
using System;
using System.Data;
using System.Reflection;
using System.Xml.Linq;

namespace CROFFLE_WPF.Classes
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
        SQLiteDB db;

        ISetting _Setting = new _Setting() as ISetting;
        internal bool logged_in = false;
        private byte[] pkey = new byte[8];
        private string waffleid = string.Empty, wafflepw = string.Empty;
        
        private string sAccount = Contents.Name(EContents.eAccount);
        private string sSetting = Contents.Name(EContents.eSetting);

        internal string sno = string.Empty, sname = string.Empty;

        /// <summary>
        /// Initializing - Setting을 Load하고 Table확인.
        /// </summary>
        internal Settings()
        {
            db = new SQLiteDB();

            Initialize();
        } // Settings

        internal void Initialize()
        {
            Console.WriteLine("[Settings] Initialize");
            db.LoadOnDB(sAccount, out DataTable table);
            var row_count = table.Rows.Count;

            if (row_count != 0) { LoadAccount(); logged_in = true; }
            else { logged_in = false; }

            db.LoadOnDB(sSetting, out table);
            row_count = table.Rows.Count;
            if (row_count != 0) { LoadSetting(); }
            else { SaveSetting(); }
        } // Initialize


        /// <summary>
        /// 계정을 암/복호화하기 위한 키를 생성합니다.
        /// </summary>
        /// <returns></returns>
        internal override int GenerateKey()
        {
            Console.WriteLine("> [GenerateKey] Generating...");
            var result = _Setting.GeneratePKEY(out pkey);
            return result;
        } // GenerateKey

        internal void SetAccount(string ID, string PW)
        {
            Console.WriteLine("[Settings] SetAccount");
            waffleid = ID;
            wafflepw = PW;
            GenerateKey();
        } // SetAccount

        internal void GetAccount(out string userid, out string userpw)
        {
            Console.WriteLine("[Settings] GetAccount");
            userid = waffleid;
            userpw = wafflepw;
        } // GetAccount

        /// <summary>
        /// 계정 정보를 DB에 저장합니다.
        /// </summary>
        internal override int SaveAccount()
        {
            Console.WriteLine("[Settings] SaveAccount");
            _Setting.DES_((int)ModeDES.Encrypt, ref waffleid, ref wafflepw, ref pkey, out string en_id, out string en_pw);
            db.SaveOnDB(sAccount, $@"0, '{en_id}', '{en_pw}', '{Convert.ToBase64String(pkey)}'");
            return 1;
        } // SaveAccount

        /// <summary>
        /// 계정 정보를 DB에서 Load합니다
        /// </summary>
        internal override int LoadAccount()
        {
            Console.WriteLine("[Settings] LoadAccount from DB");
            db.LoadOnDB(sAccount, out DataTable table);

            if (table.Rows.Count == 0) return 0;
            string en_id = table.Rows[0]["userid"].ToString() ?? "";
            string en_pw = table.Rows[0]["userpw"].ToString() ?? "";
            pkey = Convert.FromBase64String(table.Rows[0]["pkey"].ToString() ?? "");

            _Setting.DES_((int)ModeDES.Decrypt, ref en_id, ref en_pw, ref pkey, out waffleid, out wafflepw);

            return 1;
        } // LoadAccount

        internal int RemoveAccount()
        {
            db.ResetTable(sAccount);
            return 1;
        } // RemoveAccount

        /// <summary>
        /// 세팅 값을 DB에서 가져옵니다.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal override int LoadSetting()
        {
            Console.WriteLine("[Settings] LoadSetting from DB");
            DataTable? table = null;
            var fields = typeof(ASettings).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            try
            {
                db.LoadOnDB(sSetting, out table);
            }
            catch (NullReferenceException)
            {
                return 0;
            }
            if (table == null) return 0;

            foreach (var field in fields)
            {
                DataRow[] row;
                if (field.FieldType == typeof(bool))
                {
                    row = table.Select($@"keyname = '{field.Name}'");
                    string value;
                    if(row.Length >0) value = row[0]["value"].ToString() ?? string.Empty;
                    else value = string.Empty;

                    if (value != null && value != string.Empty)
                    {
                        var res = bool.TryParse(value, out bool cValue);
                        if (res) field.SetValue(this, cValue);
                        else return 0;
                    }
                    else return 0;
                }
                else if (field.FieldType == typeof(int))
                {
                    row = table.Select($@"keyname = '{field.Name}'");
                    string value;
                    if (row.Length > 0) value = row[0]["value"].ToString() ?? string.Empty;
                    else value = string.Empty;

                    if (value != null && value != string.Empty)
                    {
                        var res = int.TryParse(value, out int cValue);
                        if (res) field.SetValue(this, cValue);
                        else return 0;
                    }
                    else return 0;
                }
                else return 0;
            }
            return 1;
        } // LoadSetting

        /// <summary>
        /// 세팅값을 DB에 저장합니다. 오류 발생 시 0을 반환합니다.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal override int SaveSetting()
        {
            Console.WriteLine("[Settings] SaveSetting to DB");
            var fields = typeof(ASettings).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(bool))
                {
                    db.SaveOnDB(sSetting, $@"'{field.Name}', '{field.GetValue(this)}'");
                }

                else if (field.FieldType == typeof(int))
                {
                    db.SaveOnDB(sSetting, $@"'{field.Name}', '{field.GetValue(this)}'");
                }

                else return 0;  // if문을 조건을 만족하지 않는 경우 무조건 return 0을 하기 때문에 오류 발생
            }
            return 1;
        }

        internal void UpdateAutoStart()
        {
            if (OperatingSystem.IsWindows())
            {
                string regStartUpPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                RegistryKey? key = Registry.CurrentUser.OpenSubKey(regStartUpPath, true);

                if (key == null) throw new NullReferenceException("Registry Key is null");

                if (auto_start) key.SetValue("Croffle", $@"{Environment.CurrentDirectory}\Croffle.exe");
                else key.DeleteValue("Croffle", false);
            }
            else if (OperatingSystem.IsMacOS())
            {
                var plistPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Croffle", "Croffle.plist");

                XDocument plist = new(
                    new XElement("plist",
                        new XAttribute("version", "1.0"),
                        new XElement("dict",
                            new XElement("key", "Label"),
                            new XElement("string", "Croffle"),
                            new XElement("key", "ProgramArguments"),
                            new XElement("array",
                                new XElement("string", "open"),
                                new XElement("string", $@"{Environment.CurrentDirectory}\Croffle.exe")
                            ),
                            new XElement("key", "RunAtLoad"),
                            new XElement("true/>")
                        )
                    )
                );
                var path = Path.GetDirectoryName(plistPath);
                if (path == null) throw new NullReferenceException("Path is null");

                Directory.CreateDirectory(path);
            }
        } // ChangeAutoStart
    } // Settings
} // CROFFLE_WPF.Classes