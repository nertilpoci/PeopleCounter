using DirectShowLib;
using Emgu.CV;
using Emgu.CV.Structure;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Data.Common.Utils;
using System.Diagnostics;
using System.Linq;


namespace FaceCounter.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        ThreadSafeList<Image<Gray, byte>> InsidePeoples;
        Recognizer mainRecognizer;
        private string _inCamera, _outCamera;
        private bool _isCounting;
        Counter inCounter, outCounter;
        public MainViewModel()
        {
            var capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            AvailableCameras = capDevices.Select(z => z.Name).ToList();
            InsidePeoples = new ThreadSafeList<Image<Gray, byte>>();
            mainRecognizer = new Recognizer();
            StartCountingCommand = new RelayCommand(StartCounting);
            StopCountingCommand = new RelayCommand(StopCounting);
        }
        private void StopCounting()
        {
            inCounter?.Stop();
            outCounter?.Stop();
            IsCounting = false;
        }

            private void StartCounting()
        {
            if (!string.IsNullOrEmpty(InCamera))
            {
                inCounter = new Counter(new Camera { Name = "InCamera", Source = InCamera, Type = CameraType.In });
                inCounter.OnCountChanged += new EventHandler<Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>>(counterHandler);
                inCounter.Start();
                IsCounting = true;
            }
            if(!string.IsNullOrEmpty(OutCamera))
            {
                outCounter = new Counter(new Camera { Name = "OutCamera", Source = OutCamera, Type = CameraType.Out });
                outCounter.OnCountChanged += new EventHandler<Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>>(counterHandler2);
                outCounter.Start();
                IsCounting = true;
            }
        }
        void counterHandler(object sender, Image<Gray, byte> e)
        {
            mainRecognizer.TrainingImages = InsidePeoples.ToList();
            mainRecognizer.SetImageLabels();
            int index = mainRecognizer.RecogniseReturnLabel(e, 2000);
            if (index == -1)
            {
                InsidePeoples.Add(e);
                In = In + 1;
                Debug.WriteLine("Person Entered");
            }
        }
        void counterHandler2(object sender, Image<Gray, byte> e)
        {
            mainRecognizer.TrainingImages = InsidePeoples.ToList();
            mainRecognizer.SetImageLabels();
            int index = mainRecognizer.RecogniseReturnLabel(e, 2000);
            if (index != -1)
            {
                InsidePeoples.RemoveAt(index);
                Debug.WriteLine("Person Left");
                Out = Out + 1;
            }
        }
        private int _in;
        public int In { get { return _in; } set { _in = value; RaisePropertyChanged("In"); } }

        private int _out;
        public int Out { get { return _out; } set { _out = value; RaisePropertyChanged("Out"); } }

        private List<string> availableCameras;
        public List<string> AvailableCameras
        {
            get { return availableCameras; }
            set { this.availableCameras = value; RaisePropertyChanged("AvailableCameras"); }

        }
        public RelayCommand StartCountingCommand { get; set; }
        public RelayCommand StopCountingCommand { get; set; }
        public string InCamera { get => _inCamera; set { _inCamera = value; RaisePropertyChanged("InCamera"); } }
        public string OutCamera { get => _outCamera; set { _outCamera = value; RaisePropertyChanged("OutCamera"); } }

        public bool IsCounting { get => _isCounting; set { _isCounting = value; RaisePropertyChanged("IsCounting"); } }
    }
}