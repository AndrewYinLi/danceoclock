using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.Kinect;
using System.IO;
using System.Threading.Tasks;
using WMPLib;

namespace danceoclock
{

    public partial class KinectWindow : Window
    {
        // set up kinect
        KinectSensor Sensor;
        MultiSourceFrameReader Reader;
        public IList<Body> Bodies; // list of bodies detected
        public  Gesture newGesture = null;

        // music player
        WindowsMediaPlayer Player;

        // tolerance of movement/angle matches in %
        public double Tolerance;

        // maximum number of frames before timeout for each key frame
        public double Timeout;

        // number of repeats for each movement
        public int Numrepeats;
        public int currentRep = 0;

        // dictionary of available gestures - Gesture.body should be null initially -
        // PUT IN UI MAIN, but keep a copy here (load from UI main via constructor)
        public Dictionary<string, Gesture> Gestures; // save/load these in txt file

        // counter for frames
        public int CurrentNumFrames = 0;

        // Number of seconds to take new frame
        public double Freq = 0;

        // How long gesture should be
        public int Length = 0;

        // Frames left to record in the gesture
        public int FramesLeft = 0;

        // mode - either to record new gestures (true) or activate the alarm (false)
        public bool Recording = true;

        public Gesture currentGesture = null;
        public int currentFrameInd = 0;
        public int Iterations = 0;

        // list of joints for highlighting incorrect joints
        public List<JointType> joints = null;

        // ptr to parent (Andrew's UI)
        MainWindow parent = null;
        string path = null;


        // constructor for recording mode
        public KinectWindow(MainWindow parent, string path, double sec, int length)
        {
            this.parent = parent;
            this.path = path;
            InitializeComponent();
            Freq = sec;
            Length = length;
            FramesLeft = (int) Math.Ceiling(Length / Freq);
        }


        // constructor for alarm mode, gestNamesList (list of names of all gestures used in order) comes from UI main
        public KinectWindow(MainWindow parent, string gesturePath, string musicPath, double tolerance, double timeout, int numrepeats)
        {
            this.parent = parent;
            Recording = false;

            Tolerance = tolerance;
            Timeout = timeout;
            Numrepeats = numrepeats;

            Gesture gesture = new Gesture();
            string[] lines = File.ReadAllLines(gesturePath);
            foreach(string line in lines)
            {
                string[] arr = line.Split(' ');
                List<double> anglesList = new List<double>();
                List<double> coordsList = new List<double>();

                int i = 0;
                foreach(string angle in arr)
                {
                    if (i < 10) { anglesList.Add(double.Parse(angle)); }
                    else { coordsList.Add(double.Parse(angle)); }
                    i++;                    
                }
                KeyFrame frame = new KeyFrame(anglesList, coordsList);
                gesture.Keyframes.Add(frame);
            }
            currentGesture = gesture;
            InitializeComponent();

            Player = new WindowsMediaPlayer();
            Player.PlayStateChange +=
                new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
            Player.URL = musicPath;

            Player.controls.play();
        }

        private void Player_PlayStateChange(int currentState)
        {
            if ((WMPPlayState)currentState == WMPPlayState.wmppsStopped)
            {
                Player.controls.play();
            }
        }

        // when loading window, set up sensor
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Sensor = KinectSensor.GetDefault();

            if (Sensor != null)
            {
                Sensor.Open();

                Reader = Sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                Reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }
        }

