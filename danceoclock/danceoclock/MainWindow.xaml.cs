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
using System.Timers;
using System.Media;
using System.IO;
using Microsoft.Kinect;

namespace danceoclock {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private List<Alarm> alarmList = new List<Alarm>();
        Timer nextAlarm;
        int nextAlarmChronologicalPriority = -1;
        string nextAlarmMusicPath = "";
        string nextAlarmActionPath = "";

        public void refreshAlarms() {
            alarmList.Sort((x, y) => x.getChronologicalPriority().CompareTo(y.getChronologicalPriority()));
            alarmListBox.Items.Clear();
            foreach (Alarm alarm in alarmList) {
                alarmListBox.Items.Add(alarm.getFiller());
            }
            
        }

        void checkNextAlarm() {
            if (alarmList.Count == 0) return;
            Alarm mostRecent = alarmList[0];
            if(nextAlarmChronologicalPriority == -1 || mostRecent.getChronologicalPriority() < nextAlarmChronologicalPriority)
            {
                setNextAlarm(mostRecent.year, mostRecent.month, mostRecent.day, mostRecent.armyHour, mostRecent.minute);
                nextAlarmChronologicalPriority = mostRecent.getChronologicalPriority();
                nextAlarmMusicPath = mostRecent.musicPath;
                nextAlarmActionPath = mostRecent.actionPath;
            }
        }

        void setNextAlarm(int year, int month, int day, int hour, int minute) {
            DateTime currentTime = DateTime.Now;
            DateTime targetDate = new DateTime(year, month, day);
            DateTime targetTime = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, hour, minute, 0);

            double tickTime = (targetTime - currentTime).TotalMilliseconds;
            nextAlarm = new Timer(tickTime);
            nextAlarm.Elapsed += nextAlarmElapsed;
            nextAlarm.Start();
        }

        
        void nextAlarmElapsed(object sender, ElapsedEventArgs e) {
            nextAlarm.Stop();
            Application.Current.Dispatcher.Invoke((Action)delegate {
                KinectWindow kw = new KinectWindow(nextAlarmActionPath, nextAlarmMusicPath, 10, 60, 2);
                kw.Show();
            });
            checkNextAlarm();
        }

        public void createNewAlarm(string musicPath, string date, int h, int m, bool isAM, string action) {
            alarmList.Add(new Alarm(musicPath, date, h , m, isAM, action));
            refreshAlarms();
            checkNextAlarm();
        }

        public void writeGesture(Gesture gesture, string path)
        {
            //StreamWriter f = new StreamWriter(path, true); // The overload with a true bool appends to file instead of overwriting
            StreamWriter f = new StreamWriter(path);
            foreach(KeyFrame frame in gesture.Keyframes)
            {
                StringBuilder sb = new StringBuilder();
                foreach(double angle in frame.Angles)
                {
                    sb.Append(angle);
                    sb.Append(" ");
                }

                foreach (double coords in frame.Coords)
                {
                    sb.Append(coords + " ");
                }

                sb.Remove(sb.Length-1, 1); // trim space
                f.WriteLine(sb.ToString());
            }
            f.Close();
        }


        public MainWindow() {
            InitializeComponent();
        }

        private void newAlarmButton_Click(object sender, RoutedEventArgs e) {
            Window newAlarmWindow = new NewAlarmWindow(this, null);
            newAlarmWindow.Show();
        }

        private void deleteAlarmButton_Click(object sender, RoutedEventArgs e) {
            if (alarmListBox.SelectedIndex != -1)
            {
                alarmList.Remove(alarmList[alarmListBox.SelectedIndex]);
                refreshAlarms();
                checkNextAlarm();
            }
            
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
                checkNextAlarm();
            }
            
        }

        private void recordActionButton_Click(object sender, RoutedEventArgs e)
        {
            NewAction newAction = new NewAction(this);
            newAction.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //string gesturePath, double tolerance, double timeout, int numrepeats
            KinectWindow kw = new KinectWindow("C:\\Users\\shanali\\Desktop\\penis.txt", "C:\\Users\\shanali\\Desktop\\Li_MysteryDungeon_Scene.mp3", 10, 60, 2);
            kw.Show();
        }
    }
}
