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
        }

        // constructor for record mode - sets all the frame settings
        public KeyFrame(Body body, List<double> Settings)
        {
            setAngles(Settings);
            this.Body = body;
        }
    }
}
