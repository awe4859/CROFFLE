using CROFFLE_WPF.Classes;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CROFFLE_WPF.WPF_xamls.Windows
{
    /// <summary>
    /// AlarmWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmWindow : Window
    {
        Alarm alarm;
        DispatcherTimer timer;

        public AlarmWindow(string contentsID, int min)
        {
            alarm = new Alarm();
            InitializeComponent();

            Top = SystemParameters.WorkArea.Height;
            Left = SystemParameters.WorkArea.Width - Width;


            alarm.LoadData(contentsID);
            lb_title.Content = alarm.TITLE;
            var text = $@"시작까지 {min}분 전";
            lb_detail.Content = text;

            if (alarm.TYPE != EContents.eTask) grid_t_Icon.Visibility = Visibility.Collapsed;
            if (alarm.TYPE != EContents.eSchedule) grid_s_Icon.Visibility = Visibility.Collapsed;
            if (alarm.TYPE != EContents.eWaffle) grid_w_Icon.Visibility = Visibility.Collapsed;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = Top;
            da.To = SystemParameters.WorkArea.Height - Height;
            da.Duration = TimeSpan.FromSeconds(0.5);
            BeginAnimation(TopProperty, da);
            timer.Start();
        }

        private void Btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_close_MouseEnter(object sender, MouseEventArgs e)
        {
            bd_close.Background = Brushes.Red;
        }

        private void Btn_close_MouseLeave(object sender, MouseEventArgs e)
        {
            bd_close.Background = Brushes.Transparent;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine($@"[AlarmWindow] Timer: {timer.Interval} ms Done");
            ((DispatcherTimer)sender).Stop();

            DoubleAnimation da = new DoubleAnimation();
            da.From = Top;
            da.To = SystemParameters.WorkArea.Height;
            da.Duration = TimeSpan.FromSeconds(0.5);
            BeginAnimation(TopProperty, da);

            var close_timer = new DispatcherTimer();
            close_timer.Interval = TimeSpan.FromSeconds(1);
            close_timer.Tick += Timer_Close;
            close_timer.Start();
        }

        private void Timer_Close(object sender, EventArgs e)
        {
            Console.WriteLine($@"[AlarmWindow] Timer: {timer.Interval} ms Done");
            ((DispatcherTimer)sender).Stop();
            Close();
        }
    }
}
