using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CROFFLE.xamls.Controls
{
    /// <summary>
    /// Interaction logic for RoundButton.xaml
    /// </summary>
    public partial class RoundButton : UserControl
    {
        // Dependency Properties
        // Button Properties
        public static readonly DependencyProperty ButtonColorDependencyProperty =
            DependencyProperty.Register("ButtonColor", typeof(Brush), typeof(RoundButton), new PropertyMetadata(Brushes.White));
        public static readonly DependencyProperty ButtonCornerRadiusDependencyProperty =
            DependencyProperty.Register("ButtonCornerRadius", typeof(CornerRadius), typeof(RoundButton), new PropertyMetadata(new CornerRadius(5.0)));
        public static readonly DependencyProperty ButtonBorderThicknessDependencyProperty =
            DependencyProperty.Register("ButtonBorderThickness", typeof(Thickness), typeof(RoundButton), new PropertyMetadata(new Thickness(1.0)));
        public static readonly DependencyProperty ButtonBorderBrushDependencyProperty =
            DependencyProperty.Register("ButtonBorderBrush", typeof(Brush), typeof(RoundButton), new PropertyMetadata(Brushes.Black));

        // Text Properties
        public static readonly DependencyProperty ButtonTextDependencyProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(RoundButton), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty ButtonForegroundDependencyProperty =
            DependencyProperty.Register("ButtonForeground", typeof(Brush), typeof(RoundButton), new PropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty ButtonFontFamilyDependencyProperty =
            DependencyProperty.Register("ButtonFontFamily", typeof(FontFamily), typeof(RoundButton), new PropertyMetadata(new FontFamily("../Font/#KCC-Ganpan")));
        public static readonly DependencyProperty ButtonFontSizeDependencyProperty =
            DependencyProperty.Register("ButtonFontSize", typeof(double), typeof(RoundButton), new PropertyMetadata(20.0));

        // Click Event
        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RoundButton));

        // Button Properties
        public Brush ButtonColor
        {
            get { return (Brush)GetValue(ButtonColorDependencyProperty); }
            set { SetValue(ButtonColorDependencyProperty, value); }
        }
        public CornerRadius ButtonCornerRadius
        {
            get { return (CornerRadius)GetValue(ButtonCornerRadiusDependencyProperty); }
            set { SetValue(ButtonCornerRadiusDependencyProperty, value); }
        }
        public Thickness ButtonBorderThickness
        {
            get { return (Thickness)GetValue(ButtonBorderThicknessDependencyProperty); }
            set { SetValue(ButtonBorderThicknessDependencyProperty, value); }
        }
        public Brush ButtonBorderBrush
        {
            get { return (Brush)GetValue(ButtonBorderBrushDependencyProperty); }
            set { SetValue(ButtonBorderBrushDependencyProperty, value); }
        }

        // Text Properties
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextDependencyProperty); }
            set { SetValue(ButtonTextDependencyProperty, value); }
        }
        public Brush ButtonForeground
        {
            get { return (Brush)GetValue(ButtonForegroundDependencyProperty); }
            set { SetValue(ButtonForegroundDependencyProperty, value); }
        }
        public FontFamily ButtonFontFamily
        {
            get { return (FontFamily)GetValue(ButtonFontFamilyDependencyProperty); }
            set { SetValue(ButtonFontFamilyDependencyProperty, value); }
        }
        public double ButtonFontSize
        {
            get { return (double)GetValue(ButtonFontSizeDependencyProperty); }
            set { SetValue(ButtonFontSizeDependencyProperty, value); }
        }

        // Click Event
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public RoundButton()
        {
            InitializeComponent();
        }

        private void MouseDown_Button(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                RaiseEvent(new RoutedEventArgs(ClickEvent));
            }
        }
    }
}
