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

        private GeneralPage generalPage;
        private AlarmPage alarmPage;
        private LoginPage wafflePage;
        private InfoPage infoPage;
        
        // 탭메뉴 색상값
        private SolidColorBrush activateColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3CB1FF"));
        private SolidColorBrush activateText = Brushes.White;
        private SolidColorBrush standardColor = Brushes.LightGray;
        private SolidColorBrush standardText = Brushes.Black;

        internal Preferences(Settings settings)
        {
            InitializeComponent();

            setting = settings;

            setting.LoadAccount();
            setting.LoadSetting();

            generalPage = new GeneralPage(ref setting);
            alarmPage = new AlarmPage(ref setting);
            infoPage = new InfoPage();
            mainFrame.Content = generalPage;
            Check_Login();
        }


        // 저장 유무 확인
        internal void CheckSave()
        {
            var dialog = new OkCancel_DIalog("알림", "저장하시겠습니까?");
            dialog.Owner = this;

            //세팅 저장
            if (dialog.ShowDialog() == true)
            {
                generalPage.Save();
                new ConfirmDialog("알림", "저장되었습니다.") { Owner = this }.ShowDialog();
            }
        }


        // 클릭 이벤트

        private void General_btn_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = generalPage = new GeneralPage(ref setting);
            StandardButton();
            HighLightButton(general_bd, general_lb);
        }


        private void Alarm_btn_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = alarmPage;
            StandardButton();
            HighLightButton(alarm_bd, alarm_lb);
        }

        private void Waffle_btn_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = wafflePage;
            StandardButton();
            HighLightButton(waffle_bd,waffle_lb);
        }

        private void Info_btn_Click(object sender, RoutedEventArgs e)
        {
            //test
            mainFrame.Content = infoPage;
            StandardButton();
            HighLightButton(info_bd, info_lb);
        }

        // 확인 취소 버튼

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            if (generalPage.Changed())
            {
                if (MessageBox.Show("저장되지 않았습니다. 저장 하시겠습니까?", "알림", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    generalPage.Save();
                    this.Close();
                }
                else this.Close();
            }
            else this.Close();
        }

        private void Check_Login()
        {
            if (setting.logged_in)
            {
                wafflePage = new WaffleUserPage(setting);
            }
            else
            {
                wafflePage = new WaffleLoginPage(ref setting);
            }
            wafflePage.LoggedChange += WafflePage_LoggedChange;
        }

        private void WafflePage_LoggedChange(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            RaiseEvent(new RoutedEventArgs(SettingChanged));
            Close();
        }

        private void Save_Setting_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            RaiseEvent(new RoutedEventArgs(SettingChanged));
            CheckSave();
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
            /*generalPage.NotifyClose();*/
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
            if (true) return;

            //RaiseEvent(new RoutedEventArgs(AskUpdate));

            //Close();
        } // MouseClick_Save
        #endregion
    }
}