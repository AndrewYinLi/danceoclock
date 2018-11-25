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
        
        public void refreshAlarms() {
            alarmList.Sort((x, y) => x.getChronologicalPriority().CompareTo(y.getChronologicalPriority()));
            alarmListBox.Items.Clear();
            foreach (Alarm alarm in alarmList) {
                alarmListBox.Items.Add(alarm.getFiller());
            }
            
        }

        /* This function was taken from: https://stackoverflow.com/questions/21299214/how-to-set-timer-to-execute-at-specific-time-in-c-sharp */
        Timer timer;
        void setup_Timer() {
            DateTime currentTime = DateTime.Now;
            DateTime targetTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 2, 47, 0);
            if (currentTime > targetTime)
                targetTime = targetTime.AddDays(1);

            double tickTime = (targetTime - currentTime).TotalMilliseconds;
            timer = new Timer(tickTime);
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        
        void timer_Elapsed(object sender, ElapsedEventArgs e) {
            timer.Stop();

            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = "C:\\Users\\andre\\Downloads\\sample.wav";
            player.Play();
        }

        public void createNewAlarm(string musicPath, string date, int h, int m, bool isAM, string action) {
            alarmList.Add(new Alarm(musicPath, date, h , m, isAM, action));
            refreshAlarms();
            
        }

        public void writeGesture(Gesture gesture, string path)
        {
            //StreamWriter f = new StreamWriter(path, true); // The overload with a true bool appends to file instead of overwriting
            StreamWriter f = new StreamWriter(path);
            // how write body???????
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
                

                /*Body body = frame.Body;
                sb.Append(body.Joints[JointType.Head].Position.X + " ");
                sb.Append(body.Joints[JointType.Head].Position.Y + " ");
                sb.Append(body.Joints[JointType.Neck].Position.X + " ");
                sb.Append(body.Joints[JointType.Neck].Position.Y + " ");
                sb.Append(body.Joints[JointType.ShoulderLeft].Position.X + " ");
                sb.Append(body.Joints[JointType.ShoulderLeft].Position.Y + " ");
                sb.Append(body.Joints[JointType.ElbowLeft].Position.X + " ");
                sb.Append(body.Joints[JointType.ElbowLeft].Position.Y + " ");
                sb.Append(body.Joints[JointType.ShoulderRight].Position.X + " ");
                sb.Append(body.Joints[JointType.ShoulderRight].Position.Y + " ");
                sb.Append(body.Joints[JointType.ElbowRight].Position.X + " ");
                sb.Append(body.Joints[JointType.ElbowRight].Position.Y + " ");
                sb.Append(body.Joints[JointType.WristLeft].Position.X + " ");
                sb.Append(body.Joints[JointType.WristLeft].Position.Y + " ");
                sb.Append(body.Joints[JointType.WristRight].Position.X + " ");
                sb.Append(body.Joints[JointType.WristRight].Position.Y + " ");
                sb.Append(body.Joints[JointType.SpineBase].Position.X + " ");
                sb.Append(body.Joints[JointType.SpineBase].Position.Y + " ");
                sb.Append(body.Joints[JointType.HipLeft].Position.X + " ");
                sb.Append(body.Joints[JointType.HipLeft].Position.Y + " ");
                sb.Append(body.Joints[JointType.HipRight].Position.X + " ");
                sb.Append(body.Joints[JointType.HipRight].Position.Y + " ");
                sb.Append(body.Joints[JointType.KneeLeft].Position.X + " ");
                sb.Append(body.Joints[JointType.KneeLeft].Position.Y + " ");
                sb.Append(body.Joints[JointType.KneeRight].Position.X + " ");
                sb.Append(body.Joints[JointType.KneeRight].Position.Y + " ");
                sb.Append(body.Joints[JointType.AnkleLeft].Position.X + " ");
                sb.Append(body.Joints[JointType.AnkleLeft].Position.Y + " ");
                sb.Append(body.Joints[JointType.AnkleRight].Position.X + " ");
                sb.Append(body.Joints[JointType.AnkleRight].Position.Y + " ");*/

                sb.Remove(sb.Length-1, 1); // trim space
                f.WriteLine(sb.ToString());
            }
            f.Close();
        }


        public MainWindow() {
            InitializeComponent();
            setup_Timer();
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
            if (alarmListBox.SelectedIndex != -1)
            {
                alarmList.Remove(alarmList[alarmListBox.SelectedIndex]);
                refreshAlarms();
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
            KinectWindow kw = new KinectWindow("C:\\Users\\shanali\\Desktop\\penis.txt", 30, 60, 1);
            kw.Show();
        }
    }
}
