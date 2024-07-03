using Croffle.Classes;
using Croffle.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace CROFFLE_WPF.WPF_xamls
{
    /// <summary>
    /// Preferences.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Preferences : Window
    {
        private Settings setting;

        private GeneralPage generalPage;
        private AlarmPage alarmPage;
        private WaffleLoginPage waffleLoginPage;
        private InfoPage infoPage;
        
        // 탭메뉴 색상값
        private SolidColorBrush activateColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3CB1FF"));
        private SolidColorBrush activateText = Brushes.White;
        private SolidColorBrush standardColor = Brushes.LightGray;
        private SolidColorBrush standardText = Brushes.Black;

        public Preferences()
        {
            InitializeComponent();

            setting = new Settings();

            setting.LoadAccount();
            setting.LoadSetting();

            generalPage = new GeneralPage(ref setting);
            alarmPage = new AlarmPage();
            waffleLoginPage = new WaffleLoginPage();
            infoPage = new InfoPage();
            mainFrame.Content = generalPage;

            this.MouseLeftButtonDown += Preferences_MouseLeftButton;
        }


        // 저장 유무 확인
        internal void CheckSave()
        {
            var answer = MessageBox.Show("저장 하시겠습니까?", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            //세팅 저장
            if (answer == MessageBoxResult.OK)
            {
                generalPage.Save();
                MessageBox.Show("저장되었습니다.");
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
            mainFrame.Content = waffleLoginPage;
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
                if (System.Windows.MessageBox.Show("저장되지 않았습니다. 저장 하시겠습니까?", "알림", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    generalPage.Save();
                    this.Close();
                }
                else this.Close();
            }
            else this.Close();
        }

        private void Save_Setting_Click(object sender, RoutedEventArgs e)
        {
            CheckSave();
        }


        // 창 관련 이벤트
        void Preferences_MouseLeftButton(Object sender, MouseButtonEventArgs e)
        {
            //창 이동
            this.DragMove();
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

    }
}