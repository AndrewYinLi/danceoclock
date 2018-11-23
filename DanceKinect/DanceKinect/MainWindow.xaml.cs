using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace DanceKinect
{

    public partial class MainWindow : Window
    {
        // set up kinect
        KinectSensor Sensor;
        MultiSourceFrameReader Reader;
        public static IList<Body> Bodies; // list of bodies detected
        public static Gesture newGesture = null;

        // tolerance of movement/angle matches in %
        public static double Tolerance;

        // maximum number of frames before timeout for each key frame
        public static double Timeout;

        // number of repeats for each movement
        public static double Numrepeats;

        // dictionary of available gestures - Gesture.body should be null initially -
        // PUT IN UI MAIN, but keep a copy here (load from UI main via constructor)
        public static Dictionary<string, Gesture> Gestures; // save/load these in txt file

        // counter for frames
        public static int CurrentNumFrames = 0;

        // Number of seconds to take new frame
        public static int Sec = 0;

        // How long gesture should be
        public static int Length = 0;

        // Frames left to count
        public static int FramesLeft = 0;

        // mode - either to record new gestures (true) or activate the alarm (false)
        public static bool Recording = true;

        // for alarm mode - list of names of all gestures used in order
        List<string> gestNamesList = null;

        // for alarm mode - list of all gestures used in order
        List<Gesture> gestList = null;

        // constructor for recording mode
        public MainWindow()
        {
            loadDict();
            InitializeComponent();

            // put these in constructor as params
            Tolerance = 0.5;
            Timeout = 100;
            Numrepeats = 1;

            InitRecording();
        }


        // constructor for alarm mode, gestNamesList (list of names of all gestures used in order) comes from UI main
        public MainWindow(List<string> gestNamesList)
        {
            Recording = false;
            loadDict();

            this.gestNamesList = gestNamesList; // gesture name list - create gestList after body is determined
            gestList = new List<Gesture>(); // make new gesture list for the used gestures

            // populate gestList
            // go through gestNamesList, get values from Gestures, put found Gesture in gestList
            foreach (string gestName in gestNamesList)
            {
                if (Gestures.TryGetValue(gestName, out Gesture gest))
                {
                    gestList.Add(gest);
                }
            }

            InitializeComponent();

            // put these in constructor as params
            Tolerance = 0.5;
            Timeout = 100;
            Numrepeats = 1;
        }

        // load dictionary of all gestures from txt - perhaps do this in the UI main
        public void loadDict()
        {
            Gestures = new Dictionary<string, Gesture>(); // dictionary of all gestures - load from txt
        }

        // initialize the functionality for record a new alarm gesture, put these in constructor?
        public void InitRecording()
        {
            CurrentNumFrames = 0;
            Sec = 1;
            Length = 2;
            FramesLeft = 30 * Length;
        }

        // initialize the alarm display for alarm deactivation
        public void InitAlarmDisplay()
        {

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
        }

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
                                canvas.DrawSkeleton(body);
                                List<double> settings = NextFrame(body);

                                // recording mode
                                if (Recording)
                                {
                                    // If 1 second has passed
                                    if (++CurrentNumFrames == Sec * 30)
                                    {
                                        if ((FramesLeft -= CurrentNumFrames) >= 0)
                                        {
                                            Gesture.AddKeyframe(new KeyFrame(body, settings));
                                        }
                                        else
                                        {
                                            // Pass newGesture into Main UI window
                                            // exit
                                        }
                                        CurrentNumFrames = 0;
                                    }

                                } else // alarm mode
                                {
                                    // loop through gestList, match keyframes for number of repetitions
                                    foreach (Gesture gest in gestList)
                                    {
                                        gest.setBody(body); // use the right body instance
                                        gest.Repeat();
                                    }
                                }
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

            Console.WriteLine("neckangle" + JointsAngle(body.Joints[JointType.Head], body.Joints[JointType.Neck], body.Joints[JointType.ShoulderLeft]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.Head], body.Joints[JointType.Neck], body.Joints[JointType.ShoulderLeft]));

            // shoulderleftangle
            Console.WriteLine("shoulderleftangle" + JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft]));

            // shoulderrightangle
            Console.WriteLine("shoulderrightangle" + JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight]));

            // elbowleftangle
            Console.WriteLine("elbowleftangle" + JointsAngle(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft]));

            // elbowrightangle
            Console.WriteLine("elbowrightangle" + JointsAngle(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight]));

            // spineangle
            Console.WriteLine("spineangle" + JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.SpineBase], body.Joints[JointType.HipLeft]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.SpineBase], body.Joints[JointType.HipLeft]));

            // hipleftangle
            Console.WriteLine("hipleftangle" + JointsAngle(body.Joints[JointType.KneeLeft], body.Joints[JointType.HipLeft], body.Joints[JointType.SpineBase]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.KneeLeft], body.Joints[JointType.HipLeft], body.Joints[JointType.SpineBase]));

            // hiprightangle
            Console.WriteLine("hiprightangle" + JointsAngle(body.Joints[JointType.KneeRight], body.Joints[JointType.HipRight], body.Joints[JointType.SpineBase]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.KneeRight], body.Joints[JointType.HipRight], body.Joints[JointType.SpineBase]));

            // kneeleftangle 
            Console.WriteLine("kneeleftangle" + JointsAngle(body.Joints[JointType.HipLeft], body.Joints[JointType.KneeLeft], body.Joints[JointType.AnkleLeft]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.HipLeft], body.Joints[JointType.KneeLeft], body.Joints[JointType.AnkleLeft]));

            // kneerightangle
            Console.WriteLine("kneerightangle" + JointsAngle(body.Joints[JointType.HipRight], body.Joints[JointType.KneeRight], body.Joints[JointType.AnkleRight]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.HipRight], body.Joints[JointType.KneeRight], body.Joints[JointType.AnkleRight]));
            
            return CurrentAngles;
        }
    }
}
