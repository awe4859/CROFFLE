using Croffle.Classes;
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
using System.Windows.Shapes;

namespace CROFFLE_WPF.WPF_xamls
{
    /// <summary>
    /// ScheduleEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ScheduleEditor : Window
    {
        private Schedules schedules;
        private bool sizecontrol = true;

        internal ScheduleEditor()
        {
            InitializeComponent();
            schedules = new Schedules();
            Schedule_Edit();
        }

        private void Schedule_Edit()
        {
            subject_tb.Foreground = Brushes.Black;
            if (!string.IsNullOrEmpty(schedules.place))
            {
                place_tb.Text = schedules.place;
                place_tb.Foreground = Brushes.Black;
            }
            Delete_Btn.IsCancel = schedules.canceled;
            Ok_bt.IsEnabled = true;
        }
        //창 드래그
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        //일정추가정보 열기/닫기
        public void fold_btn_Click(object sender, RoutedEventArgs e)
        {
            if (sizecontrol)
            {
                this.Height += 175;
                fold_btn.Content = "닫기";
                sizecontrol = false;
            }
            else
            {
                this.Height -= 175;
                fold_btn.Content = "더보기<";
                sizecontrol = true;
            }
        }

        //TextBox 입력
        private void subject_tb_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "제목을 입력하세요")
            {
                textBox.Text = "";
            }
        }
        private void place_tb_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "장소를 입력하세요")
            {
                textBox.Text = "";
            }
        }
        private void transportation_tb_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "이동수단을 입력하세요")
            {
                textBox.Text = "";
            }
        }
        private void etc_tb_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "기타 메모사항을 입력하세요")
            {
                textBox.Text = "";
            }
        }

        //Editor Color 
        private void Button_Red_Click(object sender, RoutedEventArgs e)
        {
            Color color = Color.FromArgb(255, 255, 68, 25);
            titleBar_bd.Background = new SolidColorBrush(color);
            Ok_bd.Background = new SolidColorBrush(color);

        }

        private void Button_Green_Click(object sender, RoutedEventArgs e)
        {
            Color color = Color.FromArgb(255, 128, 255, 42);
            titleBar_bd.Background = new SolidColorBrush(color);
            Ok_bd.Background = new SolidColorBrush(color);

        }
        private void Button_Blue_Click(object sender, RoutedEventArgs e)
        {
            Color color = Color.FromArgb(255, 0, 165, 255);
            titleBar_bd.Background = new SolidColorBrush(color);
            Ok_bd.Background = new SolidColorBrush(color);


        }
        private void Button_Yellow_Click(object sender, RoutedEventArgs e)
        {
            titleBar_bd.Background = new SolidColorBrush(Color.FromRgb(255, 215, 0));
            Ok_bd.Background = new SolidColorBrush(Color.FromRgb(255, 215, 0));
        }

        //trash-can icon Color
        private void Delete_Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            delete_icon.Source = new BitmapImage(new Uri("/Icon/trash-can-red.png", UriKind.Relative));
        }
        private void Delete_Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            delete_icon.Source = new BitmapImage(new Uri("/Icon/trash-can-solid.png", UriKind.Relative));
        }

        //Editor Exit
        public void cancel_bt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Editor Delete
        private void Delete_Btn_Click(object sender, RoutedEventArgs e)
        {
            schedules.DeleteOnDB();
            this.Close();
        }

        // EditorCalendar Open
        private void daystart_bt_Click(object sender, RoutedEventArgs e)
        {
            EditorCalendar editorCalendar = new EditorCalendar(ref schedules.scheduleTime);
            // editorCalendar를 현 마우스 위치에 띄운다.
            editorCalendar.WindowStartupLocation = WindowStartupLocation.Manual;
            Point mousePosition = Mouse.GetPosition(Application.Current.MainWindow);
            editorCalendar.Left = mousePosition.X + Application.Current.MainWindow.Left;
            editorCalendar.Top = mousePosition.Y + Application.Current.MainWindow.Top;
            editorCalendar.ShowDialog();
            if (editorCalendar.modified)
            {
                schedules.scheduleTime = (DateTime)editorCalendar.SelectedDateTime;
                //schedules.SaveOnDB();
                DayStart_lb.Content = editorCalendar.Value.ToString("yyyy년 MM월 dd일 HH시 mm분");

                if (onoffControl.state)
                {
                    schedules.endTime = schedules.scheduleTime.Date.AddHours(23).AddMinutes(59);
                    DayEnd_lb.Content = schedules.endTime.ToString("yyyy년 MM월 dd일 HH시 mm분");
                }
            }
        }

        private void dayend_bt_Click(object sender, RoutedEventArgs e)
        {
            EditorCalendar editorCalendar = new EditorCalendar(ref schedules.endTime);
            // editorCalendar를 현 마우스 위치에 띄운다.
            editorCalendar.WindowStartupLocation = WindowStartupLocation.Manual;
            Point mousePosition = Mouse.GetPosition(Application.Current.MainWindow);
            editorCalendar.Left = mousePosition.X + Application.Current.MainWindow.Left;
            editorCalendar.Top = mousePosition.Y + Application.Current.MainWindow.Top;
            if (onoffControl.state)
            {
                MessageBox.Show("시작일을 선택하세요.");
            }
            else
            {
                editorCalendar.ShowDialog();
                if (editorCalendar.modified)
                {
                    schedules.endTime = (DateTime)editorCalendar.SelectedDateTime;
                    //schedules.SaveOnDB();
                    DayEnd_lb.Content = editorCalendar.Value.ToString("yyyy년 MM월 dd일 HH시 mm분");
                }
            }
        }

        private void OnoffControl_Click(Object sender, MouseButtonEventArgs e)
        {
            if (schedules.scheduleTime == default(DateTime) && schedules.endTime == default(DateTime))
            {
                return;
            }
            else
            {
                schedules.endTime = schedules.scheduleTime.Date.AddHours(23).AddMinutes(59);
                DayEnd_lb.Content = schedules.endTime.ToString("yyyy년 MM월 dd일 HH시 mm분");
            }
        }

        //ScheduleSave
        private void Ok_bt_Click(object sender, RoutedEventArgs e)
        {
            if (schedules.scheduleTime > schedules.endTime)
            {
                MessageBox.Show("시작일보다 종료일이 앞설 수 없습니다.");
            }
            else
            {
                schedules.title = subject_tb.Text;
                schedules.place = place_tb.Text;
                schedules.SaveOnDB();
                this.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
