using CROFFLE_WPF.Classes;
using CROFFLE_WPF.WPF_xamls.Dialog;
using CroffleDataManager.SQLiteDB;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CROFFLE_WPF.WPF_xamls.Editor
{
    /// <summary>
    /// ScheduleEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ScheduleEditor : EditorWindow
    {
        private Schedules schedules;
        private bool sizecontrol = true;

        private DateTime sTime;
        private DateTime eTime;
        private string title;
        private string place;
        private string transp;
        private string etc;

        private Button color_btn;
        private SQLiteDB db;

        private bool isChanged = false;
        private bool isColorChanged = false;
        private bool isAllDay = false;

        internal ScheduleEditor(DateTime date)
        {
            InitializeComponent();
            schedules = null;
            db = new SQLiteDB();
            sTime = date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
            eTime = date.AddHours(DateTime.Now.Hour + 1).AddMinutes(DateTime.Now.Minute);
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        internal ScheduleEditor(string contentsID)
        {
            InitializeComponent();
            schedules = new Schedules(contentsID);
            db = new SQLiteDB();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Console.WriteLine("[ScheduleEditor] ScheduleEditor: Existing Schedule");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("[ScheduleEditor] Window_Loaded: Loading");
            if (schedules == null)
            {
                Console.WriteLine("> [Window_Loaded] Set New Schedule.");
                color_btn = btn_Red;
                isColorChanged = true;
                sTime = DateTime.Now;
                eTime = DateTime.Now.AddHours(1);
                title = "";
                place = "";
                transp = "";
                etc = "";
            }
            else
            {
                Console.WriteLine("> [Window_Loaded] Set Existing Schedule.");
                schedules.GetARGB(out Color color);
                color_btn = ButtonFind(color);
                title = schedules.title;
                sTime = schedules.startTime;
                eTime = schedules.endTime;
                isAllDay = schedules.bAllDay;
                place = schedules.place;
                transp = schedules.transp;
                etc = schedules.etc;

                tb_title.Text = schedules.title;
                DayStart_lb.Content = schedules.startTime.ToString("시작: yyyy년 MM월 dd일");
                DayEnd_lb.Content = schedules.endTime.ToString("종료: yyyy년 MM월 dd일");
                if (!isAllDay)
                {
                    DayStart_lb.Content += schedules.startTime.ToString(" HH시 mm분");
                    DayEnd_lb.Content += schedules.endTime.ToString(" HH시 mm분");
                }
                tb_place.Text = place;
                tb_transp.Text = transp;
                tb_etc.Text = etc;
                SW_AllDay.state = isAllDay;
                sw_alarm.state = schedules.bAlarm;
                sw_done.state = schedules.bDone;

                lb_title.Foreground = Brushes.Transparent;
                if(place != "") lb_place.Foreground = Brushes.Transparent;
                if(transp != "") lb_transp.Foreground = Brushes.Transparent;
                if(etc != "") lb_etc.Foreground = Brushes.Transparent;
            }
            SW_AllDay.ValueChanged += new RoutedEventHandler(SW_AllDay_Changed);
            sw_alarm.ValueChanged += new RoutedEventHandler(SW_Changed);
            sw_done.ValueChanged += new RoutedEventHandler(SW_Changed);
            ChangeColor(color_btn);
            CheckChange();
        }

        //일정추가정보 열기/닫기
        public void fold_btn_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation();
            if (sizecontrol)
            {
                da.From = Height;
                da.To = Height + 175;
                lb_fold.Content = "닫기 △";
                sizecontrol = false;
            }
            else
            {
                da.From = Height;
                da.To = Height - 175;
                lb_fold.Content = "더보기▽";
                sizecontrol = true;
            }
            da.Duration = new Duration(new TimeSpan(1000000));
            BeginAnimation(HeightProperty, da);
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
            if (tb == tb_place)
            {
                lb_place.Foreground = Brushes.Transparent;
            }
            if (tb == tb_transp)
            {
                lb_transp.Foreground = Brushes.Transparent;
            }
            if (tb == tb_etc)
            {
                lb_etc.Foreground = Brushes.Transparent;
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
            if (tb == tb_place)
            {
                if (tb_place.Text == "") lb_place.Foreground = Brushes.DimGray;
            }
            if (tb == tb_transp)
            {
                if (tb_transp.Text == "") lb_transp.Foreground = Brushes.DimGray;
            }
            if (tb == tb_etc)
            {
                if (tb_etc.Text == "") lb_etc.Foreground = Brushes.DimGray;
            }
        }

        private void Btn_Color_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            ChangeColor(btn);
            color_btn = btn;

            if (schedules != null)
            {
                schedules.GetARGB(out Color color);
                isColorChanged = !ButtonFind(color).Equals(color_btn);
            }
            CheckChange();
        } // Btn_Color_Click

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (schedules == null) return;
            OkCancel_DIalog ok_Dialog = new OkCancel_DIalog("주의", "스케줄을 삭제하시겠습니까?");
            ok_Dialog.Owner = this;

            if (ok_Dialog.ShowDialog() == true)
            {
                schedules.DeleteOnDB();
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
        private void STime_Btn_Click(object sender, RoutedEventArgs e)
        {
            EditorCalendar editorCalendar = new EditorCalendar(sTime);
            // editorCalendar를 현 마우스 위치에 띄운다.
            editorCalendar.WindowStartupLocation = WindowStartupLocation.Manual;
            Point mousePosition = Mouse.GetPosition(Application.Current.MainWindow);
            editorCalendar.Left = mousePosition.X + Application.Current.MainWindow.Left;
            editorCalendar.Top = mousePosition.Y + Application.Current.MainWindow.Top;
            if(editorCalendar.ShowDialog() == true)
            {
                db.LoadOnDB("date_temp", out DataTable table);
                var selectedValue = table.Rows[0]["value"].ToString();
                sTime = DateTime.Parse(selectedValue);
                Console.WriteLine(sTime);
                DayStart_lb.Content = sTime.ToString("시작: yyyy년 MM월 dd일");
                if (!isAllDay) DayStart_lb.Content += $@"{sTime: HH시 mm분}";
                isChanged = true;
                db.ResetTable("date_temp");
            }
            CheckChange();
        }

        private void ETime_Btn_Click(object sender, RoutedEventArgs e)
        {
            EditorCalendar editorCalendar = new EditorCalendar(eTime);
            // editorCalendar를 현 마우스 위치에 띄운다.
            editorCalendar.WindowStartupLocation = WindowStartupLocation.Manual;
            Point mousePosition = Mouse.GetPosition(Application.Current.MainWindow);
            editorCalendar.Left = mousePosition.X + Application.Current.MainWindow.Left;
            editorCalendar.Top = mousePosition.Y + Application.Current.MainWindow.Top;
            if (editorCalendar.ShowDialog() == true)
            {
                db.LoadOnDB("date_temp", out DataTable table);
                var selectedValue = table.Rows[0]["value"].ToString();
                eTime = DateTime.Parse(selectedValue);
                DayEnd_lb.Content = eTime.ToString("종료: yyyy년 MM월 dd일");
                if (!isAllDay) DayEnd_lb.Content += $@"{eTime: HH시 mm분}";
                isChanged = true;
                db.ResetTable("date_temp");
            }
            CheckChange();
        }

        private void SW_AllDay_Changed(object sender, RoutedEventArgs e)
        {
            if (SW_AllDay.state)
            {
                sTime = sTime.Date;
                DayStart_lb.Content = sTime.ToString("시작: yyyy년 MM월 dd일");
                eTime = eTime.Date.AddDays(1).AddSeconds(-1);
                DayEnd_lb.Content = eTime.ToString("종료: yyyy년 MM월 dd일");
            }
            else
            {
                sTime = sTime.Date.AddHours(DateTime.Now.Hour);
                DayStart_lb.Content = sTime.ToString("시작: yyyy년 MM월 dd일 HH시 mm분");
                eTime = sTime.Date.AddHours(DateTime.Now.Hour + 1);
                DayEnd_lb.Content = eTime.ToString("종료: yyyy년 MM월 dd일 HH시 mm분");
            }
            CheckChange();
        }

        private void SW_Changed(object sender, RoutedEventArgs e)
        {
            CheckChange();
        }

        private void CheckChange()
        {
            bool changed = tb_title.Text != title
                || tb_place.Text != place
                || tb_transp.Text != transp
                || tb_etc.Text != etc;
            if (schedules != null) changed = changed || isColorChanged;
            if (schedules != null) changed = changed || sw_alarm.state != schedules.bAlarm || sw_done.state != schedules.bDone;
            if (schedules != null) changed = changed || SW_AllDay.state != schedules.bAllDay;
            if (schedules != null) changed = changed || sTime != schedules.startTime || eTime != schedules.endTime;
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
            if(sTime > eTime)
            {
                ConfirmDialog ok_Dialog = new ConfirmDialog("오류", "시작 시간이 종료 시간보다 늦습니다.");
                ok_Dialog.Owner = this;
                ok_Dialog.ShowDialog();
                return;
            }

            if (schedules == null) schedules = new Schedules();
            var color = ((SolidColorBrush)BorderFind(color_btn).Background).Color;
            schedules.FromARGB(color);
            schedules.title = tb_title.Text;
            schedules.startTime = sTime;
            schedules.endTime = eTime;
            schedules.whens = sTime.Date;
            schedules.place = tb_place.Text;
            schedules.transp = tb_transp.Text;
            schedules.etc = tb_etc.Text;
            schedules.bAlarm = sw_alarm.state;
            schedules.bDone = sw_done.state;
            schedules.bAllDay = SW_AllDay.state;
            schedules.SaveOnDB();

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
