using CROFFLE_WPF.Classes;
using CROFFLE_WPF.WPF_xamls.Dialog;
using System.Windows;
using System.Windows.Controls;

namespace CROFFLE_WPF.WPF_xamls.Pages
{
    /// <summary>
    /// AlarmPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmPage : SettingPages
    {

        Settings setting;

        private bool isChanged = false;

        internal bool IsChanged { get { return isChanged; } }

        internal AlarmPage(ref Settings setting)
        {
            InitializeComponent();

            this.setting = setting;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            setting.LoadSetting();
            sw_alarm_on.state = setting.alarm;
        } // Page_Loaded

        public override void Save()
        {
            setting.alarm = sw_alarm_on.state;
            setting.SaveSetting();

            isChanged = false;
        } // Save

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sw_alarm_on.state != setting.alarm)
            {
                isChanged = true;
            }
            if (isChanged)
            {
                if (new OkCancel_DIalog("주의", "변경사항이 저장되지 않았습니다.\n변경사항을 저장하시겠습니까?").ShowDialog() == true)
                {
                    Save();
                }
            }
        } // Page_Unloaded
    } // class
} // namespace
