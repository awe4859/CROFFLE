using CROFFLE_WPF.Classes;
using CROFFLE_WPF.WPF_xamls.Dialog;
using System;
using System.Windows;
using WaffleHttp;

namespace CROFFLE_WPF.WPF_xamls.Pages
{
    /// <summary>
    /// WaffleLoginPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WaffleLoginPage : LoginPage
    {
        private Settings settings;
        internal WaffleLoginPage(ref Settings settings)
        {
            InitializeComponent();
            this.settings = settings;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender == tb_id)
            {
                lb_id.Visibility = Visibility.Hidden;
            }
            if (sender == tb_pw)
            {
                lb_pw.Visibility = Visibility.Hidden;
            }
        } // TextBox_GotFocus

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender == tb_id)
            {
                if (tb_id.Text == "")
                {
                    lb_id.Visibility = Visibility.Visible;
                }
            }
            if (sender == tb_pw)
            {
                if (tb_pw.Password == "")
                {
                    lb_pw.Visibility = Visibility.Visible;
                }
            }
        } // TextBox_LostFocus

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            login_check();
        } // btn_login_Click

        private void Enter_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                login_check();
            }
        } // Enter_KeyDown

        private void login_check()
        {
            if (tb_id.Text == "")
            {
                var dialog = new ConfirmDialog("주의", "아이디를 입력해주세요.");
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
                return;
            }
            if (tb_pw.Password == "")
            {
                var dialog = new ConfirmDialog("주의", "비밀번호를 입력해주세요.");
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
                return;
            }

            var waffle = new Waffle("application/x-www-form-urlencoded; charset=utf-8", 0, 0, 0);
            waffle.SetWaffleCookie(tb_id.Text, tb_pw.Password);
            var result = waffle.SetWaffle(tb_id.Text, tb_pw.Password);
            Console.WriteLine(result);
            if (result == -1) new ConfirmDialog("알림", "오류가 발생했습니다.\n잠시 후 다시 시도해주세요.") { Owner = Window.GetWindow(Parent) }.ShowDialog();
            if (result == -2) new ConfirmDialog("알림", "아이디 또는 비밀번호가 일치하지 않습니다.") { Owner = Window.GetWindow(Parent) }.ShowDialog();
            if (result != 1) return;

            settings.SetAccount(tb_id.Text, tb_pw.Password);
            settings.SaveAccount();
            new ConfirmDialog("알림", "로그인 성공!") { Owner = Window.GetWindow(Parent) }.ShowDialog();
            RaiseEvent(new RoutedEventArgs(LoggedChangeEvent));
            // 로그인 처리
        } // login_check
    } // class
} // namespace
