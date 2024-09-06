using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace CROFFLE_WPF.WPF_xamls.Pages
{
    /// <summary>
    /// InfoPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InfoPage : Page
    {
        public InfoPage()
        {
            InitializeComponent();
        }

        private void RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }
    }
}
