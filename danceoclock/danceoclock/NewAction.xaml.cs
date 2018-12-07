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
using System.Windows.Shapes;
using System.Windows.Forms;

namespace danceoclock
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class NewAction : Window
    {
        MainWindow parent = null;
        public bool isOpen;

        public NewAction(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
            isOpen = true;
            this.Closed += new EventHandler(NewAlarmWindow_Closed);
        }

        private void browsePathButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                dirTextBox.Text = dialog.SelectedPath;
            }
            
        }

        private void recordButton_Click(object sender, RoutedEventArgs e)
        {
            double recordLength = 0;
            double sampleRate = 0;

            Double.TryParse(lengthBox.Text, out recordLength);
            Double.TryParse(rateBox.Text, out sampleRate);

            if (recordLength <= 0 || sampleRate <= 0 || sampleRate > recordLength)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Please make sure that your input is valid: Recording Length and Sample Rate should be positive decimal numbers, and Sample Rate should be less than Recording Length.",
                                      "Input Error",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Error);

            }
            else if (System.IO.Path.GetFileName(fileNameTextBox.Text) == null)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Invalid file name",
                                      "Input Error",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Error);
            }
            else if (System.IO.Path.GetFileName(dirTextBox.Text) == null)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Invalid save directory",
                                      "Input Error",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Error);
            }
            else
            {
                KinectWindow kinectWindow = new KinectWindow(parent, dirTextBox.Text + "\\" + fileNameTextBox.Text + ".txt", sampleRate, recordLength);
                kinectWindow.Show();
                Close();
            }
        }

        void NewAlarmWindow_Closed(object sender, EventArgs e)
        {
            isOpen = false;
        }
    }
}
