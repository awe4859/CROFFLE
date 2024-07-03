using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Croffle.Classes;
using System.Windows.Forms;
using System.Drawing;
using UserControl = System.Windows.Controls.UserControl; // Windows.Forms의 UserControl과 겹쳐서 따로 선언(?)
using System.IO;

namespace CROFFLE_WPF.WPF_xamls
{
    /// <summary>
    /// GeneralPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GeneralPage : Page
    {
        private NotifyIcon notifyIcon = null;

        private Settings _setting;

        private bool systray_value = false;
        private bool auto_start_value = false;
        private bool show_week_value = false;
        private bool show_cancel_value = false;
        private bool show_done_value = false;
        private bool done_doubleC_value = false;
        private WindowState WindowState;

        internal GeneralPage(ref Settings setting)
        {
            InitializeComponent();
            _setting = setting;
            SetupNotifyIcon();
        }

        #region notify event

        private void SetupNotifyIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new Icon("./Icon/croffle.ico");
            notifyIcon.Visible = true;
            notifyIcon.Text = "Croffle";

            var contextMenu = new ContextMenuStrip();
            var exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += ExitMenuItem_Click;
            contextMenu.Items.Add(exitMenuItem);

            notifyIcon.ContextMenuStrip = contextMenu;

            // Add Click event handler
            notifyIcon.Click += NotifyIcon_Click;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            // Handle the click event
            this.WindowState = WindowState.Normal;
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                System.Windows.Application.Current.Shutdown();
            });
        }

        /*internal void NotifyClose()
        {
            notifyIcon.Dispose();
        }*/
        #endregion

        #region 초기값 load
        private void GeneralPage_Load(object sender, RoutedEventArgs e)
        {
            // load된 값을 받아옴
            auto_start_value = _setting.auto_start;
            systray_value = _setting.system_tray;
            show_week_value = _setting.show_week;
            show_cancel_value = _setting.show_cancel;
            show_done_value = _setting.show_done;
            done_doubleC_value = _setting.done_doubleC;

            Systray_Change(_setting.system_tray);
            AutoStart_Change(_setting.auto_start);
            Show_Week_Change(_setting.show_week);
            Show_Cancel_Change(_setting.show_cancel);
            Show_Done_Change(_setting.show_done);
            Done_DoubleC_Change(_setting.done_doubleC);
        }

        /*load시 저장된 설정값에 맞춰 온오프 스위치 값을 바꿔줌*/
        private void Systray_Change(bool value)
        {
            if (value) { system_tray_switch.Change(); notifyIcon.Visible = true; }
            else notifyIcon.Visible = false;
        }

        private void AutoStart_Change(bool value)
        {
            if (value) { auto_start_switch.Change(); }
        }

        private void Show_Week_Change(bool value)
        {
            if (value) { show_week_switch.Change(); }
        }

        private void Show_Cancel_Change(bool value)
        {
            if (value) { show_cancel_switch.Change(); }
        }

        private void Show_Done_Change(bool value)
        {
            if (value) { show_done_switch.Change(); }
        }

        private void Done_DoubleC_Change(bool value)
        {
            if (value) { done_doubleC_switch.Change(); }
        }

        #endregion


        internal void Save()
        {
            _setting.system_tray = systray_value;
            _setting.auto_start = auto_start_value;
            _setting.show_week = show_week_value;
            _setting.show_cancel = show_cancel_value;
            _setting.show_done = show_done_value;
            _setting.done_doubleC = done_doubleC_value;

            _setting.SaveSetting();
        }


        // 값 변경 여부 확인
        // 값이 바뀌면 true 반환
        internal bool Changed()
        {
            if (_setting.auto_start != auto_start_value) return true;
            else if (_setting.system_tray != systray_value) return true;
            else if (_setting.show_week != show_week_value) return true;
            else if (_setting.show_cancel != show_cancel_value) return true;
            else if (_setting.show_done != show_done_value) return true;
            else if (_setting.done_doubleC != done_doubleC_value) return true;
            else return false;
        }

        /* 클릭 이벤트 */
        private void System_tray_click(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as UserControl;
            if (system_tray_switch.state)
            {
                systray_value = true;
                notifyIcon.Visible = true;
            }
            else
            {
                systray_value = false;
                notifyIcon.Visible = false;
            }
        }

        private void Auto_Start_Click(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as UserControl;
            if (auto_start_switch.state) auto_start_value = true;
            else auto_start_value = false;
        }

        private void Show_Done_Click(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as UserControl;
            if (show_done_switch.state) show_done_value = true;
            else show_done_value = false;
        }

        private void Show_Cancel_Click(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as UserControl;
            if (show_cancel_switch.state) show_cancel_value = true;
            else show_cancel_value = false;
        }

        private void Show_Week_Click(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as UserControl;
            if (show_week_switch.state) show_week_value = true;
            else show_week_value = false;
        }

        private void Done_DoubleC_Click(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as UserControl;
            if (done_doubleC_switch.state) done_doubleC_value = true;
            else done_doubleC_value = false;
        }

       /*저장안하고 메뉴 탭 변경 시 저장 여부를 체크*/
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Changed())
            {
                if (System.Windows.MessageBox.Show("저장되지 않았습니다. 저장 하시겠습니까?", "알림", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    Save();
                }
            }
        }

    }
}
