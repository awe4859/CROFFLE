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
    /// EditorCalendar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditorCalendar : Window
    {
        public bool modified = false;
        public DateTime? SelectedDateTime { get; private set; }
        public DateTime Value
        {
            get { return SelectedDateTime ?? default(DateTime); }
        }

        internal EditorCalendar(ref DateTime dateTime)
        {
            InitializeComponent();

            for (int i = 0; i < 24; i++)
            {
                hoursComboBox.Items.Add(i.ToString("00") + "시");
            }

            for (int i = 0; i < 60; i++)
            {
                minutesComboBox.Items.Add(i.ToString("00") + "분");
            }
            SelectedDateTime = dateTime;

        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = calendar.SelectedDate;
            if (selectedDate.HasValue)
            {
                DateTime temp = selectedDate.Value;
                temp = temp.AddHours(hoursComboBox.SelectedIndex);
                temp = temp.AddMinutes(minutesComboBox.SelectedIndex);
                SelectedDateTime = temp;
                modified = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("날짜를 고르지 않았습니다.");
            }

        }

    }
}
