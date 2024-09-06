using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CROFFLE_WPF.WPF_xamls.Dialog
{
    /// <summary>
    /// Interaction logic for OkCancel_DIalog.xaml
    /// </summary>
    public partial class OkCancel_DIalog : Window
    {
        public OkCancel_DIalog()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        public OkCancel_DIalog(string title, string message)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            tb_Name_lb.Content = title;
            description.Content = message;
        }

        private void MouseDragTitle(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void MouseEnter_Close(object sender, MouseEventArgs e)
        {
            bd_closeBtn.Background = Brushes.Red;
        }
        private void MouseLeave_Close(object sender, MouseEventArgs e)
        {
            bd_closeBtn.Background = Brushes.Transparent;
        }
        private void MouseClick_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
