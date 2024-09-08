using AnniversaryAPI;
using CROFFLE_WPF.WPF_xamls.Controls;
using CroffleDataManager.SQLiteDB;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using CROFFLE_WPF.Classes;
using CROFFLE_WPF.WPF_xamls.Editor;
using System.Runtime.CompilerServices;

namespace CROFFLE_WPF.WPF_xamls.Windows
{
    /// <summary>
    /// DailyInfo.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DailyInfo : Window
    {
        public static RoutedEvent UpdateDailyInfo = EventManager.RegisterRoutedEvent(
            "UpdateDailyInfo", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DailyInfo));
        public event RoutedEventHandler UpdateDailyInfoEvent
        {
            add { AddHandler(UpdateDailyInfo, value); }
            remove { RemoveHandler(UpdateDailyInfo, value); }
        }

        private DateTime date;
        private DataTable memo;
        private DataTable component;

        private string dayOfWeek;
        private string apikey;

        private bool onTask = true;
        private bool onSchedule = true;
        private bool onWaffle = true;

        private Brush bSchedule;
        private Brush bTask;
        private Brush bWaffle;

        public DailyInfo(DateTime date)
        {
            InitializeComponent();
            this.date = date;
            SetDayofWeek();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            bSchedule = bd_onSchedule.Background;
            bTask = bd_onTask.Background;
            bWaffle = bd_onWaffle.Background;
        }

        public DailyInfo(DateTime date, string apikey)
        {
            InitializeComponent();
            this.date = date;
            this.apikey = apikey;
            SetDayofWeek();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            bSchedule = bd_onSchedule.Background;
            bTask = bd_onTask.Background;
            bWaffle = bd_onWaffle.Background;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lb_title.Content = date.ToString($@"yyyy년 MM월 dd일 ({dayOfWeek})");

            AnnvAPI annv = new AnnvAPI(apikey);
            annv.GetDailyInfo(date, out var dailyInfo);
            if (dailyInfo.Rows.Count == 0)
            {
                annv.SetEveryAnniversaryOnDB(date.Year, date.Month);
                annv.GetDailyInfo(date, out dailyInfo);
            }
            foreach (DataRow info in dailyInfo.Rows)
            {
                //Label Name="lb_annv" Foreground="Black" FontSize="20" FontWeight="Bold" FontFamily="../../Font/#Dongle" Padding="0"
                var label = new Label();

                label.Content = info["dateName"].ToString();
                label.FontSize = 20;
                label.FontWeight = FontWeights.Bold;
                label.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Font/#Dongle");
                label.Padding = new Thickness(0);
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignment = VerticalAlignment.Center;

                bool.TryParse(info["isHoliday"].ToString(), out var isHoliday);

                if (isHoliday) { label.Foreground = Brushes.Red; }
                else label.Foreground = Brushes.Black;

                sp_annv.Children.Add(label);
            }
            //lb_annv.Content = label;

            UpdateInfo();
        }
        private void OnOff_Contents_Btn(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            if (btn == btn_onSchedule)
            {
                if (onSchedule) { onSchedule = false; bd_onSchedule.Background = Brushes.DimGray; }
                else { onSchedule = true; bd_onSchedule.Background = bSchedule; }
            }
            else if (btn == btn_onTask)
            {
                if (onTask) { onTask = false; bd_onTask.Background = Brushes.DimGray; }
                else { onTask = true; bd_onTask.Background = bTask; }
            }
            else if (btn == btn_onWaffle)
            {
                if (onWaffle) { onWaffle = false; bd_onWaffle.Background = Brushes.DimGray; }
                else { onWaffle = true; bd_onWaffle.Background = bWaffle; }
            }
            LoadProperty();
        } // OnOff_Contents_Btn

        private void AddContents_Btn(object sender, RoutedEventArgs e)
        {
            var btn = sender as MenuItem;
            if (btn == null) return;

            EditorWindow editor = null;
            if (btn == cm_addMemo) editor = new MemoEditor(date);
            if (btn == cm_addTask) editor = new TaskEditor(date);
            if (btn == cm_addSchedule) editor = new ScheduleEditor(date);

            if (editor == null) { Console.WriteLine("[DailyInfo] Error: Failed to create EditorWindow from AddContents ContextMenu."); return; }

            editor.Owner = this;
            editor.AskUpdateEvent += new RoutedEventHandler(Refresh_Btn);
            editor.ShowDialog();
        } // AddContents_Btn

