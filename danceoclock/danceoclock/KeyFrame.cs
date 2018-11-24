using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace danceoclock
{
    // represents a key frame in a gesture that the user must match
    public class KeyFrame
    {
        // frame settings - angles to match to
        public List<double> Angles;

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

            this.Iterations = 0;
        }

        // constructor - sets all the frame settings
        public KeyFrame(List<double> Settings)
        {
            setAngles(Settings);
        }

        // constructor - sets all the frame settings
        public KeyFrame(Body body, List<double> Settings)
        {
            setAngles(Settings);
            this.Body = body;

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
                Boolean AllMatch = true;

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
