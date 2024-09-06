using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CROFFLE_WPF.Classes;
using CROFFLE_WPF.WPF_xamls.Dialog;
using System;


namespace CROFFLE_WPF.WPF_xamls.Editor
{
    /// <summary>
    /// MemoEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MemoEditor : EditorWindow
    {

        private Memos memo;
        private Button color_btn;

        private DateTime memo_date;

        private string title;
        private string detailText;

        private bool isChanged = false;
        private bool isColorChanged = false;

        public MemoEditor(DateTime date)
        {
            memo = null;
            memo_date = date;
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        } // MemoEditor

        public MemoEditor(string contentID)
        {
            memo = new Memos(contentID);
            memo_date = memo.whens;
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        } // MemoEditor

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (memo == null)
            {
                color_btn = btn_Red;
                isColorChanged = true;
                title = "";
                detailText = "";
            }
            else
            {
                memo.GetARGB(out Color color);
                color_btn = ButtonFind(color);
                title = memo.title;
                detailText = memo.detailText;

                tb_title.Text = memo.title;
                tb_detail.Text = memo.detailText;
                lb_title.Foreground = Brushes.Transparent;
                lb_detail.Foreground = Brushes.Transparent;

            }
            ChangeColor(color_btn);
            CheckChange();
        } // Window_Loaded

        private void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckChange();
        } // tb_TextChanged

        private void tb_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            if (tb == tb_title)
            {
                lb_title.Foreground = Brushes.Transparent;
            }
            if (tb == tb_detail)
            {
                lb_detail.Foreground = Brushes.Transparent;
            }

        } // tb_GotFocus

        private void tb_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            if (tb == tb_title)
            {
                if (tb_title.Text == "") lb_title.Foreground = Brushes.DimGray;
            }
            if (tb == tb_detail)
            {
                if (tb_detail.Text == "") lb_detail.Foreground = Brushes.DimGray;
            }
        } // tb_LostFocus

        private void Btn_Color_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            ChangeColor(btn);
            color_btn = btn;

            if (memo != null)
            {
                memo.GetARGB(out Color color);
                isColorChanged = !ButtonFind(color).Equals(color_btn);
            }
            CheckChange();
        } // Btn_Color_Click

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (memo == null) return;
            OkCancel_DIalog ok_Dialog = new OkCancel_DIalog("주의", "메모를 삭제하시겠습니까?");
            ok_Dialog.Owner = this;

            if (ok_Dialog.ShowDialog() == true)
            {
                memo.DeleteOnDB();
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


        private void CheckChange()
        {
            bool changed = !(tb_title.Text == title && tb_detail.Text == detailText);
            if (memo != null) changed = changed || isColorChanged;
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
            if(button == btn_Red) return bd_Btn_Red;
            if(button == btn_Green) return bd_Btn_Green;
            if(button == btn_Blue) return bd_Btn_Blue;
            if(button == btn_Yellow) return bd_Btn_Yellow;

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
                if (e.ClickCount == 2)
                {
                    if (WindowState == WindowState.Normal)
                        WindowState = WindowState.Maximized;
                    else
                        WindowState = WindowState.Normal;
                }
                else if (WindowState == WindowState.Maximized)
                {
                    Point p = e.GetPosition(this);
                    if (p.X < SystemParameters.PrimaryScreenWidth / 3)
                    {
                        WindowState = WindowState.Normal;
                        Left = 0;
                        Top = p.Y - 10;
                    }
                    else if (p.X > SystemParameters.PrimaryScreenWidth / 3 * 2)
                    {
                        WindowState = WindowState.Normal;
                        Left = SystemParameters.PrimaryScreenWidth - Width;
                        Top = p.Y - 10;
                    }
                    else
                    {
                        WindowState = WindowState.Normal;
                        Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
                        Top = p.Y - 10;
                    }
                }
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
            if (tb_detail.Text == string.Empty)
            {
                ConfirmDialog ok_Dialog = new ConfirmDialog("주의", "내용을 입력해주세요.");
                ok_Dialog.Owner = this;
                ok_Dialog.ShowDialog();
                return;
            }

            if(memo == null) memo = new Memos();
            var color = ((SolidColorBrush)BorderFind(color_btn).Background).Color;
            memo.FromARGB(color);
            memo.whens = memo_date;
            memo.title = tb_title.Text;
            memo.detailText = tb_detail.Text;
            memo.SaveOnDB();

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
    } // class MemoEditor
} // namespace CROFFLE_WPF.WPF_xamls