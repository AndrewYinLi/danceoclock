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

namespace danceoclock {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 

    public partial class NewAlarmWindow : Window {

        private MainWindow parent;
        private Alarm oldAlarm;
        public bool isOpen;

        public NewAlarmWindow(MainWindow parent, Alarm oldAlarm) {
            InitializeComponent();
            alarmDatePicker.SelectedDate = DateTime.Today;
            musicPathTextBox.TextWrapping = TextWrapping.NoWrap;
            this.parent = parent;
            this.oldAlarm = oldAlarm;
            if (oldAlarm != null) {
                musicPathTextBox.Text = oldAlarm.musicPath; 
                alarmDatePicker.SelectedDate = new DateTime(oldAlarm.year, oldAlarm.month, oldAlarm.day);
                hoursTextBox.Text = oldAlarm.hour+"";
                minutesTextBox.Text = oldAlarm.minute+"";
                if (!oldAlarm.isAM) { 
                    amButton.IsChecked = false;
                    pmButton.IsChecked = true;
                }
                actionTextBox.Text = oldAlarm.actionPath;
            }

            this.Closed += new EventHandler(NewAlarmWindow_Closed);

        }

        private void browseMusicButton_Click(object sender, RoutedEventArgs e) {
            browseAndSet(musicPathTextBox);
        }

        private void browseActionTextBox_Click(object sender, RoutedEventArgs e) {
            browseAndSet(actionTextBox);
        }

        private void browseAndSet(TextBox listBoxToSet) {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true) {
                listBoxToSet.Text = dlg.FileName;
            }
        }

        private void createAlarmButton_Click(object sender, RoutedEventArgs e) {
            try
            {
                // validate audio file
                if (!string.Equals(musicPathTextBox.Text.Substring(Math.Max(0, musicPathTextBox.Text.Length - 4)), ".mp3"))
                {
                    MessageBoxResult result = MessageBox.Show("Invalid music file.",
                                                              "File Error",
                                                              MessageBoxButton.OK,
                                                              MessageBoxImage.Error);
                }

                // validate action file
                else if (!string.Equals(actionTextBox.Text.Substring(Math.Max(0, actionTextBox.Text.Length - 4)), ".txt"))
                {
                    MessageBoxResult result = MessageBox.Show("Invalid action file.",
                                                              "File Error",
                                                              MessageBoxButton.OK,
                                                              MessageBoxImage.Error);
                }

                // validate numbers
                int numrepeats, tolerance, timeout = 0;

                Int32.TryParse(RepBox.Text, out numrepeats);
                Int32.TryParse(ToleranceBox.Text, out tolerance);
                Int32.TryParse(MaxtimeBox.Text, out timeout);

                if (numrepeats <= 0)
                {
                    MessageBoxResult result = MessageBox.Show("Repetitions must be a positive integer.",
                                          "Input Error",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
                }

                else if (tolerance <= 0 || tolerance >= 360)
                {
                    MessageBoxResult result = MessageBox.Show("Tolerance must be an integer angle greater than 0 and less than 360.",
                                          "Input Error",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
                }

                else if (timeout <= 0)
                {
                    MessageBoxResult result = MessageBox.Show("Maximum Frame Time must be a positive integer.",
                                          "Input Error",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
                }

                else
                {
                    parent.createNewAlarm(musicPathTextBox.Text, alarmDatePicker.DisplayDate.ToString().Split(' ')[0], Int32.Parse(hoursTextBox.Text), Int32.Parse(minutesTextBox.Text), 
                        (amButton.IsChecked == true) ? true : false, actionTextBox.Text, numrepeats, tolerance, timeout * 30);
                    if (oldAlarm != null) parent.refreshAlarms();
                    isOpen = false;
                    Close();
                }

            } catch (System.FormatException) // TODO fix time formatting settings
            {
                MessageBoxResult result = MessageBox.Show("Please set a valid time.",
                                          "Input Error",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
            }
        }

        private void hoursTextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (hoursTextBox.Text.Equals("h"))
            {
                hoursTextBox.Text = "";
            }
        }

        private void minutesTextBox_MousEnter(object sender, MouseEventArgs e)
        {
            if (minutesTextBox.Text.Equals("m"))
            {
                minutesTextBox.Text = "";
            }
        }

        void NewAlarmWindow_Closed(object sender, EventArgs e)
        {
            isOpen = false;
        }
    }
}
