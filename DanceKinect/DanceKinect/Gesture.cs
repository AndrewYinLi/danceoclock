using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace DanceKinect
{
    class Gesture
    {
        // list of key frames in the gesture
        public static List<KeyFrame> Keyframes;

        // body used to match movements
        public static Body Body;

        public Gesture(Body body)
        {
            Keyframes = new List<KeyFrame>();
            Body = body;
        }   

        // add a keyframe
        public static void AddKeyframe(KeyFrame keyframe)
        {
            Keyframes.Add(keyframe);
        }

        // repeat the specified number of times and match the movements, return whether or not the set was successfully completed
        public static void Repeat()
        {
            for (int i = 0; i < MainWindow.Numrepeats; i++) {

                foreach (KeyFrame kf in Keyframes)
                {
                    kf.Check(MainWindow.NextFrame(Body));
                }
            }
        }
    }
}
