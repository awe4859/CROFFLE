using CROFFLE_WPF.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CROFFLE_WPF.WPF_xamls.Pages;
using CROFFLE_WPF.WPF_xamls.Dialog;

namespace CROFFLE_WPF.WPF_xamls.Windows
{
    /// <summary>
    /// Preferences.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Preferences : Window
    {
        public static RoutedEvent SettingChanged = EventManager.RegisterRoutedEvent(
            "SettingChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Preferences));
        public event RoutedEventHandler SettingChange
        {
            add { AddHandler(SettingChanged, value); }
            remove { RemoveHandler(SettingChanged, value); }
        }

        private Settings setting;

        //private GeneralPage generalPage;
        //private AlarmPage alarmPage;
        //private LoginPage wafflePage;
        //private InfoPage infoPage;
        private SettingPages settingPages;
        
        // 탭메뉴 색상값
        private SolidColorBrush activateColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3CB1FF"));
        private SolidColorBrush activateText = Brushes.White;
        private SolidColorBrush standardColor = Brushes.LightGray;
        private SolidColorBrush standardText = Brushes.Black;


        private bool isChanged = false;

        internal Preferences(Settings settings)
        {
            InitializeComponent();

            setting = settings;

            setting.LoadAccount();
            setting.LoadSetting();

            //generalPage = new GeneralPage(ref setting);
            //alarmPage = new AlarmPage(ref setting);
            //infoPage = new InfoPage();
            //mainFrame.Content = generalPage;
            settingPages = new GeneralPage(ref setting);
            mainFrame.Content = settingPages;
        }


        // 저장 유무 확인
        internal void CheckSave()
        {
            if (settingPages.GetType() == typeof(LoginPage) || settingPages.GetType() == typeof(InfoPage))
            {
                return;
            }
            var dialog = new OkCancel_DIalog("알림", "저장하시겠습니까?");
            dialog.Owner = this;

            //세팅 저장
            if (dialog.ShowDialog() == true)
            {
                settingPages.Save();
                new ConfirmDialog("알림", "저장되었습니다.") { Owner = this }.ShowDialog();
            }
        }

        // 클릭 이벤트

        private void General_btn_Click(object sender, RoutedEventArgs e)
        {
            settingPages = new GeneralPage(ref setting);
            mainFrame.Content = settingPages;
            StandardButton();
            HighLightButton(general_bd, general_lb);
        }

        private void Alarm_btn_Click(object sender, RoutedEventArgs e)
        {
            settingPages = new AlarmPage(ref setting);
            mainFrame.Content = settingPages;
            StandardButton();
            HighLightButton(alarm_bd, alarm_lb);
        }

        private void Waffle_btn_Click(object sender, RoutedEventArgs e)
        {
            Check_Login();
            mainFrame.Content = settingPages;
            StandardButton();
            HighLightButton(waffle_bd,waffle_lb);
        }

        private void Info_btn_Click(object sender, RoutedEventArgs e)
        {
            //test
            settingPages = new InfoPage();
            mainFrame.Content = settingPages;
            StandardButton();
            HighLightButton(info_bd, info_lb);
        }

        // 확인 취소 버튼

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = null;
            Close();
        }

        private void Check_Login()
        {
            if (setting.logged_in)
            {
                settingPages = new WaffleUserPage(setting);
            }
            else
            {
                settingPages = new WaffleLoginPage(ref setting);
            }
            ((LoginPage)settingPages).LoggedChange += WafflePage_LoggedChange;
        }

        private void WafflePage_LoggedChange(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            RaiseEvent(new RoutedEventArgs(SettingChanged));
            Close();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            // 창 닫기
            this.Close();
        }

        // 탭 버튼 관련
        private void HighLightButton(Border border, Label label)
        {
            border.Background = activateColor;
            label.Foreground = activateText;
        }

        private void StandardButton()
        {
            general_bd.Background = standardColor;
            general_lb.Foreground = standardText;
            alarm_bd.Background = standardColor;
            alarm_lb.Foreground = standardText;
            waffle_bd.Background = standardColor;
            waffle_lb.Foreground = standardText;
            info_bd.Background = standardColor;
            info_lb.Foreground = standardText;
        }

         /*test*/
        private void Window_Closed(object sender, EventArgs e)
        {
            Visibility = Visibility.Collapsed;
            RaiseEvent(new RoutedEventArgs(SettingChanged));
            Close();
        }


        #region TitleBar
        private void MouseDragTitle(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        } // MouseDragTitle

        private void MouseEnter_Close(object sender, MouseEventArgs e)
        {
            bd_closeBtn.Background = Brushes.Red;
        } // MouseEnter_Close

        private void MouseLeave_Close(object sender, MouseEventArgs e)
        {
            bd_closeBtn.Background = Brushes.Transparent;
        } // MouseLeave_Close

        private void MouseClick_Close(object sender, RoutedEventArgs e)
        {
            Close();
        } // MouseClick_Close
        #endregion

        #region Footer
        private void MouseClick_Save(object sender, RoutedEventArgs e)
        {
            settingPages.Save();
        }
        #endregion
    }
}