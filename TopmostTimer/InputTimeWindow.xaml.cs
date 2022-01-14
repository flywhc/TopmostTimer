using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TopmostTimer
{
    /// <summary>
    /// Interaction logic for InputTimeWindow.xaml
    /// </summary>
    public partial class InputTimeWindow : Window
    {
        private TimeSpan inputTime;
        public TimeSpan InputTime => inputTime;

        public InputTimeWindow(TimeSpan currentTime)
        {
            inputTime = currentTime;
            InitializeComponent();
            TextSeconds.Text = inputTime.TotalSeconds.ToString();
        }

        private void TypeNumericValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void PasteNumericValidation(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String input = (String)e.DataObject.GetData(typeof(String));
                if (new Regex("[^0-9]+").IsMatch(input))
                {
                    e.CancelCommand();
                }
            }
            else e.CancelCommand();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            inputTime = new TimeSpan(int.Parse(TextSeconds.Text) * 10000*1000);
            Close();
        }
    }
}
