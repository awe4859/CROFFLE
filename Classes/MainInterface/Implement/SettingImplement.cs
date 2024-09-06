using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CROFFLE_WPF.Classes.MainInterface.Implement
{
    internal class SettingImplement : ISetting
    {
        int ISetting.GeneratePKEY(out byte[] key)
        {
            byte[] pkey = new byte[8];
            new Random().NextBytes(pkey);
            key = pkey;
            return 1;
        }

        int ISetting.DES_(int en_de, ref string id, ref string pw, ref byte[] key, out string userid, out string passwd)
        {
            var des = new DESCryptoServiceProvider()
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,

                Key = key,
                IV = key
            };

            var property = new
            {
                transform = en_de == 0 ? des.CreateEncryptor() : des.CreateDecryptor(),
                idvalue = en_de == 0 ? Encoding.UTF8.GetBytes(id.ToCharArray()) : Convert.FromBase64String(id),
                pwvalue = en_de == 0 ? Encoding.UTF8.GetBytes(pw.ToCharArray()) : Convert.FromBase64String(pw)
            };

            var ms = new MemoryStream();
            var cryStream = new CryptoStream(ms, property.transform, CryptoStreamMode.Write);
            var data = property.idvalue;

            cryStream.Write(data, 0, data.Length);
            cryStream.FlushFinalBlock();
            userid = en_de == 0 ? Convert.ToBase64String(ms.ToArray()).Trim('\0') : Encoding.UTF8.GetString(ms.GetBuffer()).Trim('\0');

            ms = new MemoryStream();
            cryStream = new CryptoStream(ms, property.transform, CryptoStreamMode.Write);
            data = property.pwvalue;

            cryStream.Write(data, 0, data.Length);
            cryStream.FlushFinalBlock();
            passwd = en_de == 0 ? Convert.ToBase64String(ms.ToArray()).Trim('\0') : Encoding.UTF8.GetString(ms.GetBuffer()).Trim('\0');

            return 1;
        }
    }
}
