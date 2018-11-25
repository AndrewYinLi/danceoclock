using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace danceoclock
{
    public static class Extensions
    {
        // methods for camera
        public static ImageSource ToBitmap(this ColorFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            byte[] pixels = new byte[width * height * ((format.BitsPerPixel + 7) / 8)];

            if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                frame.CopyRawFrameDataToArray(pixels);
            }
            else
            {
                frame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        public static ImageSource ToBitmap(this DepthFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            ushort minDepth = frame.DepthMinReliableDistance;
            ushort maxDepth = frame.DepthMaxReliableDistance;

            ushort[] pixelData = new ushort[width * height];
            byte[] pixels = new byte[width * height * (format.BitsPerPixel + 7) / 8];

            frame.CopyFrameDataToArray(pixelData);

            int colorIndex = 0;
            for (int depthIndex = 0; depthIndex < pixelData.Length; ++depthIndex)
            {
                ushort depth = pixelData[depthIndex];

                byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                pixels[colorIndex++] = intensity; // Blue
                pixels[colorIndex++] = intensity; // Green
                pixels[colorIndex++] = intensity; // Red

                ++colorIndex;
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        public static ImageSource ToBitmap(this InfraredFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            ushort[] frameData = new ushort[width * height];
            byte[] pixels = new byte[width * height * (format.BitsPerPixel + 7) / 8];

            frame.CopyFrameDataToArray(frameData);

            int colorIndex = 0;
            for (int infraredIndex = 0; infraredIndex < frameData.Length; infraredIndex++)
            {
                ushort ir = frameData[infraredIndex];

                byte intensity = (byte)(ir >> 7);

                pixels[colorIndex++] = (byte)(intensity / 1); // Blue
                pixels[colorIndex++] = (byte)(intensity / 1); // Green   
                pixels[colorIndex++] = (byte)(intensity / 0.4); // Red

                colorIndex++;
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        // methods for the body
        public static Joint ScaleTo(this Joint joint, double width, double height, float skeletonMaxX, float skeletonMaxY)
        {
            joint.Position = new CameraSpacePoint
            {
                X = Scale(width, skeletonMaxX, joint.Position.X),
                Y = Scale(height, skeletonMaxY, -joint.Position.Y),
                Z = joint.Position.Z
            };

            return joint;
        }

        public static Joint ScaleTo(this Joint joint, double width, double height)
        {
            return ScaleTo(joint, width, height, 1.0f, 1.0f);
        }

        private static float Scale(double maxPixel, double maxSkeleton, float position)
        {
            float value = (float)((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));

            if (value > maxPixel)
            {
                return (float)maxPixel;
            }

            if (value < 0)
            {
                return 0;
            }

            return value;
        }

        // methods for drawing
        public static void DrawSkeleton(this Canvas canvas, Body body, Color color)
        {
            if (body == null) return;

            foreach (Joint joint in body.Joints.Values)
            {
                canvas.DrawPoint(joint, color);
            }

            canvas.DrawLine(body.Joints[JointType.Head], body.Joints[JointType.Neck], color);
            canvas.DrawLine(body.Joints[JointType.Neck], body.Joints[JointType.SpineShoulder], color);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderLeft], color);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderRight], color);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.SpineMid], color);
            canvas.DrawLine(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft], color);
            canvas.DrawLine(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight], color);
            canvas.DrawLine(body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft], color);
            canvas.DrawLine(body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight], color);
            canvas.DrawLine(body.Joints[JointType.WristLeft], body.Joints[JointType.HandLeft], color);
            canvas.DrawLine(body.Joints[JointType.WristRight], body.Joints[JointType.HandRight], color);
            canvas.DrawLine(body.Joints[JointType.HandLeft], body.Joints[JointType.HandTipLeft], color);
            canvas.DrawLine(body.Joints[JointType.HandRight], body.Joints[JointType.HandTipRight], color);
            canvas.DrawLine(body.Joints[JointType.HandTipLeft], body.Joints[JointType.ThumbLeft], color);
            canvas.DrawLine(body.Joints[JointType.HandTipRight], body.Joints[JointType.ThumbRight], color);
            canvas.DrawLine(body.Joints[JointType.SpineMid], body.Joints[JointType.SpineBase], color);
            canvas.DrawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipLeft], color);
            canvas.DrawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipRight], color);
            canvas.DrawLine(body.Joints[JointType.HipLeft], body.Joints[JointType.KneeLeft], color);
            canvas.DrawLine(body.Joints[JointType.HipRight], body.Joints[JointType.KneeRight], color);
            canvas.DrawLine(body.Joints[JointType.KneeLeft], body.Joints[JointType.AnkleLeft], color);
            canvas.DrawLine(body.Joints[JointType.KneeRight], body.Joints[JointType.AnkleRight], color);
            canvas.DrawLine(body.Joints[JointType.AnkleLeft], body.Joints[JointType.FootLeft], color);
            canvas.DrawLine(body.Joints[JointType.AnkleRight], body.Joints[JointType.FootRight], color);
        }

        // display joints as points in skeleton
        public static void DrawPoint(this Canvas canvas, Joint joint, Color color)
        {
            // check if joints are being tracked
            if (joint.TrackingState == TrackingState.NotTracked) return;

            // the joint being drawn
            joint = joint.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);

            // shape for the point
            Ellipse ellipse = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(color)
            };

            // draw the point
            Canvas.SetLeft(ellipse, joint.Position.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, joint.Position.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);
        }

        // display bones as lines in skeleton
        public static void DrawLine(this Canvas canvas, Joint first, Joint second, Color color)
        {
            // check if bones are being tracked
            if (first.TrackingState == TrackingState.NotTracked || second.TrackingState == TrackingState.NotTracked) return;

            // 2 joints that the bone connects
            first = first.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);
            second = second.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);

            // draw the line
            Line line = new Line
            {
                X1 = first.Position.X,
                Y1 = first.Position.Y,
                X2 = second.Position.X,
                Y2 = second.Position.Y,
                StrokeThickness = 8,
                Stroke = new SolidColorBrush(color)
            };
        }

        // methods for drawing
        public static void DrawRefSkeleton(this Canvas canvas, List<double> coords, Color color)
        {
            if (coords == null ) { return; }
            /*
            // scale and map to actual position on canvas
            for (int i = 0; i < coords.Count; i++)
            {
                if (i % 2 == 0) { coords[i] = coords[i] * 0.3 - 0.7 * canvas.ActualWidth; }
                else { coords[i] = coords[i] * 0.3 /*- 0.7 * canvas.ActualHeight; }
            }*/

            // head neck
            canvas.Children.Add(new Line { X1 = coords[0], Y1 = coords[1], X2 = coords[2], Y2 = coords[3], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            // neck spine
            canvas.Children.Add(new Line { X1 = coords[2], Y1 = coords[3], X2 = coords[16], Y2 = coords[17], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            // neck left shoulder
            canvas.Children.Add(new Line { X1 = coords[2], Y1 = coords[3], X2 = coords[4], Y2 = coords[5], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            // neck right shoulder
            canvas.Children.Add(new Line { X1 = coords[2], Y1 = coords[3], X2 = coords[8], Y2 = coords[9], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            // left shoulder elbow
            canvas.Children.Add(new Line { X1 = coords[4], Y1 = coords[5], X2 = coords[6], Y2 = coords[7], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            // right shoulder elbow
            canvas.Children.Add(new Line { X1 = coords[8], Y1 = coords[9], X2 = coords[10], Y2 = coords[11], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            // left elbow wrist
            canvas.Children.Add(new Line { X1 = coords[6], Y1 = coords[7], X2 = coords[12], Y2 = coords[13], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            // right elbow wrist
            canvas.Children.Add(new Line { X1 = coords[10], Y1 = coords[11], X2 = coords[14], Y2 = coords[15], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            // spine left hip
            canvas.Children.Add(new Line { X1 = coords[16], Y1 = coords[17], X2 = coords[18], Y2 = coords[19], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            // spine right hip
            canvas.Children.Add(new Line { X1 = coords[16], Y1 = coords[17], X2 = coords[20], Y2 = coords[21], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            // spine right hip
            canvas.Children.Add(new Line { X1 = coords[16], Y1 = coords[17], X2 = coords[20], Y2 = coords[21], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            // left hip knee
            canvas.Children.Add(new Line { X1 = coords[18], Y1 = coords[19], X2 = coords[22], Y2 = coords[23], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            // right hip knee
            canvas.Children.Add(new Line { X1 = coords[20], Y1 = coords[21], X2 = coords[24], Y2 = coords[25], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            // left knee foot
            canvas.Children.Add(new Line { X1 = coords[22], Y1 = coords[23], X2 = coords[26], Y2 = coords[27], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            // right knee foot
            canvas.Children.Add(new Line { X1 = coords[24], Y1 = coords[25], X2 = coords[28], Y2 = coords[29], StrokeThickness = 5, Stroke = new SolidColorBrush(color) });

            /*0 newCoords.Add(body.Joints[JointType.Head].Position.X);
            1 newCoords.Add(body.Joints[JointType.Head].Position.Y);
            2 newCoords.Add(body.Joints[JointType.Neck].Position.X);
            3 newCoords.Add(body.Joints[JointType.Neck].Position.Y);
            4 newCoords.Add(body.Joints[JointType.ShoulderLeft].Position.X);
            5 newCoords.Add(body.Joints[JointType.ShoulderLeft].Position.Y);
            6 newCoords.Add(body.Joints[JointType.ElbowLeft].Position.X);
            7 newCoords.Add(body.Joints[JointType.ElbowLeft].Position.Y);
            8 newCoords.Add(body.Joints[JointType.ShoulderRight].Position.X);
            9 newCoords.Add(body.Joints[JointType.ShoulderRight].Position.Y);
            10 newCoords.Add(body.Joints[JointType.ElbowRight].Position.X);
            11 newCoords.Add(body.Joints[JointType.ElbowRight].Position.Y);
            12 newCoords.Add(body.Joints[JointType.WristLeft].Position.X);
            13 newCoords.Add(body.Joints[JointType.WristLeft].Position.Y);
            14 newCoords.Add(body.Joints[JointType.WristRight].Position.X);
            15 newCoords.Add(body.Joints[JointType.WristRight].Position.Y);
            16 newCoords.Add(body.Joints[JointType.SpineBase].Position.X);
            17 newCoords.Add(body.Joints[JointType.SpineBase].Position.Y);
            18 newCoords.Add(body.Joints[JointType.HipLeft].Position.X);
            19 newCoords.Add(body.Joints[JointType.HipLeft].Position.Y);
            20 newCoords.Add(body.Joints[JointType.HipRight].Position.X);
            21 newCoords.Add(body.Joints[JointType.HipRight].Position.Y);
            22 newCoords.Add(body.Joints[JointType.KneeLeft].Position.X);
            23 newCoords.Add(body.Joints[JointType.KneeLeft].Position.Y);
            24 newCoords.Add(body.Joints[JointType.KneeRight].Position.X);
            25 newCoords.Add(body.Joints[JointType.KneeRight].Position.Y);
            26 newCoords.Add(body.Joints[JointType.AnkleLeft].Position.X);
            27 newCoords.Add(body.Joints[JointType.AnkleLeft].Position.Y);
            28 newCoords.Add(body.Joints[JointType.AnkleRight].Position.X);
            29 newCoords.Add(body.Joints[JointType.AnkleRight].Position.Y);*/
        }

        // display bones as lines in skeleton
        public static void DrawRefLine(this Canvas canvas, Joint first, Joint second, Color color)
        {
            // check if bones are being tracked
            if (first.TrackingState == TrackingState.NotTracked || second.TrackingState == TrackingState.NotTracked) return;

            // 2 joints that the bone connects
            first = first.ScaleTo(canvas.ActualWidth*0.3, canvas.ActualHeight*0.3);
            second = second.ScaleTo(canvas.ActualWidth*0.3, canvas.ActualHeight*0.3);

            // draw the line
            Line line = new Line
            {
                X1 = first.Position.X,
                Y1 = first.Position.Y,
                X2 = second.Position.X,
                Y2 = second.Position.Y,
                StrokeThickness = 5,
                Stroke = new SolidColorBrush(color)
            };

            canvas.Children.Add(line);
        }
    }
}
