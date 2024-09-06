using CROFFLE_WPF.Classes;
using CROFFLE_WPF.WPF_xamls.Dialog;
using CroffleDataManager.SQLiteDB;
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
using System.Windows.Shapes;

namespace CROFFLE_WPF.WPF_xamls.Editor
{
    /// <summary>
    /// Interaction logic for WaffleInfo.xaml
    /// </summary>
    public partial class WaffleInfo : EditorWindow
    {
        WaffleTask waffleTask;

        private DateTime deadline;
        private string title;

        public WaffleInfo(string contentID)
        {
            InitializeComponent();
            waffleTask = new WaffleTask(contentID);
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            waffleTask.GetARGB(out Color color);
            title = waffleTask.title;
            deadline = waffleTask.deadline;

            lb_title.Content = title;
            lb_deadline.Content = deadline.ToString("yyyy년 MM월 dd일 HH시 mm분");
            if (waffleTask.bDone)
            {
                lb_state.Content = "완료";
                lb_state.Foreground = Brushes.Green;
            }
            else
            {
                if (deadline < DateTime.Now)
                {
                    lb_state.Content = "기한 만료";
                    lb_state.Foreground = Brushes.Red;
                }
                else
                {
                    lb_state.Content = "미완료";
                    lb_state.Foreground = Brushes.RoyalBlue;
                }
            }
        } // Window_Loaded


        #region TitleBar
        private void MouseDragTitle(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        } // MouseDragTitle

        private void MouseEnter_Close(object sender, MouseEventArgs e)
        {
            bd_closeBtn.Background = Brushes.Red;
        } // MouseEnter_Close

        private void MouseLeave_Close(object sender, MouseEventArgs e)
        {
            bd_closeBtn.Background = Brushes.Transparent;
        } // MouseLeave_Close

        private void MouseClick_Close(object sender, RoutedEventArgs e)
        {
            Close();
        } // MouseClick_Close
        #endregion

        #region Footer
        private void MouseClick_Save(object sender, RoutedEventArgs e)
        {
            Close();
        } // MouseClick_Save
        #endregion
    } // WaffleInfo
} // namespace CROFFLE_WPF.WPF_xamls.Editor
