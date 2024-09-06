using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CROFFLE_WPF.WPF_xamls.Pages
{
    public class LoginPage : Page
    {
        public static RoutedEvent LoggedChangeEvent = EventManager.RegisterRoutedEvent(
            "LoggedChangeEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WaffleLoginPage));
        public event RoutedEventHandler LoggedChange
        {
            add { AddHandler(LoggedChangeEvent, value); }
            remove { RemoveHandler(LoggedChangeEvent, value); }
        }
        public LoginPage() : base()
        {
        }
    }
}
