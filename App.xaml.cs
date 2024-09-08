using CROFFLE_WPF.WPF_xamls.Dialog;
using System;
using System.Diagnostics;
using System.Windows;

namespace CROFFLE_WPF
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            Process[] processes = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName.ToUpper());
            if (processes.Length > 1)
            {
                new ConfirmDialog("오류", "이미 실행 중인 프로그램이 있습니다.") { WindowStartupLocation = WindowStartupLocation.CenterScreen }.ShowDialog();
                return;
            }

            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
