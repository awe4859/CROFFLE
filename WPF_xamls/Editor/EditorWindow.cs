using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CROFFLE_WPF.WPF_xamls.Editor
{
    public class EditorWindow : Window
    {
        public static RoutedEvent AskUpdate = EventManager.RegisterRoutedEvent(
            "AskUpdate", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ScheduleEditor));

        public event RoutedEventHandler AskUpdateEvent
        {
            add { AddHandler(AskUpdate, value); }
            remove { RemoveHandler(AskUpdate, value); }
        }
        public EditorWindow() : base()
        {
        }
    }
}
