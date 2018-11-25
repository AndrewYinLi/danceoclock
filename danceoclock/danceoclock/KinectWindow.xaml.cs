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
using System.IO;

namespace danceoclock
{

    public partial class KinectWindow : Window
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
        public static int Numrepeats;

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


        public static Gesture currentGesture = null;

        // ptr to parent (Andrew's UI)
        MainWindow parent = null;
        string path = null;


        // constructor for recording mode
        public KinectWindow(MainWindow parent, string path, int sec, int length)
        {
            this.parent = parent;
            this.path = path;
            InitializeComponent();

            Sec = sec;
            Length = length;
            FramesLeft = 30 * Length;
        }


        // constructor for alarm mode, gestNamesList (list of names of all gestures used in order) comes from UI main
        public KinectWindow(string gesturePath, double tolerance, double timeout, int numrepeats)
        {
            Recording = false;

            Tolerance = tolerance;
            Timeout = timeout;
            Numrepeats = numrepeats;

            /* put these in constructor as params
            Tolerance = 0.5;
            Timeout = 100;
            Numrepeats = 1; */

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
                                canvas.DrawSkeleton(body, Colors.DarkBlue);

                                List<double> settings = NextFrame(body);

                                // recording mode
                                if (Recording)
                                {
                                    // If 1 second has passed
                                    if (++CurrentNumFrames == Sec * 30)
                                    {
                                        if ((FramesLeft -= CurrentNumFrames) >= 0)
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

                                            foreach (double coord in newFrame.Coords)
                                            {
                                                Console.WriteLine(coord);
                                            }

                                            newGesture.AddKeyframe(newFrame);
                                        }
                                        else
                                        {
                                            // Pass newGesture into ui main window
                                            parent.writeGesture(newGesture, path);
                                            Close();
                                        }
                                        CurrentNumFrames = 0;
                                    }

                                }
                                else // alarm mode
                                {
                                    currentGesture.setBody(body); // use the right body instance
                                    canvas.DrawRefSkeleton(currentGesture.Keyframes[currentGesture.frameIndex].Coords, Colors.DarkBlue);

                                    foreach (double coords in currentGesture.Keyframes[currentGesture.frameIndex].Coords) { Console.WriteLine(coords); }

                                    if (currentGesture.SetKeyframe())
                                    {
                                        canvas.DrawSkeleton(body, Colors.Green);
                                        Close();
                                    } else
                                    {
                                        canvas.DrawSkeleton(body, Colors.Red);
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

            //Console.WriteLine("neckangle" + JointsAngle(body.Joints[JointType.Head], body.Joints[JointType.Neck], body.Joints[JointType.ShoulderLeft]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.Head], body.Joints[JointType.Neck], body.Joints[JointType.ShoulderLeft]));

            // shoulderleftangle
            //Console.WriteLine("shoulderleftangle" + JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft]));

            // shoulderrightangle
            //Console.WriteLine("shoulderrightangle" + JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight]));

            // elbowleftangle
            //Console.WriteLine("elbowleftangle" + JointsAngle(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft]));

            // elbowrightangle
           // Console.WriteLine("elbowrightangle" + JointsAngle(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight]));

            // spineangle
            //Console.WriteLine("spineangle" + JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.SpineBase], body.Joints[JointType.HipLeft]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.Neck], body.Joints[JointType.SpineBase], body.Joints[JointType.HipLeft]));

            // hipleftangle
            //Console.WriteLine("hipleftangle" + JointsAngle(body.Joints[JointType.KneeLeft], body.Joints[JointType.HipLeft], body.Joints[JointType.SpineBase]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.KneeLeft], body.Joints[JointType.HipLeft], body.Joints[JointType.SpineBase]));

            // hiprightangle
            //Console.WriteLine("hiprightangle" + JointsAngle(body.Joints[JointType.KneeRight], body.Joints[JointType.HipRight], body.Joints[JointType.SpineBase]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.KneeRight], body.Joints[JointType.HipRight], body.Joints[JointType.SpineBase]));

            // kneeleftangle 
            //Console.WriteLine("kneeleftangle" + JointsAngle(body.Joints[JointType.HipLeft], body.Joints[JointType.KneeLeft], body.Joints[JointType.AnkleLeft]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.HipLeft], body.Joints[JointType.KneeLeft], body.Joints[JointType.AnkleLeft]));

            // kneerightangle
            //Console.WriteLine("kneerightangle" + JointsAngle(body.Joints[JointType.HipRight], body.Joints[JointType.KneeRight], body.Joints[JointType.AnkleRight]));
            CurrentAngles.Add(JointsAngle(body.Joints[JointType.HipRight], body.Joints[JointType.KneeRight], body.Joints[JointType.AnkleRight]));


            /*
            StreamWriter f = new StreamWriter("C:\\Users\\shanali\\Desktop\\penisattempt.txt", true);
            StringBuilder sb = new StringBuilder();
            foreach (double angle in CurrentAngles)
            {
                sb.Append(angle);
                sb.Append(" ");
            }
            sb.Remove(sb.Length - 1, 1); // trim space
            f.WriteLine(sb.ToString());
            f.Close();
            */

            return CurrentAngles;
        }
    }
}
