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
    /// TaskEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TaskEditor : Window
    {
        private Tasks tasks;
        private Schedules schedules;

        public TaskEditor()
        {
            InitializeComponent();
            tasks = new Tasks();
            schedules = new Schedules();
            Task_Edit();
        }
        private void Task_Edit()
        {
            Subject_tb.Foreground = Brushes.Black;
            Delete_Btn.IsEnabled = schedules.canceled;
            Ok_bt.IsEnabled = true;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        //TextBox 입력
        private void Subject_tb_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "제목을 입력하세요")
            {
                textBox.Text = "";
            }
        }
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
        public void Delete_Btn_Click(object sender, RoutedEventArgs e)
        {
            tasks.DeleteOnDB();
            this.Close();
        }

        // EditorCalendar Open
        private void dayend_bt_Click(object sender, RoutedEventArgs e)
        {
            EditorCalendar editorCalendar = new EditorCalendar(ref schedules.endTime);
            // editorCalendar를 현 마우스 위치에 띄운다.
            editorCalendar.WindowStartupLocation = WindowStartupLocation.Manual;
            Point mousePosition = Mouse.GetPosition(Application.Current.MainWindow);
            editorCalendar.Left = mousePosition.X + Application.Current.MainWindow.Left;
            editorCalendar.Top = mousePosition.Y + Application.Current.MainWindow.Top;
            editorCalendar.ShowDialog();
            if (editorCalendar.modified)
            {
                schedules.endTime = (DateTime)editorCalendar.SelectedDateTime;
                DayEnd_lb.Content = editorCalendar.Value.ToString("yyyy년 MM월 dd일 HH시 mm분");
            }
        }

        //TaskSave
        private void Ok_bt_Click(object sender, RoutedEventArgs e)
        {
            tasks.title = Subject_tb.Text;
            tasks.SaveOnDB();
            this.Close();
        }
    }
}