        private void DailyInfoControl_Delete(object sender, RoutedEventArgs e)
        {
            var dic = sender as DailyInfoControl;
            if (dic == null) return;

            Update();
        } // DailyInfoControl_Delete

        private void Refresh_Btn(object sender, RoutedEventArgs e)
        {
            Update();
        } // Refresh_Btn

        public void Update()
        {
            UpdateInfo();
            RaiseEvent(new RoutedEventArgs(UpdateDailyInfo));
        }

        private void UpdateInfo()
        {
            var db = new SQLiteDB();
            db.GetDailyProperty(date, -1, -1, out var contents);
            memo = contents[0];
            component = contents[1];

            LoadProperty();
        }

        private void LoadProperty()
        {
            Complete_sp.Children.Clear();
            Incomplete_sp.Children.Clear();

            foreach (DataRow r in memo.Rows)
            {
                int.TryParse(r["color"].ToString(), out var color);
                var DailyMemo = new DailyInfoControl(r["contentsID"].ToString(), r["title"].ToString(), date, color);
                DailyMemo.IsUpdatedEvent += new RoutedEventHandler(Refresh_Btn);
                DailyMemo.AskCloseEvent += new RoutedEventHandler(MouseClick_Close);
                Complete_sp.Children.Add(DailyMemo);
            }

            foreach (DataRow r in component.Rows)
            {
                int.TryParse(r["color"].ToString(), out var color);
                var type = Contents.Type(r["contentsID"].ToString());
                if (type == EContents.eSchedule && !onSchedule) continue;
                if (type == EContents.eTask && !onTask) continue;
                if (type == EContents.eWaffle && !onWaffle) continue;

                DateTime.TryParse(r["ctime"].ToString(), out var ctime);
                bool.TryParse(r["alarm"].ToString(), out var alarm);
                var DailyComponent = new DailyInfoControl(r["contentsID"].ToString(), r["title"].ToString(), ctime, color);

                bool.TryParse(r["done"].ToString(), out var result);
                DailyComponent.IsDone = result;
                bool.TryParse(r["alarm"].ToString(), out result);
                DailyComponent.IsAlarm = result;

                DailyComponent.IsUpdatedEvent += new RoutedEventHandler(Refresh_Btn);
                DailyComponent.AskCloseEvent += new RoutedEventHandler(MouseClick_Close);
                if (DailyComponent.IsDone) Complete_sp.Children.Add(DailyComponent);
                else Incomplete_sp.Children.Add(DailyComponent);
            }
        }

        private void SetDayofWeek()
        {
            if (date.DayOfWeek == DayOfWeek.Monday) dayOfWeek = "월";
            else if (date.DayOfWeek == DayOfWeek.Tuesday) dayOfWeek = "화";
            else if (date.DayOfWeek == DayOfWeek.Wednesday) dayOfWeek = "수";
            else if (date.DayOfWeek == DayOfWeek.Thursday) dayOfWeek = "목";
            else if (date.DayOfWeek == DayOfWeek.Friday) dayOfWeek = "금";
            else if (date.DayOfWeek == DayOfWeek.Saturday) dayOfWeek = "토";
            else if (date.DayOfWeek == DayOfWeek.Sunday) dayOfWeek = "일";
        }

        #region TitleBar
        private void MouseDragTitle(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 2)
                {
                    if (WindowState == WindowState.Normal)
                        WindowState = WindowState.Maximized;
                    else
                        WindowState = WindowState.Normal;
                }
                else if (WindowState == WindowState.Maximized)
                {
                    Point p = e.GetPosition(this);
                    if (p.X < SystemParameters.PrimaryScreenWidth / 3)
                    {
                        WindowState = WindowState.Normal;
                        Left = 0;
                        Top = p.Y - 10;
                    }
                    else if (p.X > SystemParameters.PrimaryScreenWidth / 3 * 2)
                    {
                        WindowState = WindowState.Normal;
                        Left = SystemParameters.PrimaryScreenWidth - Width;
                        Top = p.Y - 10;
                    }
                    else
                    {
                        WindowState = WindowState.Normal;
                        Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
                        Top = p.Y - 10;
                    }
                }
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
    }
}
