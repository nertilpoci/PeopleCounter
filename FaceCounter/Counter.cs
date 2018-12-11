using DirectShowLib;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FaceCounter
{


    class Counter : ViewModelBase, IDisposable
    {


        public event EventHandler<Image<Gray, byte>> OnCountChanged;
        #region variables
        public CascadeClassifier Face = new CascadeClassifier(System.IO.Directory.GetCurrentDirectory() + "/Cascades/haarcascade_frontalface_default.xml");//Our face detection method 
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_COMPLEX, 0.5, 0.5); //Our fount for writing within the frame
        Image<Bgr, Byte> currentFrame; //current image aquired from webcam for display
        Image<Gray, byte> result; //used to store the result image
        Image<Gray, byte> gray_frame = null; //grayscale current image aquired from webcam for processing
        List<Image<Gray, byte>> prevFaces = new List<Image<Gray, byte>>(); // store all the faces found previously to compare with the others
        List<Image<Gray, byte>> currFaces = new List<Image<Gray, byte>>(); // store all the faces found in the current scan to compare with the others
        System.Drawing.Rectangle[] prevFaceLocations = null;
        Capture grabber; //This is our capture variable
        Recognizer recog;
        Thread counterThread;
        #endregion
        DisplayWindow display;
        ViewModel.ViewModelLocator locator;
        public Counter(Camera a)
        {
            this.camera = a;
            locator = new ViewModel.ViewModelLocator();
            var capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            grabber = new Capture(capDevices.ToList().IndexOf(capDevices.Single(z => z.Name == Camera.Source)));
            recog = new Recognizer();
            display = new DisplayWindow
            {
                Title = Camera.Name
            };
            display.Show();
            counterThread = new Thread(StartCounting);
        }
        public void Start()
        {
            counterThread.Start();
            grabber.Start();
        }
        public void Stop()
        {
            counterThread.Abort();
            grabber.Stop();
            grabber.Dispose();
            display.Close();
        }

        void StartCounting()
        {
            while (true)
            {
                SimultaniouFaceDetect();
                Thread.Sleep(100);
            }
        }
        void SimultaniouFaceDetect()
        {
            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame()?.Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
            if (currentFrame != null)
            {
                gray_frame = currentFrame.Convert<Gray, Byte>();
                facesDetected = Face.DetectMultiScale(gray_frame, 1.2, 10, new System.Drawing.Size(50, 50), System.Drawing.Size.Empty);
                currFaces.Clear();
                Parallel.For(0, facesDetected.Length, i =>
                {
                    try
                    {
                        facesDetected[i].X += (int)(facesDetected[i].Height * 0.15);
                        facesDetected[i].Y += (int)(facesDetected[i].Width * 0.22);
                        facesDetected[i].Height -= (int)(facesDetected[i].Height * 0.3);
                        facesDetected[i].Width -= (int)(facesDetected[i].Width * 0.35);
                        result = currentFrame.Copy(facesDetected[i]).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                        result._EqualizeHist();
                        currFaces.Add(result);
                        currentFrame.Draw(facesDetected[i], new Bgr(System.Drawing.Color.Red), 2);
                        ///  currentFrame.Draw("Camera: " + Camera.Name, ref font, new System.Drawing.Point(20, 20), new Bgr(System.Drawing.Color.Red));
                    }
                    catch
                    {


                    }
                });
                recog.TrainingImages = currFaces;
                recog.SetImageLabels();
                for (int i = 0; i < prevFaces.Count; i++)
                {
                    if (!recog.Recognise(prevFaces[i]))
                    {
                        Debug.WriteLine(this.Camera.Name + " person found inside");
                        OnCountChanged(this, prevFaces[i]);
                    }
                    else
                    {
                        Debug.WriteLine("face still exists on view");
                    }
                }
                prevFaceLocations = facesDetected;
                prevFaces.Clear();
                prevFaces.AddRange(currFaces);
                display.image.Dispatcher.Invoke(() =>
                {
                    display.image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(currentFrame.ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                });
                Debug.WriteLine("facebraber ended " + facesDetected.Count() + " faces detected");
            }
        }

        public void Dispose()
        {
            Stop();
        }

        System.Drawing.Rectangle[] facesDetected;
        public static List<Counter> Counters = new List<Counter>();
        private Camera camera;
        public Camera Camera
        {
            get
            {
                return camera;
            }

            set
            {
                camera = value;
            }
        }
    }
}
