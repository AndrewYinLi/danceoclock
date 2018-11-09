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

namespace danceoclock {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        List<Alarm> alarmList = new List<Alarm>();

        public void createNewAlarm(string musicPath, string date, int h, int m, bool isAM, string action) {
            alarmList.Add(new Alarm(musicPath, date, h , m, isAM, action));
            alarmList.Sort((x, y) => x.getChronologicalPriority().CompareTo(y.getChronologicalPriority()));
            alarmListBox.Items.Clear();
            foreach(Alarm alarm in alarmList) {
                alarmListBox.Items.Add(alarm.getFiller());
            }
            
        }

        public MainWindow() {
            InitializeComponent();
            
        }

        private void newAlarmButton_Click(object sender, RoutedEventArgs e) {
            Window newAlarmWindow = new NewAlarmWindow(this);
            newAlarmWindow.Show();
        }

        private void deleteAlarmButton_Click(object sender, RoutedEventArgs e) {
            //System.Diagnostics.Debug.Write(alarmListBox.SelectedIndex);
            
        }

        private void modifyAlarmButton_Click(object sender, RoutedEventArgs e) {

        }

        private void snoozeAlarmButton_Click(object sender, RoutedEventArgs e) {

        }

        private void abortAlarmButton_Click(object sender, RoutedEventArgs e) {

        }
    }
}
