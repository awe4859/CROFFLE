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
    /// WaffleLoginPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WaffleLoginPage : Page
    {

        private string id_default = "아이디를 입력해주세요.";
        private string pw_default = "비밀번호를 입력해주세요.";

        public WaffleLoginPage()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox obj = (TextBox)sender;
            if (obj == UsernameTextBox && UsernameTextBox.Text == id_default)
            {
                obj.Text = "";
            }
            if(obj == PasswordTextBox && PasswordTextBox.Text == pw_default)
            {
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordBox.Focus();
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender == UsernameTextBox && UsernameTextBox.Text == "")
            {
                UsernameTextBox.Text = id_default;
            }
            if (sender == PasswordBox && PasswordBox.Password == "")
            {
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Collapsed;
            }
        }
    }
}
