using CROFFLE_WPF.Classes;
using CROFFLE_WPF.WPF_xamls.Dialog;
using CroffleDataManager.SQLiteDB;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CROFFLE_WPF.WPF_xamls.Editor
{
    /// <summary>
    /// TaskEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TaskEditor : EditorWindow
    {
        private Tasks task;

        private DateTime deadline;
        private string title;

        private Button color_btn;
        private SQLiteDB db;

        private bool isChanged = false;
        private bool isColorChanged = false;

        public TaskEditor(DateTime date)
        {
            InitializeComponent();
            db = new SQLiteDB();
            task = null;
            deadline = date.AddHours(DateTime.Now.Hour);
            deadline = deadline.AddMinutes(DateTime.Now.Minute);
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        public TaskEditor(string contentID)
        {
            InitializeComponent();
            task = new Tasks(contentID);
            db = new SQLiteDB();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(task == null)
            {
                color_btn = btn_Red;
                isColorChanged = true;
                title = "";
            }
            else
            {
                task.GetARGB(out Color color);
                color_btn = ButtonFind(color);
                title = task.title;
                deadline = task.deadline;

                tb_title.Text = title;
                lb_deadline.Content = deadline.ToString("yyyy년 MM월 dd일 HH시 mm분");
                sw_alarm.state = task.bAlarm;
                sw_done.state = task.bDone;

                lb_title.Foreground = Brushes.Transparent;
            }
            ChangeColor(color_btn);
            CheckChange();
        }

        private void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckChange();
        }

        //TextBox 입력
        private void tb_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb == null) return;

            if (tb == tb_title)
            {
                lb_title.Foreground = Brushes.Transparent;
            }
        }

        private void tb_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            if (tb == tb_title)
            {
                if (tb_title.Text == "") lb_title.Foreground = Brushes.DimGray;
            }
        }

        private void Btn_Color_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            ChangeColor(btn);
            color_btn = btn;

            if (task != null)
            {
                task.GetARGB(out Color color);
                isColorChanged = !ButtonFind(color).Equals(color_btn);
            }
            CheckChange();
        } // Btn_Color_Click

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (task == null) return;
            OkCancel_DIalog ok_Dialog = new OkCancel_DIalog("주의", "메모를 삭제하시겠습니까?");
            ok_Dialog.Owner = this;

            if (ok_Dialog.ShowDialog() == true)
            {
                task.DeleteOnDB();
                RaiseEvent(new RoutedEventArgs(AskUpdate));
                Close();
            }
        } // Btn_Delete_Click

        private void Btn_Delete_MouseEnter(object sender, MouseEventArgs e)
        {
            img_delete.Source = new ImageSourceConverter().ConvertFromString("../../Icon/trash-can-red.png") as ImageSource;
        } // Btn_Delete_MouseEnter
        private void Btn_Delete_MouseLeave(object sender, MouseEventArgs e)
        {
            img_delete.Source = new ImageSourceConverter().ConvertFromString("../../Icon/trash-can-solid.png") as ImageSource;
        } // Btn_Delete_MouseLeave

        // EditorCalendar Open
        private void Btn_Deadline_Click(object sender, RoutedEventArgs e)
        {
            EditorCalendar editorCalendar = new EditorCalendar(deadline);
            // editorCalendar를 현 마우스 위치에 띄운다.
            editorCalendar.WindowStartupLocation = WindowStartupLocation.Manual;
            Point mousePosition = Mouse.GetPosition(Application.Current.MainWindow);
            editorCalendar.Left = mousePosition.X + Application.Current.MainWindow.Left;
            editorCalendar.Top = mousePosition.Y + Application.Current.MainWindow.Top;
            if (editorCalendar.ShowDialog() == true)
            {
                db.LoadOnDB("date_temp", out DataTable table);
                var selectedValue = table.Rows[0]["value"].ToString();
                deadline = DateTime.Parse(selectedValue);
                lb_deadline.Content = deadline.ToString("시작: yyyy년 MM월 dd일 HH시 mm분");
                isChanged = true;
                db.ResetTable("date_temp");
            }
            CheckChange();
        }

        private void Sw_ValueChanged(object sender, RoutedEventArgs e)
        {
            CheckChange();
        }

        private void CheckChange()
        {
            bool changed = tb_title.Text != title;
            if (task != null) changed = changed || isColorChanged;
            if (task != null) changed = changed || sw_alarm.state != task.bAlarm || sw_done.state != task.bDone;
            if (task != null) changed = changed || deadline != task.deadline;
            if (changed)
            {
                isChanged = true;
                lb_OK.Foreground = Brushes.White;
            }
            else
            {
                isChanged = false;
                lb_OK.Foreground = Brushes.DimGray;
            }
        } // CheckChange

        #region ColorChange
        private void ChangeColor(Button button)
        {
            var border = BorderFind(color_btn);
            if (border != null) border.BorderThickness = new Thickness(0);
            border = BorderFind(button);
            if (border != null) border.BorderThickness = new Thickness(3);
        } // ChangeColor

        private Border BorderFind(Button button)
        {
            if (button == btn_Red) return bd_Btn_Red;
            if (button == btn_Green) return bd_Btn_Green;
            if (button == btn_Blue) return bd_Btn_Blue;
            if (button == btn_Yellow) return bd_Btn_Yellow;

            return null;
        } // BorderFind

        private Button ButtonFind(Color color)
        {
            if (color.Equals(((SolidColorBrush)bd_Btn_Red.Background).Color)) return btn_Red;
            if (color.Equals(((SolidColorBrush)bd_Btn_Green.Background).Color)) return btn_Green;
            if (color.Equals(((SolidColorBrush)bd_Btn_Blue.Background).Color)) return btn_Blue;
            if (color.Equals(((SolidColorBrush)bd_Btn_Yellow.Background).Color)) return btn_Yellow;

            return null;
        } // ColorChecker
        #endregion

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
        #endregion

        #region Footer
        private void MouseClick_Save(object sender, RoutedEventArgs e)
        {
            if (!isChanged) return;
            if (tb_title.Text == string.Empty)
            {
                ConfirmDialog ok_Dialog = new ConfirmDialog("주의", "제목을 입력해주세요.");
                ok_Dialog.Owner = this;
                ok_Dialog.ShowDialog();
                return;
            }
            if (task == null) task = new Tasks();
            var color = ((SolidColorBrush)BorderFind(color_btn).Background).Color;
            task.FromARGB(color);
            task.title = tb_title.Text;
            task.deadline = deadline;
            task.whens = deadline.Date;
            task.bAlarm = sw_alarm.state;
            task.bDone = sw_done.state;
            task.SaveOnDB();

            RaiseEvent(new RoutedEventArgs(AskUpdate));

            Close();
        } // MouseClick_Save

        private void MouseClick_Close(object sender, RoutedEventArgs e)
        {
            if (!isChanged) { Close(); return; }
            OkCancel_DIalog ok_Dialog = new OkCancel_DIalog("주의", "저장하지 않았습니다. 취소하시겠습니까?");
            ok_Dialog.Owner = this;

            if (ok_Dialog.ShowDialog() == true)
            {
                Close();
            }
        } // MouseClick_Close
        #endregion
    }
}
