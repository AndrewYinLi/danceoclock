using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceKinect
{
    // represents a key frame in a gesture that the user must match
    class KeyFrame
    {
        // method for calculating the dot product of 2 vectors
        public static double Dot(double x1, double y1, double x2, double y2)
        {
            return x1 * x2 + y1 * y2;
        }

        // method for calculating angle between two vectors
        public static double Angle(double x1, double y1, double x2, double y2)
        {
            double theta = Math.Round((180/Math.PI)*Math.Acos((double)Dot(x1, y1, x2, y2) /
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
}
