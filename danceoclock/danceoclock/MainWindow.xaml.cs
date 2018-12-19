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
        private List<Alarm> inactiveList = new List<Alarm>();
        static Timer nextAlarm = null;
        int nextAlarmChronologicalPriority = -1;
        string nextAlarmMusicPath = "";
        string nextAlarmActionPath = "";
        int nextAlarmNumrepeats = 0;
        int nextAlarmTolerance = 0;
        int nextAlarmTimeout = 0;
        static DateTime targetTime;
        double minuteTicks = 30000.0;
        bool activeAlarmExists = false;
        // popup windows
        NewAlarmWindow newAlarmWindow;
        NewAlarmWindow newAlarmWindowModify;
        NewAction newAction;
        HelpWindow hw;

        Dictionary<string, int> alarmIndices = new Dictionary<string, int>();

        public void refreshAlarms() {
            alarmList.Sort((x, y) => x.getChronologicalPriority().CompareTo(y.getChronologicalPriority()));
            Dispatcher.Invoke(() =>
            {
                alarmListBox.Items.Clear();
                alarmIndices.Clear();
                for(int i = 0; i < inactiveList.Count; i++) {
                    string filler = "[Inactive] " + inactiveList[i].getFiller();
                    alarmListBox.Items.Add(filler);
                    alarmIndices.Add(filler, i);

                }
                for (int i = 0; i < alarmList.Count; i++) {
                    string filler = "[Active] " + alarmList[i].getFiller();
                    alarmListBox.Items.Add(filler);
                    alarmIndices.Add(filler, i);

                }
            });
            
        }

        public void disableAlarm(int alarmListIndex)
        {
            Console.WriteLine("______________________________");
            StringBuilder sb = new StringBuilder();
            sb.Append("alarmList: ");
            foreach (Alarm alarm in alarmList) sb.Append(alarm.getDebugStr() + " | ");
            Console.WriteLine(sb.ToString());
            Console.WriteLine("TARGET: " + targetTime);
            activeAlarmExists = false;
            inactiveList.Add(alarmList[alarmListIndex]);
            if (nextAlarmChronologicalPriority == alarmList[alarmListIndex].getChronologicalPriority()) {
                nextAlarm.Stop();
            }
            alarmList.RemoveAt(alarmListIndex);
            refreshAlarms();
            forceNextAlarm();
        }

        public void enableAlarm(int alarmListIndex) {
            alarmList.Add(inactiveList[alarmListIndex]);
            inactiveList.RemoveAt(alarmListIndex);
            refreshAlarms();
            forceNextAlarm();
        }

        public void stopNextAlarm() {
            if (nextAlarm != null) nextAlarm.Stop();

        }

        void forceNextAlarm()
        {
            if (alarmList.Count == 0) {
                Console.WriteLine("NO MORE");
                return;
            }
            Alarm mostRecent = alarmList[0];
            nextAlarmChronologicalPriority = mostRecent.getChronologicalPriority();
            nextAlarmMusicPath = mostRecent.musicPath;
            nextAlarmActionPath = mostRecent.actionPath;
            nextAlarmNumrepeats = mostRecent.numrepeats;
            nextAlarmTolerance = mostRecent.tolerance;
            nextAlarmTimeout = mostRecent.timeout;
            activeAlarmExists = true;
            setNextAlarm(mostRecent.year, mostRecent.month, mostRecent.day, mostRecent.armyHour, mostRecent.minute);
        }

        void setNextAlarm(int year, int month, int day, int hour, int minute) {
            //DateTime currentTime = DateTime.Now;
            DateTime targetDate = new DateTime(year, month, day);
            targetTime = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, hour, minute, 0);
            alarmTick();
        }

        void alarmTick() {
            nextAlarm = new Timer(minuteTicks);
            nextAlarm.Elapsed += nextAlarmElapsed;
            nextAlarm.Start();
        }
        
        public bool testAlarmTime(string date, int h, int m, bool isAM)
        {
            string[] dateSplit = date.Split('/');
            int hour = h;
            int month = Int32.Parse(dateSplit[0]);
            int day = Int32.Parse(dateSplit[1]);
            int year = Int32.Parse(dateSplit[2]);
           
            //Console.WriteLine(isAM);
            if (isAM)
            {
                if (hour == 12)
                {
                    hour = 0;
                }
            }
            else
            {
                if (hour != 12)
                {
                    hour = 12 + hour;
                }
            }
            DateTime targetDate = new DateTime(year, month, day);
            DateTime targetTime = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, hour, m, 0);
            DateTime currentTime = DateTime.Now;
            return targetTime > currentTime;
        }
        
        
        void nextAlarmElapsed(object sender, ElapsedEventArgs e) {
            nextAlarm.Stop();
            DateTime currentTime = DateTime.Now;
            if (activeAlarmExists && currentTime.Date.ToString().Equals(targetTime.Date.ToString()) && currentTime.Hour == targetTime.Hour && currentTime.Minute == targetTime.Minute) {
                //Console.WriteLine(targetTime.Hour + ":" + targetTime.Minute);
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    KinectWindow kw = new KinectWindow(this, nextAlarmActionPath, nextAlarmMusicPath, nextAlarmTolerance, nextAlarmTimeout, nextAlarmNumrepeats);
                    kw.Show();
                });
                disableAlarm(0);

            }
            else {
                alarmTick();
            }
            
        }

        public void createNewAlarm(string musicPath, string date, int h, int m, bool isAM, string action, int numrepeats, int tolerance, int timeout) {
            alarmList.Add(new Alarm(musicPath, date, h , m, isAM, action, numrepeats, tolerance, timeout));
            refreshAlarms();
            stopNextAlarm();
            forceNextAlarm();
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

           if (newAlarmWindow == null || !newAlarmWindow.isOpen)
            {
                newAlarmWindow = new NewAlarmWindow(this, null);
                newAlarmWindow.Show();
                newAlarmWindow.isOpen = true;
            }
            else
            {
                newAlarmWindow.Activate();
            }
        }

        private void deleteAlarmButton_Click(object sender, RoutedEventArgs e) {
            if (alarmListBox.SelectedIndex == -1) return;
            string filler = alarmListBox.SelectedItem.ToString();
            if (filler.Substring(1, 1).Equals("A")) {
                if (nextAlarmChronologicalPriority == alarmList[alarmIndices[filler]].getChronologicalPriority()) nextAlarm.Stop();
                alarmList.Remove(alarmList[alarmIndices[filler]]);
            }
            else {
                if (nextAlarmChronologicalPriority == inactiveList[alarmIndices[filler]].getChronologicalPriority()) nextAlarm.Stop();
                inactiveList.Remove(inactiveList[alarmIndices[filler]]);
            }
            refreshAlarms();
            stopNextAlarm();
            forceNextAlarm();
        }

        private void modifyAlarmButton_Click(object sender, RoutedEventArgs e) {

            if (alarmListBox.SelectedIndex == -1) return;

            if (newAlarmWindowModify == null || !newAlarmWindowModify.isOpen)
            {
                string filler = alarmListBox.SelectedItem.ToString();
                if (filler.Substring(1, 1).Equals("A")) {
                    newAlarmWindowModify = new NewAlarmWindow(this, alarmList[alarmIndices[filler]]);
                    if (nextAlarmChronologicalPriority == alarmList[alarmIndices[filler]].getChronologicalPriority()) nextAlarm.Stop();
                    alarmList.Remove(alarmList[alarmIndices[filler]]);
                    newAlarmWindowModify.Title = "Modify Alarm";
                }
                else {
                    newAlarmWindowModify = new NewAlarmWindow(this, inactiveList[alarmIndices[filler]]);
                    if (nextAlarmChronologicalPriority == inactiveList[alarmIndices[filler]].getChronologicalPriority()) nextAlarm.Stop();
                    inactiveList.Remove(inactiveList[alarmIndices[filler]]);
                    newAlarmWindowModify.Title = "Modify Alarm";
                }
                
                newAlarmWindowModify.Show();
                newAlarmWindowModify.isOpen = true;
            }
            else
            {
                newAlarmWindowModify.Activate();
            }
        }

        public bool snooze() {
            if(inactiveList.Count > 0 && inactiveList[inactiveList.Count-1].snoozes++ < 2) {

                inactiveList[inactiveList.Count - 1].minute += 5;
                enableAlarm(inactiveList.Count - 1);
                return true;
            }
            return false;
            
        }

        private void recordActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (newAction == null || !newAction.isOpen)
            {
                newAction = new NewAction(this);
                newAction.Show();
            }
            else
            {
                newAction.Activate();
            }
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            if (hw == null || !hw.isOpen)
            {
                hw = new HelpWindow();
                hw.Show();
            }
            else
            {
                hw.Activate();
            }
        }

        private void ToggleAlarmButton_Click(object sender, RoutedEventArgs e) {
            if (alarmListBox.SelectedIndex == -1) return;
            string filler = alarmListBox.SelectedItem.ToString();
            if (filler.Substring(1, 1).Equals("A")){
                disableAlarm(alarmIndices[filler]);
            }
            else {
                enableAlarm(alarmIndices[filler]);
            }
            

        }
    }
}
