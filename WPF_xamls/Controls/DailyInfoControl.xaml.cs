using CROFFLE_WPF.Classes;
using CroffleDataManager.SQLiteDB;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using CROFFLE_WPF.WPF_xamls.Editor;
using CROFFLE_WPF.WPF_xamls.Dialog;
using CROFFLE_WPF.WPF_xamls.Windows;
using CROFFLE_WPF.Classes.MainAbstract;

namespace CROFFLE_WPF.WPF_xamls.Controls
{
    /// <summary>
    /// DailyInfoControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DailyInfoControl : UserControl
    {
        public static RoutedEvent IsUpdated = EventManager.RegisterRoutedEvent(
            "IsDeleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DailyInfoControl));

        public event RoutedEventHandler IsUpdatedEvent
        {
            add { AddHandler(IsUpdated, value); }
            remove { RemoveHandler(IsUpdated, value); }
        }

        public static RoutedEvent AskClose = EventManager.RegisterRoutedEvent(
            "AskClose", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DailyInfo));
        public event RoutedEventHandler AskCloseEvent
        {
            add { AddHandler(AskClose, value); }
            remove { RemoveHandler(AskClose, value); }
        }

        private string contentID;
        private string title;

        private DateTime date;

        private Color color;

        private bool isDone = false;
        private bool isAlarm = false;

        public bool IsDone
        {
            get { return isDone; }
            set { isDone = value; }
        }
        public bool IsAlarm
        {
            get { return isAlarm; }
            set { isAlarm = value; }
        }

        public bool done_DoubleC = false;

        public DailyInfoControl() {
            InitializeComponent();
        }

        public DailyInfoControl(string id, string title, DateTime date, int color_argb)
        {
            InitializeComponent();
            this.date = date;
            this.title = title;
            contentID = id;
            color = Color.FromArgb((byte)(color_argb >> 24), (byte)(color_argb >> 16), (byte)(color_argb >> 8), (byte)color_argb);
        }

        private void LoadedComponent(object sender, RoutedEventArgs e)
        {
            sw_alarm.state = isAlarm;
            UpdateInfo();
        }

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            ADailyProperty content;
            if (new OkCancel_DIalog("주의", "정말 삭제하시겠습니까?") { Owner = Window.GetWindow(Parent) }.ShowDialog() == false) return;
            var type = Contents.Name(contentID);
            if (type == Contents.Name(EContents.eWaffle))
            {
                new ConfirmDialog("오류", "와플 과제는 삭제할 수 없습니다.") { Owner = Window.GetWindow(Parent) }.ShowDialog();
                return;
            }
            if (type == Contents.Name(EContents.eMemo)) content = new Memos(contentID);
            else if (type == Contents.Name(EContents.eSchedule)) content = new Schedules(contentID);
            else if (type == Contents.Name(EContents.eTask)) content = new Tasks(contentID);
            else return;
            content.DeleteOnDB();
            ((StackPanel)Parent).Children.Remove(this);
            RaiseEvent(new RoutedEventArgs(IsUpdated));
        }
        
        private void Click_SW_Changed(object sender, RoutedEventArgs e)
        {
            var db = new SQLiteDB();
            var sw = sender as Onoff_Control;
            if (sw == null) return;

            var type = Contents.Name(contentID);
            db.UpdateOnDB(type, contentID, $@"alarm='{sw.state}'");
        }

        private void Btn_Delete_MouseEnter(object sender, MouseEventArgs e)
        {
            img_delete.Source = new ImageSourceConverter().ConvertFromString("Icon/trash-can-red.png") as ImageSource;
            img_delete.Opacity = 1;
        } // Btn_Delete_MouseEnter
        private void Btn_Delete_MouseLeave(object sender, MouseEventArgs e)
        {
            img_delete.Source = new ImageSourceConverter().ConvertFromString("Icon/trash-can-solid.png") as ImageSource;
            img_delete.Opacity = 0.5;
        } // Btn_Delete_MouseLeave

