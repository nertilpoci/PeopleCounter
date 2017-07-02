using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

/// <summary>
/// Desingned to remove the training a EigenObjectRecognizer code from the main form
/// </summary>
class Recognizer : IDisposable
{

    //Eigen
    //EigenObjectRecognizer recognizer;
    FaceRecognizer recognizer;
    //training variables
    public List<Image<Gray, byte>> TrainingImages ;
    public List<int> ImageLabels ;
  
    public Recognizer()
    {
        ImageLabels = new List<int>();
        TrainingImages = new List<Image<Gray, byte>>();
        recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100);//50
    }
  
    public void SetImageLabels()
    {
        ImageLabels.Clear();
        for(int i=0;i<TrainingImages.Count();i++)
        {
            ImageLabels.Add(i);
        }
    }
    


    public bool Recognise(Image<Gray, byte> Input_image, int Eigen_Thresh = -1)
    {
        try
        {
            recognizer.Train(TrainingImages.ToArray(), ImageLabels.ToArray() );
            FaceRecognizer.PredictionResult ER = recognizer.Predict(Input_image);   
            if (ER.Label == -1)
            {
            return false;
            }
            else
            {
            return true;
            }
        }
        catch
        {
           return false;
        }     
    }
    public int RecogniseReturnLabel(Image<Gray, byte> Input_image, int Eigen_Thresh = -1)
    {
        try {
            recognizer.Train(TrainingImages.ToArray(), ImageLabels.ToArray());
            FaceRecognizer.PredictionResult ER = recognizer.Predict(Input_image);
            return ER.Label;
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>
    /// Dispose of Class call Garbage Collector
    /// </summary>
    public void Dispose()
    {   
        GC.Collect();
    }
}

