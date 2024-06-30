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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CROFFLE_WPF.WPF_xamls
{
    /// <summary>
    /// DailyControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DailyControl : UserControl
    {
        private DateTime date_value;
        internal DateTime Day { get { return date_value; } }


        public DailyControl(DateTime date)
        {
            InitializeComponent();
            date_value = date;
            dayNum_lb.Content = date_value.ToString("dd"); // 날짜 설정
            
        }

        internal void ChangeBackground(Brush brushes)
        {
            LabelsBox_Border.Background = brushes;

            ContentsBox_Border.Background = brushes;
           // brus/-+hes = Brushes.Red;

        }
        internal void ChangeCornerRadius(int TopLeft, int TopRight, int BottomRight, int BttomLeft)
        {
           
            LabelsBox_Border.CornerRadius = new CornerRadius(TopLeft, TopRight, 0, 0);
            ContentsBox_Border.CornerRadius = new CornerRadius(0, 0, BottomRight, BttomLeft);
        }
        public bool IsToday()
        {
            return Day == DateTime.Today;
        }
    }
}
