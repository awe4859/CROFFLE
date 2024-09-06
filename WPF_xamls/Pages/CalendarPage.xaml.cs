using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CROFFLE_WPF.Classes;
using CROFFLE_WPF.WPF_xamls.Controls;
using CROFFLE_WPF.WPF_xamls.Windows;

namespace CROFFLE_WPF.WPF_xamls.Pages
{
    /// <summary>
    /// Calendar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CalendarPage : Page
    {
        public static RoutedEvent CalendarSoftRefresh = EventManager.RegisterRoutedEvent(
            "CalendarSoftRefresh", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CalendarPage));
        public event RoutedEventHandler CalendarSoftRefreshEvent
        {
            add { AddHandler(CalendarSoftRefresh, value); }
            remove { RemoveHandler(CalendarSoftRefresh, value); }
        }

        public static RoutedEvent CalendarHardRefresh = EventManager.RegisterRoutedEvent(
            "CalendarHardRefresh", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CalendarPage));
        public event RoutedEventHandler CalendarHardRefreshEvent
        {
            add { AddHandler(CalendarHardRefresh, value); }
            remove { RemoveHandler(CalendarHardRefresh, value); }
        }

        // 전역변수
        private int year;
        private int month;
        private int day;

        private string apikey;

        Settings settings;
        WaffleTask waffleTask;

        internal CalendarPage(ref Settings setting)
        {
            InitializeComponent();
            year = DateTime.Now.Year;
            month = DateTime.Now.Month;
            day = DateTime.Now.Day;
            settings = setting;

            Load_Calendar();
        }

        internal CalendarPage(string apikey, ref Settings setting)
        {
            InitializeComponent();
            this.apikey = apikey;
            year = DateTime.Now.Year;
            month = DateTime.Now.Month;
            day = DateTime.Now.Day;
            settings = setting;

            Load_Calendar();
        }

        internal void Load_Calendar()
        {
            todayBtn_lb.Content = day;
            if (settings.show_week) week_lb.Content = GetWeekOfMonth()+"주차";
            else week_lb.Visibility = Visibility.Collapsed;
            // 현재 날짜를 기준으로 연도와 월을 가져와서, 해당 월의 첫 번째 날을 나타내는 DateTime 객체를 생성
            
            DateTime days = new DateTime(year, month, 1);                    
            DateTime date_pointer = days;
            //이전달의 마지막 일요일을 구하고, 달력의 시작 위치를 date_pointer에 저장  
           
            int pre_mon_num = (int)days.DayOfWeek;

            if (pre_mon_num == 0) date_pointer = date_pointer.AddDays(-7);
            else date_pointer = date_pointer.AddDays(0 - pre_mon_num);
            //Console.WriteLine(date_pointer.ToString("yyyy-MM-dd"));
            dateNavLb.Content = year + "년 " + month + "월";

            dailyControlArea.Children.Clear();

            var show_done = settings.show_done;

            int index = 0;
            while (index < 42)
            {
                DailyControl dc = new DailyControl(date_pointer, show_done, apikey);
                dc.Name = $@"dc{date_pointer:yyMMdd}";

                if (dc.Day.Month == days.Month) dc.bd_DailyControl.Background = Brushes.Transparent;
                else dc.bd_DailyControl.Background = Brushes.LightGray;

                if (index == 41) dc.ChangeCornerRadius(0, 0, 10, 0);

                int row = index / 7;
                int col = index % 7 +1;

                if (col == 1) dc.SetHoliday();
                if (col == 7) dc.SetForeground(Colors.RoyalBlue);
                dailyControlArea.Children.Add(dc);
                Grid.SetRow(dc, row);
                Grid.SetColumn(dc, col);

                dc.DailyControlRefreshEvent += Dc_DailyControlRefreshEvent;
                
                index++;
                date_pointer = date_pointer.AddDays(1);
            }
        }

        private void Dc_DailyControlRefreshEvent(object sender, RoutedEventArgs e)
        {
            Load_Calendar();
            RaiseEvent(new RoutedEventArgs(CalendarSoftRefresh));
        }

        private void Dc_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //dailyInfo 띄우기
        {
            var dc = sender as DailyControl;
            if (dc == null) return;

            DailyInfo dailyInfo = new DailyInfo(dc.Day, apikey);
            dailyInfo.ShowDialog();
            //DailyInfo를 ShowDialog형태로 open
        }

        internal static int GetWeekOfMonth()
        //주차 구하기
        {
            DateTime now = DateTime.Today;
            int basisWeekOfDay = (now.Day - 1) % 7;
            int thisWeek = (int)now.DayOfWeek;
            double val = Math.Ceiling((double)now.Day / 7);
            
            if (basisWeekOfDay > thisWeek) 
                val++;
           
            return Convert.ToInt32(val);
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
            // 다음달 버튼
        {
            month++;
            if (month > 12)
            {
                month = 1;
                year++;
            }

            // 캘린더 다시 로드

            Load_Calendar();
        }

        private void prevBtn_Click(object sender, RoutedEventArgs e)

            // 이전달 버튼
        {
            month--;
            if (month < 1)
            {
                month = 12;
                year--;
            }

            // 캘린더 다시 로드
            Load_Calendar();
        }

        private void todayBtn_Click(object sender, RoutedEventArgs e)
            //오늘날로 돌아오기 버튼
        {
            year = DateTime.Now.Year;
            month = DateTime.Now.Month;
            day = DateTime.Now.Day;
            Load_Calendar();
        }

      
    }
}

       
    


