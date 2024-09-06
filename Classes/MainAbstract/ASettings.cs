namespace CROFFLE_WPF.Classes.MainAbstract
{
    internal abstract class ASettings
    {
        internal bool auto_start = true, system_tray = true, show_week = true, show_cancel = false, show_done = true, done_doubleC = true,
            show_oneline = true, show_color = true, alarm = true, show_warning = false;

        internal int warning_minute = 0;
        internal int v_size = 800, h_size = 400;

        internal abstract int LoadSetting();
        internal abstract int SaveSetting();
        internal abstract int GenerateKey();
        internal abstract int SaveAccount();
        internal abstract int LoadAccount();
    }
}
