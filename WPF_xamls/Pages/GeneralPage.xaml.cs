using System.Windows;
using System.Windows.Controls;
using CROFFLE_WPF.Classes;
using CROFFLE_WPF.WPF_xamls.Dialog;

namespace CROFFLE_WPF.WPF_xamls.Pages
{
    /// <summary>
    /// GeneralPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GeneralPage : SettingPages
    {
        public static RoutedEvent SettingGeneralChangedEvent = EventManager.RegisterRoutedEvent(
            "SettingGeneralChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GeneralPage));
        public event RoutedEventHandler SettingGeneralChanged
            {
            add { AddHandler(SettingGeneralChangedEvent, value); }
            remove { RemoveHandler(SettingGeneralChangedEvent, value); }
        }

        private Settings _setting;

        private bool _isChanged = false;

        internal GeneralPage(ref Settings setting)
        {
            InitializeComponent();
            _setting = setting;
        } // GeneralPage


        private void GeneralPage_Load(object sender, RoutedEventArgs e)
        {
            // load된 값을 받아옴
            auto_start_switch.state = _setting.auto_start;
            system_tray_switch.state = _setting.system_tray;
            show_cancel_switch.state = _setting.show_cancel;
            show_done_switch.state = _setting.show_done;
            show_week_switch.state = _setting.show_week;
            done_doubleC_switch.state = _setting.done_doubleC;
        } // GeneralPage_Load

        public override void Save()
        {
            _setting.auto_start = auto_start_switch.state;
            _setting.system_tray = system_tray_switch.state;
            _setting.show_cancel = show_cancel_switch.state;
            _setting.show_done = show_done_switch.state;
            _setting.show_week = show_week_switch.state;
            _setting.done_doubleC = done_doubleC_switch.state;

            _setting.SaveSetting();

            _isChanged = false;
        } // Save

       /*저장안하고 메뉴 탭 변경 시 저장 여부를 체크*/
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_isChanged)
            {
                if (new OkCancel_DIalog("주의", "저장하지 않았습니다.\n저장하시겠습니까?").ShowDialog() == true)
                {
                    Save();
                }
            }
        } // Page_Unloaded
    } // GeneralPage
} // namespace
