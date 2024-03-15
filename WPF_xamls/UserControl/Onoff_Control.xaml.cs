using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CROFFLE_WPF.WPF_xamls
{
    /// <summary>
    /// Onoff_Control.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Onoff_Control : UserControl
    {

        private bool value = false;

        public bool state { get {  return value; } set { this.value = value; } }

        public Onoff_Control()
        {
            InitializeComponent();
            Initializing();
        }

        public void Initializing()
        {
            if (value)
            {
                switch_bd.Background = Brushes.LimeGreen;
                changer_grid.Width = Width;
            }
            else
            {
                switch_bd.Background = Brushes.LightGray;
                changer_grid.Width = Width;
            }
        }

        public void Change()
        {
            DoubleAnimation da = new DoubleAnimation();

            if (value)
            {
                da.From = Width;
                da.To = 20;
                switch_bd.Background = Brushes.LightGray;
                value = !value;
                Console.WriteLine($@"On Off Switch Value Changed: {value}");
            }
            else
            {
                da.From = 20;
                da.To = Width;
                switch_bd.Background = Brushes.LimeGreen;
                value = !value;
                Console.WriteLine($@"On Off Switch Value Changed: {value}");
            }
            da.Duration = new Duration(new TimeSpan(500000));

            changer_grid.BeginAnimation(Grid.WidthProperty, da);
        }

        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            Change();
        }
    }
}