        private void snoozeAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            parent.snooze();
            Close();
        }

        // when closing window
        private void Window_Closed(object sender, EventArgs e)
        {
            if (Reader != null)
            {
                Reader.Dispose();
            }

            if (Sensor != null)
            {
                Sensor.Close();
            }

            Player.close();
        }

        /*.
        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
        */

        // method for reading information from the sensor
        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();
        
            // Displaying the camera feed via color frame
            // comment out this part to only display skeleton for displaying movement instructions
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    camera.Source = frame.ToBitmap();
                }
            }

            // Displaying the skeleton via the body frame
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    canvas.Children.Clear();
                    
                    Bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(Bodies);

                    foreach (var body in Bodies)
                    {
                        if(newGesture == null)
                        {
                            newGesture = new Gesture(body);
                        }
                        if (body != null)
                        {
                            if (body.IsTracked)
                            {
                                canvas.DrawSkeleton(body, Colors.DarkBlue);
                                /*
                                if (frameMessageLabel.Visibility != Visibility.Hidden)
                                {
                                    frameMessageLabel.Visibility = Visibility.Hidden;
                                    Refresh(frameMessageLabel);
                                }
                                */
                                List<double> settings = NextFrame(body);

                                // recording mode
                                if (Recording)
                                {
                                    
                                    // how many frames has passed, compare to frequency*fps, we want to record once every freq*fps
                                    if (++CurrentNumFrames == (int) Math.Ceiling(Freq * 30))
                                    {
                                        // record take one frame, so decrement number of frames left to record
                                        if ((--FramesLeft) >= 0)
                                        {
                                            KeyFrame newFrame = new KeyFrame(body, settings);

                                            newFrame.Coords = new List<double>();
                                            newFrame.Coords.Add(body.Joints[JointType.Head].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.Head].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
                                            newFrame.Coords.Add(body.Joints[JointType.Neck].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.Neck].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
                                            newFrame.Coords.Add(body.Joints[JointType.ShoulderLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.ShoulderLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
                                            newFrame.Coords.Add(body.Joints[JointType.ElbowLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.ElbowLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
                                            newFrame.Coords.Add(body.Joints[JointType.ShoulderRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.ShoulderRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
                                            newFrame.Coords.Add(body.Joints[JointType.ElbowRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.ElbowRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
                                            newFrame.Coords.Add(body.Joints[JointType.WristLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.WristLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
                                            newFrame.Coords.Add(body.Joints[JointType.WristRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.WristRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
                                            newFrame.Coords.Add(body.Joints[JointType.SpineBase].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.SpineBase].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
                                            newFrame.Coords.Add(body.Joints[JointType.HipLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.HipLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
                                            newFrame.Coords.Add(body.Joints[JointType.HipRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.HipRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
                                            newFrame.Coords.Add(body.Joints[JointType.KneeLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.KneeLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
                                            newFrame.Coords.Add(body.Joints[JointType.KneeRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.KneeRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
                                            newFrame.Coords.Add(body.Joints[JointType.AnkleLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.AnkleLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
                                            newFrame.Coords.Add(body.Joints[JointType.AnkleRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
                                            newFrame.Coords.Add(body.Joints[JointType.AnkleRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);

                                            newGesture.AddKeyframe(newFrame);
                                        }
                                        else
                                        {
                                            // Pass newGesture into ui main window
                                            parent.writeGesture(newGesture, path);
                                            Close();
                                        }

                                        // recount frames that have passed
                                        CurrentNumFrames = 0;
                                    }

                                }
                                else // alarm mode
                                {
                                    // populate joints list
                                    populateJoints();

                                    currentGesture.setBody(body); // use the right body instance

                                    // loop through numrepeats
                                    if (currentRep == Numrepeats) { Close(); }

                                    // match the current keyframe
                                    KeyFrame currentFrame = currentGesture.Keyframes[currentFrameInd];

                                    // show reference skeleton
                                    canvas.DrawRefSkeleton(currentFrame.Coords, Colors.Purple);

                                    // whether or not all the angles match
                                    bool AllMatch = true;
                                    StringBuilder sb = new StringBuilder();

                                    // current angles
                                    List<double> Current = NextFrame(body);

                                    // loop through matching angles to compare them
                                    for (int i = 0; i < 10; i++)
                                    {
                                        // if the angle is not within the tolerated range
                                        if (!(Math.Abs(Current[i] - currentFrame.Angles[i]) <= Tolerance))
                                        {
                                            //highlight incorrect joints
                                            canvas.DrawPoint(body.Joints[joints[i]], Colors.Red);

                                            AllMatch = false;
                                            // break; // don't have to check anymore
                                        }
                                    }

                                    if (AllMatch)
                                    {
                                        // if all angles match
                                        Iterations = 0;
                                        currentFrameInd++;
                                    }

                                    // after looping through all keyframes, go on to next rep
                                    if (currentFrameInd == currentGesture.Keyframes.Count()) {
                                        currentFrameInd = 0;
                                        currentRep++;
                                    }
                                }
                            }
                            else
                            {
                                /*
                                if(frameMessageLabel.Visibility != Visibility.Visible)
                                {
                                    frameMessageLabel.Visibility = Visibility.Visible;
                                    Refresh(frameMessageLabel);
                                }
                                */
                            }
                        }
                    }
                }
            }
        }

        // method for calculating the dot product of 2 vectors
        private static double Dot(double x1, double y1, double x2, double y2)
        {
            return x1 * x2 + y1 * y2;
        }

        // method for calculating angle between two vectors
        private static double Angle(double x1, double y1, double x2, double y2)
        {
            // check for zero vector
            if ((x1 == 0 && y1 == 0) || (x2 == 0 && y2 == 0))
            {
                return 0;
            }
            else
            {
                double theta = Math.Round((180 / Math.PI) * Math.Acos((double)Dot(x1, y1, x2, y2) /
                    (double)(Math.Sqrt(Math.Pow(x1, 2) + Math.Pow(y1, 2)) *
                    (Math.Sqrt(Math.Pow(x2, 2) + Math.Pow(y2, 2))))));

                // check for obtuse angle
                if (Dot(x1, y1, x2, y2) < 0)
                {
                    theta = 360 - theta;
                }

                return theta;
            }
        }

        // method for taking 3 joints and calculating the angle between them
        private static double JointsAngle(Joint a, Joint b, Joint c)
        {
            // first vector b->a
            double x1 = a.Position.X - b.Position.X;
            double y1 = a.Position.Y - b.Position.Y;

            // second vector b->c
            double x2 = c.Position.X - b.Position.X;
            double y2 = c.Position.Y - b.Position.Y;

            // return angle
            return Angle(x1, y1, x2, y2);
        }

        // collect information for the next frame
        public static List<double> NextFrame(Body body)
        {
            List<double> CurrentAngles = new List<double>();

            // calculate and add angles
            
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.Head], body.Joints[JointType.Neck], body.Joints[JointType.ShoulderLeft]));

            // shoulderleftangle

            CurrentAngles.Add(JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft]));

            // shoulderrightangle
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight]));

            // elbowleftangle
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft]));

            // elbowrightangle
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight]));

            // spineangle
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.SpineBase], body.Joints[JointType.HipLeft]));

            // hipleftangle
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.KneeLeft], body.Joints[JointType.HipLeft], body.Joints[JointType.SpineBase]));

            // hiprightangle
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.KneeRight], body.Joints[JointType.HipRight], body.Joints[JointType.SpineBase]));

            // kneeleftangle 
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.HipLeft], body.Joints[JointType.KneeLeft], body.Joints[JointType.AnkleLeft]));

            // kneerightangle
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.HipRight], body.Joints[JointType.KneeRight], body.Joints[JointType.AnkleRight]));

            return CurrentAngles;
        }

        public void populateJoints()
        {
            joints = new List<JointType>();

            joints.Add(JointType.Neck);
            joints.Add(JointType.ShoulderLeft);
            joints.Add(JointType.ShoulderRight);
            joints.Add(JointType.ElbowLeft);
            joints.Add(JointType.ElbowRight);
            joints.Add(JointType.SpineBase);
            joints.Add(JointType.HipLeft);
            joints.Add(JointType.HipRight);
            joints.Add(JointType.KneeLeft);
            joints.Add(JointType.KneeRight);
        }
    }
}
