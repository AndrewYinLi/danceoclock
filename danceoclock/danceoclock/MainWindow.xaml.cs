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

        private List<Alarm> alarmList = new List<Alarm>();
        
        public void refreshAlarms() {
            alarmList.Sort((x, y) => x.getChronologicalPriority().CompareTo(y.getChronologicalPriority()));
            alarmListBox.Items.Clear();
            foreach (Alarm alarm in alarmList) {
                alarmListBox.Items.Add(alarm.getFiller());
            }
        }

        public void createNewAlarm(string musicPath, string date, int h, int m, bool isAM, string action) {
            alarmList.Add(new Alarm(musicPath, date, h , m, isAM, action));
            refreshAlarms();
            
        }

        public MainWindow() {
            InitializeComponent();
            
        }

        private void newAlarmButton_Click(object sender, RoutedEventArgs e) {
            Window newAlarmWindow = new NewAlarmWindow(this, null);
            newAlarmWindow.Show();
        }

        private void deleteAlarmButton_Click(object sender, RoutedEventArgs e) {
            alarmList.Remove(alarmList[alarmListBox.SelectedIndex]);
            refreshAlarms();
            
        }

        private void modifyAlarmButton_Click(object sender, RoutedEventArgs e) {
            Window newAlarmWindow = new NewAlarmWindow(this, alarmList[alarmListBox.SelectedIndex]);
            newAlarmWindow.Title = "Modify Alarm";
            alarmList.Remove(alarmList[alarmListBox.SelectedIndex]);
            newAlarmWindow.Show();
        }

        private void snoozeAlarmButton_Click(object sender, RoutedEventArgs e) {
            if(alarmList[alarmListBox.SelectedIndex].snoozes++ < 2) {
                alarmList[alarmListBox.SelectedIndex].minute += 5;
                refreshAlarms();
            }
            
        }

        private void abortAlarmButton_Click(object sender, RoutedEventArgs e) {
            alarmList.Remove(alarmList[alarmListBox.SelectedIndex]);
            refreshAlarms();
        }
    }
}
