using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FaceCounter.ViewModel;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Concurrent;
using System.Data.Common.Utils;
using System.Diagnostics;

namespace FaceCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private void Info_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/nertilpoci/PeopleCounter");
        }
    }
}
