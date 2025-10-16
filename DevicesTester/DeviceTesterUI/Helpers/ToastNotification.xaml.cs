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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DeviceTesterUI.Helpers
{
    /// <summary>
    /// Interaction logic for ToastNotification.xaml
    /// </summary>
    public partial class ToastNotification : UserControl
    {
        private readonly DispatcherTimer _autoHideTimer;

        public ToastNotification()
        {
            InitializeComponent();
            _autoHideTimer = new DispatcherTimer();
            _autoHideTimer.Tick += (s, e) => Hide();

            // Pause timer when mouse is over the toast
            ToastBorder.MouseEnter += (s, e) => _autoHideTimer.Stop();
            ToastBorder.MouseLeave += (s, e) => _autoHideTimer.Start();
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(ToastNotification), new PropertyMetadata(""));

        public Brush TextColor
        {
            get => (Brush)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }
        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register(nameof(TextColor), typeof(Brush), typeof(ToastNotification), new PropertyMetadata(Brushes.Red));

        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(ToastNotification), new PropertyMetadata(Brushes.Red));

        public int DurationSeconds
        {
            get => (int)GetValue(DurationSecondsProperty);
            set => SetValue(DurationSecondsProperty, value);
        }
        public static readonly DependencyProperty DurationSecondsProperty =
            DependencyProperty.Register(nameof(DurationSeconds), typeof(int), typeof(ToastNotification), new PropertyMetadata(2));

        public void Show(string message, Brush? color = null)
        {
            Message = message;
            if (color != null)
            {
                TextColor = color;
                BorderBrush = color;
            }

            Visibility = Visibility.Visible;

            // Reset timer
            _autoHideTimer.Stop();
            _autoHideTimer.Interval = TimeSpan.FromSeconds(DurationSeconds);

            // Check if mouse is already over the toast
            if (ToastBorder.IsMouseOver)
            {
                // Don't start timer if user is hovering
                // Timer will start on MouseLeave
            }
            else
            {
                _autoHideTimer.Start();
            }

            // Fade-in animation
            var fadeIn = (Storyboard)Resources["FadeIn"];
            fadeIn.Begin();
        }


        public void Hide()
        {
            _autoHideTimer.Stop();
            if (Resources["FadeOut"] is Storyboard fadeOut)
            {
                fadeOut.Completed += (s, e) =>
                {
                    Visibility = Visibility.Collapsed;
                    ToastBorder.Opacity = 0;
                };

                fadeOut.Begin();
            }
            else
            {
                // Fallback: if storyboard missing, just collapse
                Visibility = Visibility.Collapsed;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Hide();
    }
}
