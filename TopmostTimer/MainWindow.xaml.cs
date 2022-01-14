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
        private double _aspectRatio;
        private SizeChangedInfo? _sizeInfo;

        private TimeSpan TargetTime = new TimeSpan(60*10000*1000);

        public MainWindow()
        {
            InitializeComponent();
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
            CurrentTime.Content = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", diff.TotalHours, diff.Minutes, diff.Seconds, diff.Milliseconds / 10);

            if (diff > TargetTime)
            {
                CurrentTime.Foreground = Brushes.Red;
            }
            else
            {
                CurrentTime.Foreground = Brushes.Black;
            }
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
            CurrentTime.Content = "00:00:00.00";
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
        }
    }
}