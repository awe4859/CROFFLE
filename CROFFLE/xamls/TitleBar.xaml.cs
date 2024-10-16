using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;

namespace CROFFLE.xamls
{
    /// <summary>
    /// Interaction logic for TitleBar.xaml
    /// </summary>
    public partial class TitleBar : UserControl
    {
        public static readonly DependencyProperty titleDependencyProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TitleBar), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty BackgroundDependencyProperty =
            DependencyProperty.Register("BackColor", typeof(Brush), typeof(TitleBar), new PropertyMetadata(Brushes.Gray));
        public static readonly DependencyProperty ForegroundDependencyProperty =
            DependencyProperty.Register("ForeColor", typeof(Brush), typeof(TitleBar), new PropertyMetadata(Brushes.White));
        public static readonly DependencyProperty FontFamilyDependencyProperty =
            DependencyProperty.Register("TitleFontFamily", typeof(FontFamily), typeof(TitleBar), new PropertyMetadata(new FontFamily("../Font/#KCC-Ganpan")));
        public static readonly DependencyProperty FontSizeDependencyProperty =
            DependencyProperty.Register("TitleFontSize", typeof(double), typeof(TitleBar), new PropertyMetadata(20.0));
        
        public static readonly DependencyProperty HeightDependencyProperty =
            DependencyProperty.Register("TitleHeight", typeof(double), typeof(TitleBar), new PropertyMetadata(30.0));
        public static readonly DependencyProperty WidthDependencyProperty =
            DependencyProperty.Register("TitleWidth", typeof(double), typeof(TitleBar), new PropertyMetadata(800.0));
        
        public string Title
        {
            get { return (string)GetValue(titleDependencyProperty); }
            set { SetValue(titleDependencyProperty, value); }
        }
        public Brush BackColor
        {
            get { return (Brush)GetValue(BackgroundDependencyProperty); }
            set { SetValue(BackgroundDependencyProperty, value); }
        }
        public Brush ForeColor
        {
            get { return (Brush)GetValue(ForegroundDependencyProperty); }
            set { SetValue(ForegroundDependencyProperty, value); }
        }
        public FontFamily TitleFontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyDependencyProperty); }
            set { SetValue(FontFamilyDependencyProperty, value); }
        }
        public double TitleFontSize
        {
            get { return (double)GetValue(FontSizeDependencyProperty); }
            set { SetValue(FontSizeDependencyProperty, value); }
        }
        public double TitleHeight
        {
            get { return (double)GetValue(HeightDependencyProperty); }
            set { SetValue(HeightDependencyProperty, value); }
        }
        public double TitleWidth
        {
            get { return (double)GetValue(WidthDependencyProperty); }
            set { SetValue(WidthDependencyProperty, value); }
        }


        public TitleBar()
        {
            InitializeComponent();
        } // TitleBar

        private void TitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.StateChanged += Window_StateChanged;
                WindowChrome windowChrome = new()
                {
                    CaptionHeight = 0,
                    UseAeroCaptionButtons = false,
                    ResizeBorderThickness = new(5)
                };
                WindowChrome.SetWindowChrome(window, windowChrome);
            }
            else if (Application.Current.GetType().Name != "WpfSurfaceApp")
            {
                throw new("TitleBar must be a child of a Window");
            }
        } // TitleBar_Loaded

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.WindowState = WindowState.Minimized;
        } // MinimizeButton_Click

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window.WindowState == WindowState.Maximized)
            {
                //TitleGrid.Margin = new(0);
                btnMaximize.Content = 1;
                window.WindowState = WindowState.Normal;
            }
            else
            {
                //TitleGrid.Margin = new(8, 8, 8, 0);
                btnMaximize.Content = 2;
                window.WindowState = WindowState.Maximized;
            }
        } // MaximizeButton_Click

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Close();
        } // CloseButton_Click

        private void Mouse_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window.WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Normal;

                var mousePosition = e.GetPosition(window);
                var screenPosition = PointToScreen(mousePosition);
                window.Top = mousePosition.Y - TitleHeight * 2 / 3;
                window.Left = mousePosition.X - window.Width / 2;
            }
            window.DragMove();
        } // Mouse_LeftButtonDown

        private void Window_StateChanged(object sender, EventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window.WindowState == WindowState.Maximized)
            {
                TitleGrid.Margin = new(8, 8, 8, 0);
                btnMaximize.Content = 2;
            }
            else
            {
                TitleGrid.Margin = new(0);
                btnMaximize.Content = 1;
            }
        }// Window_StateChanged
    } // TitleBar
} // CROFFLE.xamls