        private void DoubleClick_DailyInfoControl(object sender, MouseButtonEventArgs e)
        {
            var type = Contents.Name(contentID);
            if (done_DoubleC)
            {
                if(type == Contents.Name(EContents.eTask))
                {
                    var content = new Tasks(contentID);
                    if (content.bDone == false) content.bDone = true;
                    content.SaveOnDB();
                    return;
                }
                if (type == Contents.Name(EContents.eSchedule))
                {
                    var content = new Schedules(contentID);
                    if (content.bDone == false) content.bDone = true;
                    content.SaveOnDB();
                    return;
                }
            }
            modify();
        }

        private void Modify_DailyInfoControl(object sender, RoutedEventArgs e)
        {
            modify();
        }

        private void AskUpdate(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateInfo();
                RaiseEvent(new RoutedEventArgs(IsUpdated));
            }
            catch (IndexOutOfRangeException)
            {
                ((StackPanel)Parent).Children.Remove(this);
            }
        }

        private void CloseEvent(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(AskClose));
        }

        private void AddContents_Btn(object sender, RoutedEventArgs e)
        {
            var btn = sender as MenuItem;
            if (btn == null) return;

            EditorWindow editor = null;
            if (btn == cm_addMemo) editor = new MemoEditor(date);
            if (btn == cm_addTask) editor = new TaskEditor(date);
            if (btn == cm_addSchedule) editor = new ScheduleEditor(date);

            if (editor == null) { Console.WriteLine("[DailyInfo] Error: Failed to create EditorWindow from AddContents ContextMenu."); return; }

            editor.Owner = Window.GetWindow(Parent);
            editor.AskUpdateEvent += new RoutedEventHandler(AskUpdate);
            editor.ShowDialog();
        } // AddContents_Btn

        private void modify()
        {
            var type = Contents.Name(contentID);
            EditorWindow editor;

            if (type == Contents.Name(EContents.eMemo)) editor = new MemoEditor(contentID);
            else if (type == Contents.Name(EContents.eSchedule)) editor = new ScheduleEditor(contentID);
            else if (type == Contents.Name(EContents.eTask)) editor = new TaskEditor(contentID);
            else if (type == Contents.Name(EContents.eWaffle)) editor = new WaffleInfo(contentID);
            else return;

            editor.AskUpdateEvent += new RoutedEventHandler(AskUpdate);
            editor.Owner = Window.GetWindow(this);
            editor.Show();
        }

        public void UpdateInfo()
        {
            var type = Contents.Name(contentID);

            if (type == Contents.Name(EContents.eMemo))
            {
                sw_alarm.Visibility = Visibility.Hidden;
                isAlarm = false;
                isDone = false;
            }
            if (type == Contents.Name(EContents.eWaffle))
            {
                sw_alarm.Visibility = Visibility.Hidden;
                img_delete.Visibility = Visibility.Hidden;
                btn_delete.Visibility = Visibility.Hidden;
            }

            lb_title.Content = title;
            lb_date.Content = date.ToString("yyyy년 MM월 dd일 HH:mm");
            bd_background.Background = new SolidColorBrush(color);
            SetDone();
        }

        public void SetDate(DateTime date)
        {
            lb_date.Content = date.ToString("yyyy년 MM월 dd일 HH:mm");
        }

        public void SetTitle(string title)
        {
            lb_title.Content = title;
        }

        public void SetDone()
        {
            if (Contents.Name(contentID) == Contents.Name(EContents.eMemo)) return;
            if (isDone)
            {
                if (Contents.Name(contentID) != Contents.Name(EContents.eWaffle)) sw_alarm.Visibility = Visibility.Hidden;
                img_done.Visibility = Visibility.Visible;
            }
            else
            {

                if (Contents.Name(contentID) != Contents.Name(EContents.eWaffle)) sw_alarm.Visibility = Visibility.Visible;
                img_done.Visibility = Visibility.Hidden;
                sw_alarm.state = isAlarm;
            }
        }

    }
}
