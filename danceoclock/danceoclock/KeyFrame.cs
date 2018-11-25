using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows.Controls;

namespace danceoclock
{
    // represents a key frame in a gesture that the user must match
    public class KeyFrame
    {
        // frame settings - angles to match to
        public List<double> Angles;

        // for display - coordinates of joints
        public List<double> Coords;

        // body to read movements from
        public Body Body;

        // number of checking iterations (number of frames)
        public int Iterations;


        public void setAngles(List<double> Settings)
        {
            Angles = new List<double>();

            foreach (double angle in Settings)
            {
                Angles.Add(angle);
            }
        }

        public void setCoords(List<double> Coords)
        {
            this.Coords = new List<double>();

            foreach (double coord in Coords)
            {
                this.Coords.Add(coord);
            }
        }

        // constructor - sets all the frame settings
        public KeyFrame(List<double> Settings, List<double> newCoords)
        {
            setAngles(Settings);
            setCoords(newCoords);
            this.Iterations = 0;
        }

        // constructor for record mode - sets all the frame settings
        public KeyFrame(Canvas canvas, Body body, List<double> Settings)
        {
            setAngles(Settings);
            this.Body = body;
            this.Iterations = 0;

            // set coords
            List<double> newCoords = new List<double>();
            newCoords.Add(body.Joints[JointType.Head].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.Head].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
            newCoords.Add(body.Joints[JointType.Neck].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.Neck].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
            newCoords.Add(body.Joints[JointType.ShoulderLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.ShoulderLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
            newCoords.Add(body.Joints[JointType.ElbowLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.ElbowLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
            newCoords.Add(body.Joints[JointType.ShoulderRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.ShoulderRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
            newCoords.Add(body.Joints[JointType.ElbowRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.ElbowRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
            newCoords.Add(body.Joints[JointType.WristLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.WristLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
            newCoords.Add(body.Joints[JointType.WristRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.WristRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
            newCoords.Add(body.Joints[JointType.SpineBase].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.SpineBase].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
            newCoords.Add(body.Joints[JointType.HipLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.HipLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
            newCoords.Add(body.Joints[JointType.HipRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.HipRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
            newCoords.Add(body.Joints[JointType.KneeLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.KneeLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
            newCoords.Add(body.Joints[JointType.KneeRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.KneeRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
            newCoords.Add(body.Joints[JointType.AnkleLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.AnkleLeft].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);
            newCoords.Add(body.Joints[JointType.AnkleRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.X);
            newCoords.Add(body.Joints[JointType.AnkleRight].ScaleTo(canvas.ActualWidth, canvas.ActualHeight).Position.Y);

            setCoords(newCoords);
        }

        // method for checking if the current frame matches the set frame, return whether or not frame was correctly matched
        public bool Check(List<double> Current)
        {
            // add iteration number
            Iterations++;

            // if timeout
            if (Iterations > KinectWindow.Timeout)
            {
                // reset frame first
                Reset();
                return false;
            }
            else
            {
                // whether or not all the angles match
                bool AllMatch = true;

                // loop through matching angles in Current and Angles to compare them
                for (int i = 0; i < Angles.Count; i++)
                {
                    // if the angle is not within the tolerated range
                    if (!(Math.Abs(Current[i] - Angles[i]) <= Angles[i] * KinectWindow.Tolerance))
                    {
                        AllMatch = false;
                        break; // don't have to check anymore
                    }
                }

                if (AllMatch) {
                    // if all angles match
                    Reset();
                    return true;
                } else {
                    // if not all angles match, recurse
                    return Check(KinectWindow.NextFrame(Body));
                }
            }
        }

        // method for resetting iterations
        private void Reset()
        {
            Iterations = 0;
        }
    }
}
