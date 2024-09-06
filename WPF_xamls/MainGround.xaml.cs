using AnniversaryAPI;
using CROFFLE_WPF.Classes;
using CROFFLE_WPF.WPF_xamls.Dialog;
using CROFFLE_WPF.WPF_xamls.Pages;
using CROFFLE_WPF.WPF_xamls.Windows;
using CroffleDataManager.SQLiteDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using WaffleHttp;

namespace CROFFLE_WPF.WPF_xamls
{
    /// <summary>
    /// MainGround.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainGround : Window
    {
        private string content_type = "application/x-www-form-urlencoded; charset=utf-8";
        // API Key by data.go.kr - Korea Astronomy and Space Science Institute (KASI) Special Day Information
        // https://www.data.go.kr/data/15012690/openapi.do
        private string annv_api_key = "";
        int done = (0xFF << 24) | (0x80 << 16) | (0xE1 << 8) | 0x2A;
        int not_done = (0xFF << 24) | (0x00 << 16) | (0xA5 << 8) | 0xFF;
        int expired = (0xFF << 24) | (0xFF << 16) | (0x44 << 8) | 0x19;

        private Waffle waffle;
        private Settings settings;
        private LoadingWindow lw;
        private AlarmManager alarmManager;

        private DispatcherTimer timer;

        NotifyForms notify;
        CalendarPage mainCalendar;

        internal MainGround()
        {
            Visibility = Visibility.Collapsed;
            lw = new LoadingWindow();
            lw.Show();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Hide();
            SetNotify();
            Thread.Sleep(1000);
            CheckComponentDB();

            settings = new Settings();
            settings.UpdatekAutoStart();
            waffle = new Waffle(content_type, done, not_done, expired);
            alarmManager = new AlarmManager();

            LoadFromWaffle();
            Thread.Sleep(1000);

            LoadCalendar();
            Thread.Sleep(1000);

            alarmManager.Update();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(30);
            timer.Tick += Timer_Update;
            timer.Start();

            lw.Close();
            Show();
        }

        private void LoadCalendar()
        {
            mainCalendar = new CalendarPage(annv_api_key, ref settings);
            mainCalendar.CalendarSoftRefreshEvent += SoftUpdateAksed;
            pageFrame.Content = mainCalendar;
        }

        private void quitBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OkCancel_DIalog("주의", "알림을 받을 수 없습니다.\n프로그램을 종료하시겠습니까?");
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                notify.Dispose();
                notify.Close();
                Close();
            }
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            var p = new Preferences(settings);
            p.Owner = this;
            p.SettingChange += P_SettingChange;
            p.ShowDialog();
        }

        private void P_SettingChange(object sender, RoutedEventArgs e)
        {
            CheckComponentDB();

            settings = new Settings();
            settings.UpdatekAutoStart();
            waffle = new Waffle(content_type, done, not_done, expired);

            LoadFromWaffle();
            Thread.Sleep(1000);

            LoadCalendar();
            Thread.Sleep(1000);

            alarmManager.Update();

            timer.Stop();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(30);
            timer.Tick += Timer_Update;
            timer.Start();

            lw.Close();
            Show();
        }

        private void SoftUpdateAksed(object sender, RoutedEventArgs e)
        {
            timer.Stop();

            LoadFromWaffle();
            Thread.Sleep(1000);

            alarmManager.Update();

            timer.Stop();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(30);
            timer.Tick += Timer_Update;
            timer.Start();

            lw.Close();
            Show();
        }

        private void MouseEnter_Hyperlink(object sender, MouseEventArgs e)
        {
            var lb = sender as Grid;
            if (lb == null) return;

            var color = Color.FromArgb(0xFF, 0x94, 0xD5, 0xF8);
            if (lb == grid_WkuBtn) bd_wkuBtn.Background = new SolidColorBrush(color);
            if (lb == grid_IntraBtn) bd_IntraBtn.Background = new SolidColorBrush(color);
            if (lb == grid_WaffleBtn) bd_WaffleBtn.Background = new SolidColorBrush(color);
        }

        private void MouseLeave_Hyperlink(object sender, MouseEventArgs e)
        {
            var lb = sender as Grid;
            if (lb == null) return;

            var color = Color.FromArgb(0xFF, 0x08, 0x81, 0xF5);

            if (lb == grid_WkuBtn) bd_wkuBtn.Background = new SolidColorBrush(color);
            if (lb == grid_IntraBtn) bd_IntraBtn.Background = new SolidColorBrush(color);
            if (lb == grid_WaffleBtn) bd_WaffleBtn.Background = new SolidColorBrush(color);
        }

        private void RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }

        private void SetNotify()
        {
            notify = new NotifyForms();
            notify.AskOpen += NotifyAskOpen;
            notify.AskExit += quitBtn_Click;
        }

        private void NotifyAskOpen(object sender, RoutedEventArgs e)
        {
            LoadCalendar();
            Show();
            Visibility = Visibility.Visible;
            WindowState = WindowState.Normal;
        }

        internal void LoadFromWaffle()
        {
            if (!settings.logged_in)
            {
                var dialog = new ConfirmDialog("주의", "로그인 정보가 없습니다.\n로그인 정보를 입력해주세요.");
                dialog.Owner = this;
                dialog.ShowDialog();
                return;
            }
            settings.GetAccount(out string id, out string pw);
            if (id == "" || pw == "")
            {
                settings.logged_in = false;
                settings.SaveSetting();
                var dialog = new ConfirmDialog("주의", "로그인 정보가 없습니다.\n로그인 정보를 입력해주세요.");
                dialog.Owner = this;
                dialog.ShowDialog();
                return;
            }
            var res = waffle.SetWaffleCookie(id, pw);
            if (res != 1) { new ConfirmDialog("오류", "로그인 정보를 다시 확인하거나,\n네트워크 상태를 확인해주세요") { Owner = this }.ShowDialog(); return; }
            
            //try 2 times;
            res = waffle.SetWaffle(id, pw);
            if (res == -1) { res = waffle.SetWaffle(id, pw); }
            if (res != 1) { new ConfirmDialog("오류", "로그인 정보를 다시 확인하거나,\n네트워크 상태를 확인해주세요") { Owner = this }.ShowDialog(); return; }

            settings.sno = waffle.SNO;
            Console.WriteLine($@"[MainGround] SNO: {waffle.SNO}");
            settings.sname = waffle.USERNAME;
            Console.WriteLine($@"[MainGround] SNAME: {waffle.USERNAME}");

            waffle.UpdateWaffleData();
        }

        private void Timer_Update(object sender, EventArgs e)
        {
            if(timer.IsEnabled) timer.Stop();

            settings.GetAccount(out var id, out var pw);
            var res = waffle.SetWaffle(id, pw);
            if ( res == -1 ) { LoadFromWaffle(); }
            else waffle.UpdateWaffleData();

            alarmManager.Update();

            if (Visibility == Visibility.Visible) LoadCalendar();
            
            timer.Interval = TimeSpan.FromMinutes(30);
            timer.Start();
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
                    if(p.X < SystemParameters.PrimaryScreenWidth / 3)
                    {
                        WindowState = WindowState.Normal;
                        Left = 0;
                        Top = p.Y - 10;
                    }
                    else if(p.X > SystemParameters.PrimaryScreenWidth / 3 * 2)
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
        }
        private void MouseEnter_Close(object sender, MouseEventArgs e)
        {
            bd_closeBtn.Background = Brushes.Red;
        }
        private void MouseLeave_Close(object sender, MouseEventArgs e)
        {
            bd_closeBtn.Background = Brushes.Transparent;
        }
        private void MouseClick_Close(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            Hide();
        }
        #endregion

        #region DB Test

        [DllImport("DBFileManager.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int initializeDB(string dbpath);

        [DllImport("DBFileManager.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int createDB(string dbpath);

        [DllImport("DBFileManager.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void initializeAnnvDB(string dbname);

        [DllImport("DBFileManager.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void createAnnvDB(string dbname);

        private void CheckComponentDB()
        {
            List<string> tables = new List<string> { "task", "schedule", "memo", "waffle", "setting", "account", "components", "memo_components", "memo_contents", "db_version" };
            SQLiteDB db = new SQLiteDB();

            try
            {
                db.CheckDatabase(tables);
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(db.GetDirPath());
                createDB(db.GetFullPath());
            }
            catch (TaskCanceledException)
            {
                initializeDB(db.GetFullPath());
            }
           
            Console.WriteLine($@"[CROFFLE] Current DB Version: {db.GetDB_Version()}");

            AnnvAPI annv = new AnnvAPI(annv_api_key);
            try
            {
                annv.CheckAnnv();
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(annv.GetDirPath());
                createAnnvDB(annv.GetFullPath());
            }
            catch (TaskCanceledException)
            {
                initializeAnnvDB(annv.GetFullPath());
            }

            Console.WriteLine($@"[CROFFLE] Current Annv DB Version: {annv.GetDB_Version()}");
        } // TestComponentDB
        #endregion
    } // MainGround
} // CROFFLE_WPF.WPF_xamls
