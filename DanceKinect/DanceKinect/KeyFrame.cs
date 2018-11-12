using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace DanceKinect
{
    // represents a key frame in a gesture that the user must match
    class KeyFrame
    {
        // frame settings - angles to match to
        readonly List<double> Angles;

        // body to read movements from
        Body Body;

        // number of checking iterations (number of frames)
        int Iterations;

        // constructor - sets all the frame settings
        KeyFrame(Body body, List<double> Settings)
        {
            this.Body = body;

            foreach (double angle in Settings)
            {
                Angles.Add(angle);
            }

            this.Iterations = 0;
        }

        // method for checking if the current frame matches the set frame, return whether or not frame was correctly matched
        public Boolean Check(List<double> Current)
        {
            // add iteration number
            Iterations++;

            // if timeout
            if (Iterations > MainWindow.Timeout)
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
                    if (!(Math.Abs(Current[i] - Angles[i]) <= Angles[i] * MainWindow.Tolerance))
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
                    return Check(MainWindow.NextFrame(Body));
                    return false;
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
