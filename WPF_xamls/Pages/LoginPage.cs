using System.Windows;

namespace CROFFLE_WPF.WPF_xamls.Pages
{
    public class LoginPage : SettingPages
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

        public override void Save()
        {
        }
    }
}
