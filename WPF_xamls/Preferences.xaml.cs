using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CROFFLE_WPF.WPF_xamls
{
    /// <summary>
    /// Preferences.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Preferences : Window
    {
        GeneralPage generalPage;
        AlarmPage alarmPage;
        WaffleLoginPage waffleLoginPage;
        InfoPage infoPage;

        private static SolidColorBrush activateColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3CB1FF"));
        private static SolidColorBrush activateText = Brushes.White;
        private static SolidColorBrush standardColor = Brushes.LightGray;
        private static SolidColorBrush standardText = Brushes.Black;

        public Preferences()
        {
            InitializeComponent();
            generalPage = new GeneralPage();
            alarmPage = new AlarmPage();
            waffleLoginPage = new WaffleLoginPage();
            infoPage = new InfoPage();
            mainFrame.Content = generalPage;

            this.MouseLeftButtonDown += Preferences_MouseLeftButton;
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
            /*foreach (var child in Titlebutton.Children)
            {
                if (child is Grid grid)
                {
                    var border = grid.Children.OfType<Border>().FirstOrDefault();
                    var label = grid.Children.OfType<Label>().FirstOrDefault();

                    if (border != null && label != null)
                    {
                        border.Background = standardColor;
                        label.Foreground = standardText;
                    }
                }
            }*/
            general_bd.Background = standardColor;
            general_lb.Foreground = standardText;
            alarm_bd.Background = standardColor;
            alarm_lb.Foreground = standardText;
            waffle_bd.Background = standardColor;
            waffle_lb.Foreground = standardText;
            info_bd.Background = standardColor;
            info_lb.Foreground = standardText;
        }

        // 클릭 이벤트

        private void General_btn_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = generalPage; // generalPage로 전환
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

        private void Ok_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
