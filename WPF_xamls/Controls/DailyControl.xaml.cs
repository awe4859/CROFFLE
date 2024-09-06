using AnniversaryAPI;
using CROFFLE_WPF.WPF_xamls.Editor;
using CROFFLE_WPF.WPF_xamls.Windows;
using CroffleDataManager.SQLiteDB;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CROFFLE_WPF.WPF_xamls.Controls
{
    /// <summary>
    /// DailyControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DailyControl : UserControl
    {
        public static RoutedEvent DailyControlRefresh = EventManager.RegisterRoutedEvent(
            "DailyControlRefresh", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DailyControl));
        public event RoutedEventHandler DailyControlRefreshEvent
        {
            add { AddHandler(DailyControlRefresh, value); }
            remove { RemoveHandler(DailyControlRefresh, value); }
        }

        private string apikey;
        private DateTime date_value;
        internal DateTime Day { get { return date_value; } }

        private int done;


        public DailyControl(DateTime date, bool done)
        {
            InitializeComponent();
            date_value = date;
            if (done) this.done = -1;
            else this.done = 0;
            if (IsToday())
            {
                bd_DailyControl.Background = Brushes.PaleGreen;
            }
            dayNum_lb.Content = date_value.ToString("dd"); // 날짜 설정
            Set_Annv();

        }

        public DailyControl(DateTime date, bool done, string apikey)
        {
            InitializeComponent();
            date_value = date;
            this.apikey = apikey;
            if (done) this.done = -1;
            else this.done = 0;
            if (IsToday())
            {
                bd_DailyControl.Background = Brushes.PaleGreen;
            }
            dayNum_lb.Content = date_value.ToString("dd"); // 날짜 설정
            Set_Annv();

            UpdateInfo();
        }

        private void DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DailyInfo dailyInfo = new DailyInfo(date_value, apikey);
            dailyInfo.Owner = Application.Current.MainWindow;
            dailyInfo.UpdateDailyInfoEvent += new RoutedEventHandler(RefreshAll);
            dailyInfo.Show();
        }

        private void OpenInfo(object sender, RoutedEventArgs e)
        {
            DailyInfo dailyInfo = new DailyInfo(date_value, apikey);
            dailyInfo.Owner = Application.Current.MainWindow;
            dailyInfo.UpdateDailyInfoEvent += new RoutedEventHandler(RefreshAll);
            dailyInfo.Show();
        }

        private void RefreshAll(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(DailyControlRefresh));
        }

        private void AddContents_Btn(object sender, RoutedEventArgs e)
        {
            var btn = sender as MenuItem;
            if (btn == null) return;

            EditorWindow editor = null;
            if (btn == cm_addMemo) editor = new MemoEditor(date_value);
            if (btn == cm_addTask) editor = new TaskEditor(date_value);
            if (btn == cm_addSchedule) editor = new ScheduleEditor(date_value);

            if (editor == null) { Console.WriteLine("[DailyInfo] Error: Failed to create EditorWindow from AddContents ContextMenu."); return; }

            editor.Owner = Application.Current.MainWindow;
            editor.AskUpdateEvent += new RoutedEventHandler(RefreshAll);
            editor.ShowDialog();
        } // AddContents_Btn

        private void Set_Annv()
        {
            AnnvAPI annvAPI = new AnnvAPI(apikey);
            if(date_value.Day == 1) annvAPI.CheckAnniversaryMonth(date_value.Year, date_value.Month);
            annvAPI.GetDailyInfo(date_value, out var table);
            dailyInfo_lb.Content = "";
            foreach (DataRow row in table.Rows)
            {
                dailyInfo_lb.Content = row["dateName"].ToString();
                bool.TryParse(row["isHoliday"].ToString(), out var holiday);
                if (holiday) SetHoliday();
            }
        }

        public void UpdateInfo()
        {
            SQLiteDB db = new SQLiteDB();
            db.GetDailyProperty(date_value, done, -1, out var contents);
            var count = contents[1].Rows.Count;

            if (count == 0) { for (var i = 0; i < 3; i++) SetContentsInvisible(i); return; }
            if (count > 3)
            {
                content3.Visibility = Visibility.Visible;
                content3_lb.Content = $@"외 {count - 2}개의 일정 있음";
                count = 2;
            }
            for (var i = 0; i < 3; i++)
            {
                if (i < count){
                var title = contents[1].Rows[i]["title"].ToString();
                int.TryParse(contents[1].Rows[i]["color"].ToString(), out var color);
                    SetContents(i, title, color);
                }
                else
                {
                    SetContentsInvisible(i);
                }
            }
        }


        public void SetContents(int index, string title, int color_argb)
        {
            if (index == 0) {
                content1.Visibility = Visibility.Visible;
                if (title.Length > 15) content1_lb.Content = title.Substring(0, 15) + "...";
                else content1_lb.Content = title;
                content1_bd.Background = new SolidColorBrush(Color.FromArgb((byte)(color_argb >> 24), (byte)(color_argb >> 16), (byte)(color_argb >> 8), (byte)color_argb));
            }
            else if (index == 1)
            {
                content2.Visibility = Visibility.Visible;
                if (title.Length > 15) content2_lb.Content = title.Substring(0, 15) + "...";
                else content2_lb.Content = title;
                content2_bd.Background = new SolidColorBrush(Color.FromArgb((byte)(color_argb >> 24), (byte)(color_argb >> 16), (byte)(color_argb >> 8), (byte)color_argb));
            }
            else if (index == 2)
            {
                content3.Visibility = Visibility.Visible;
                if (title.Length > 15) content3_lb.Content = title.Substring(0, 15) + "...";
                else content3_lb.Content = title;
                content3_bd.Background = new SolidColorBrush(Color.FromArgb((byte)(color_argb >> 24), (byte)(color_argb >> 16), (byte)(color_argb >> 8), (byte)color_argb));
            }
            else new IndexOutOfRangeException("DailControl Out of Range Error");
         }

        private void SetContentsInvisible(int index)
        {
            Grid grid = null;
            if (index == 0) grid = content1;
            else if (index == 1) grid = content2;
            else if (index == 2) grid = content3;
            else new IndexOutOfRangeException("DailControl Out of Range Error");

            grid.Visibility = Visibility.Collapsed;
        }
        internal void ChangeCornerRadius(int TopLeft, int TopRight, int BottomRight, int BttomLeft)
        {

            LabelsBox_Border.CornerRadius = new CornerRadius(TopLeft, TopRight, 0, 0);
            ContentsBox_Border.CornerRadius = new CornerRadius(0, 0, BottomRight, BttomLeft);
        }
        public bool IsToday()
        {
            var result = date_value.Date == DateTime.Today;
            return result;
        }
        public void SetHoliday()
        {
            dailyInfo_lb.Foreground = Brushes.Red;
            dayNum_lb.Foreground = Brushes.Red;
        }
        public void SetForeground(Color color)
        {
            dailyInfo_lb.Foreground = new SolidColorBrush(color);
            dayNum_lb.Foreground = new SolidColorBrush(color);
        }
    }
}
