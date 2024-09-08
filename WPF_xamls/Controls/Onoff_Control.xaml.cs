using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CROFFLE_WPF.WPF_xamls.Controls
{
    /// <summary>
    /// Onoff_Control.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Onoff_Control : UserControl
    {
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
            "ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Onoff_Control));
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        private bool value = false;

        public bool state
        {
            get {  return value; }
            set
            {
                if (this.value != value){
                    Change();
                }
            }
        }

        public Onoff_Control() // 파라미터로 value 받기
        {
            InitializeComponent();
        }

        public void Change()
        {
            DoubleAnimation da = new DoubleAnimation();

            if (value)
            {
                Move_off();
            }
            else
            {
                Move_on();
            }
            RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }

        private void Move_off()
        {
            value = false;
            DoubleAnimation da = new DoubleAnimation();
            da.From = Width;
            da.To = 20;
            switch_bd.Background = Brushes.LightGray;
            Console.WriteLine($@"On Off Switch Value Changed: {value}");
            da.Duration= new Duration(new TimeSpan(500000));
            changer_grid.BeginAnimation(WidthProperty, da);
        }

        private void Move_on()
        {
            value = true;
            DoubleAnimation da = new DoubleAnimation();
            da.From = 20;
            da.To = Width;
            switch_bd.Background = Brushes.LimeGreen;
            Console.WriteLine($@"On Off Switch Value Changed: {value}");
            da.Duration = new Duration(new TimeSpan(500000));
            changer_grid.BeginAnimation(WidthProperty, da);
        }

        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            Change();
        }
    }
}
