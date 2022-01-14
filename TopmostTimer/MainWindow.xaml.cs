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
        private DateTime startTime;
        private bool isRunning = false;

        public MainWindow()
        {
            InitializeComponent();
            this.Topmost = true;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(10000);
            timer.Tick += Timer_Tick;
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            TimeSpan diff = DateTime.Now - startTime;
            CurrentTime.Content = string.Format("{0:00}:{1:00}", diff.Minutes, diff.Seconds);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                StartButton.Content = "Start";
                timer.Stop();
            }
            else
            {
                StartButton.Content = "Stop";
                startTime= DateTime.Now;
                timer.Start();
            }
            isRunning = !isRunning;
        }
    }
}
