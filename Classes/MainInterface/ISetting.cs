namespace CROFFLE_WPF.Classes.MainInterface
{
    internal interface ISetting
    {
        /// <summary>
        /// 암호화키를 랜덤생성
        /// </summary>
        int GeneratePKEY(out byte[] key);

        /// <summary>
        /// 암·복호화.
        /// </summary>
        int DES_(int des_mode, ref string id, ref string pw, ref byte[] key, out string userid, out string passwd);
    }
}
