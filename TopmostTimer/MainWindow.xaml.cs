using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace TopmostTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private DispatcherTimer resizeTimer;
        private DateTime startTime;
        private TimeSpan baseTimeDiff = new(0);
        private bool isRunning = false;
        private readonly double _aspectRatio;
        private SizeChangedInfo? _sizeInfo;
        private TimeSpan TargetTime = new(60*10000*1000);

        public MainWindow()
        {
            Topmost = true;
            // 保持窗口宽高比例，当窗口尺寸改变后延迟0.1秒纠正窗口比例
            _aspectRatio = Width / Height;
            resizeTimer = new DispatcherTimer();
            resizeTimer.Interval = new TimeSpan(100 * 10000); // 0.1 seconds
            resizeTimer.Tick += ResizeTimer_Tick;

            // 刷新时间显示的timer
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(10000); // 1ms
            timer.Tick += Timer_Tick;

            InitializeComponent();
        }

        // 纠正窗口比例
        private void ResizeTimer_Tick(object? sender, EventArgs e)
        {
            resizeTimer.Stop();
            if (_sizeInfo == null) return;
            var percentWidthChange = Math.Abs(_sizeInfo.NewSize.Width - _sizeInfo.PreviousSize.Width) / _sizeInfo.PreviousSize.Width;
            var percentHeightChange = Math.Abs(_sizeInfo.NewSize.Height - _sizeInfo.PreviousSize.Height) / _sizeInfo.PreviousSize.Height;

            if (percentWidthChange > percentHeightChange)
                this.Height = _sizeInfo.NewSize.Width / _aspectRatio;
            else
                this.Width = _sizeInfo.NewSize.Height * _aspectRatio;
        }

        // 当窗口改变大小后触发，延迟resizeTimer的时间后再纠正窗口比例
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            _sizeInfo = sizeInfo;
            resizeTimer.Stop();
            resizeTimer.Start();
            base.OnRenderSizeChanged(sizeInfo);
        }

        // for dragging the window
        public void DragWindow(object sender, MouseButtonEventArgs args)
        {
            DragMove();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            TimeSpan diff = DateTime.Now - startTime;
            diff += baseTimeDiff;

            if (IsCountDown)
            {
                var remainingTime = TargetTime - diff;
                CurrentTime.Content = TimeSpanToString(remainingTime);
                if (remainingTime.Ticks < 0)
                {
                    CurrentTime.Foreground = Brushes.Red;
                }
                else
                {
                    CurrentTime.Foreground = Brushes.Black;
                }
            }
            else
            {
                CurrentTime.Content = TimeSpanToString(diff);
                if (diff > TargetTime)
                {
                    CurrentTime.Foreground = Brushes.Red;
                }
                else
                {
                    CurrentTime.Foreground = Brushes.Black;
                }
            }
        }

        private static string TimeSpanToString(TimeSpan time)
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}", time.TotalHours, Math.Abs(time.Minutes), Math.Abs(time.Seconds), Math.Abs(time.Milliseconds / 10));
        }

        private void StartPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                timer.Stop();
                TimeSpan diff = DateTime.Now - startTime;
                baseTimeDiff += diff;
            }
            else
            {
                startTime = DateTime.Now;
                timer.Start();
            }
            isRunning = !isRunning;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            baseTimeDiff = new(0);
            timer.Stop();
            isRunning = false;
            if (IsCountDown)
            {
                CurrentTime.Content = TimeSpanToString(TargetTime);
            }
            else
            {
                CurrentTime.Content = "00:00:00.00";
            }
            CurrentTime.Foreground = Brushes.Black;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TargetButton_Click(object sender, RoutedEventArgs e)
        {
            Topmost = false;
            var inputWindow = new InputTimeWindow(TargetTime);
            inputWindow.ShowDialog();
            TargetTime = inputWindow.InputTime;
            Topmost = true;

            StopButton_Click(sender, e);
        }

        private bool IsCountDown => CountDownButton==null? false: CountDownButton.IsChecked != null && CountDownButton.IsChecked== true;

        private void CountDownButton_Checked(object sender, RoutedEventArgs e)
        {
            StopButton_Click(sender,e);
        }

        private void TimerButton_Checked(object sender, RoutedEventArgs e)
        {
            if (CurrentTime != null)
            {
                StopButton_Click(sender, e);
            }
        }
    }
}