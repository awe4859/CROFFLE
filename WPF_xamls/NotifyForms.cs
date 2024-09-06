using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using CROFFLE_WPF.WPF_xamls.Controls;

namespace CROFFLE_WPF.WPF_xamls
{
    internal class NotifyForms : Window
    {
        public static RoutedEvent NotifyAskOpenEvent = EventManager.RegisterRoutedEvent(
            "NotifyAskOpenEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DailyControl));
        public event RoutedEventHandler AskOpen
        {
            add { AddHandler(NotifyAskOpenEvent, value); }
            remove { RemoveHandler(NotifyAskOpenEvent, value); }
        }

        public static RoutedEvent NotifyAskExitEvent = EventManager.RegisterRoutedEvent(
            "NotifyAskExitEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DailyControl));
        public event RoutedEventHandler AskExit
        {
            add { AddHandler(NotifyAskExitEvent, value); }
            remove { RemoveHandler(NotifyAskExitEvent, value); }
        }

        private NotifyIcon notifyIcon;

        public NotifyForms()
        {
            SetupNotifyIcon();
        }

        public void Dispose()
        {
            notifyIcon.Dispose();
        }

        #region notify event

        private void SetupNotifyIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new Icon("../../Icon/croffle.ico");
            notifyIcon.Visible = true;
            notifyIcon.Text = "Croffle";

            var contextMenu = new ContextMenuStrip();

            var openMenuItem = new ToolStripMenuItem("열기");
            openMenuItem.Click += OpenMenuItem_Click;
            contextMenu.Items.Add(openMenuItem);

            var exitMenuItem = new ToolStripMenuItem("종료");
            exitMenuItem.Click += ExitMenuItem_Click;
            contextMenu.Items.Add(exitMenuItem);

            notifyIcon.ContextMenuStrip = contextMenu;

            // Add Click event handler
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(NotifyAskOpenEvent));
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(NotifyAskOpenEvent));
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(NotifyAskExitEvent));
        }
        #endregion
    }
}
