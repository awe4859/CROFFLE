using CroffleDataManager.SQLiteDB;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CROFFLE_WPF.WPF_xamls.Dialog
{
    /// <summary>
    /// EditorCalendar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditorCalendar : Window
    {
        public bool modified = false;
        private DateTime value;
        private SQLiteDB db;

        internal EditorCalendar(DateTime dateTime)
        {
            InitializeComponent();

            db = new SQLiteDB();
            value = dateTime;

            SetInitialValue();

            calendar.SelectedDatesChanged += ValueChanged;
            hoursComboBox.SelectionChanged += ValueChanged;
            minutesComboBox.SelectionChanged += ValueChanged;
        }

        private void SetInitialValue()
        {

            for (int i = 0; i < 24; i++)
            {
                hoursComboBox.Items.Add(i.ToString("00") + "시");
            }

            for (int i = 0; i < 60; i++)
            {
                minutesComboBox.Items.Add(i.ToString("00") + "분");
            }

            calendar.SelectedDate = value;
            hoursComboBox.SelectedIndex = value.Hour;
            Console.WriteLine($@"hour: {hoursComboBox.SelectedIndex}");
            minutesComboBox.SelectedIndex = value.Minute;
        }

        private void MouseClick_Close(object sender, RoutedEventArgs e)
        {
            if (modified)
            {
                OkCancel_DIalog okCancel_DIalog = new OkCancel_DIalog("주의", "변경사항이 저장되지 않았습니다.\n정말로 나가시겠습니까?");
                okCancel_DIalog.ShowDialog();
                if (okCancel_DIalog.DialogResult == true)
                {
                    this.Close();
                }
            }
            else Close();
        }

        private void ValueChanged(object sender, SelectionChangedEventArgs e)
        {
            if(calendar.SelectedDate != null)
            {
                var year = calendar.SelectedDate.Value.Year;
                var month = calendar.SelectedDate.Value.Month;
                var day = calendar.SelectedDate.Value.Day;
                DateTime temp = new DateTime(year, month, day);
                Console.WriteLine($@"temp: {temp}");
                temp = temp.AddHours(hoursComboBox.SelectedIndex).AddMinutes(minutesComboBox.SelectedIndex);

                if(value.Equals(temp))
                {
                    modified = false;
                }
                else
                {
                    modified = true;
                }
                value = temp;
                Console.WriteLine($@"value: {value}");  
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            db.SaveOnDB("date_temp", $@"datetime('{value:yyyy-MM-dd HH:mm}')");
            DialogResult = true;
            Close();
        }


        #region TitleBar
        private void MouseDragTitle(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
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
        #endregion
    }
}
