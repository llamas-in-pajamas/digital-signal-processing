using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using SignalGenerators;
using SignalUtils;
using View.ViewModel.Base;

namespace View.ViewModel
{
    public class ReconstructVM : BaseVM
    {

        public MainWindowVM Parent { get; set; }
        private readonly ChartHistogramVM _chartHistogramVm;

        public int QuantizationLevelsTextBox { get; set; } = 4; //TODO: move
        public int NumOfSamplesTextBox { get; set; } = 50; //TODO: move
        public ObservableCollection<string> ReconstructComboBox { get; set; }
        public string ReconstructComboBoxSelected { get; set; }

        public ObservableCollection<string> ReconstructionMethodComboBox { get; set; }
        public string ReconstructionMethodComboBoxSelected { get; set; }

        public bool DrawSamplesIsChecked { get; set; } = true;
        public bool DrawQuantaIsChecked { get; set; }
        public bool DrawReconstructedIsChecked { get; set; }

        //DATA
        public double MseTextBox { get; set; }
        public double SnrTextBox { get; set; }
        public double MdTextBox { get; set; }
        public double PsnrTextBox { get; set; }
        public double EnobTextBox { get; set; }

        public ICommand RemoveLatestButton { get; }
        public ICommand ReconstructButton { get; }
        
        public ReconstructVM(MainWindowVM parent)
        {
            Parent = parent;
            _chartHistogramVm = Parent.ChartHistogramVm;
            RemoveLatestButton = _chartHistogramVm.RemoveLatestButton;
            ReconstructButton = new RelayCommand(Reconstruct);
            ReconstructComboBox = new ObservableCollection<string>
            {
                "From Samples",
                "From Quanta"
            };
            ReconstructionMethodComboBox = new ObservableCollection<string> {
                "Sinus Cardinalis",
                "First Order"

            };
        }

        private void Reconstruct()
        {
            if (string.IsNullOrEmpty(ReconstructComboBoxSelected) || string.IsNullOrEmpty(ReconstructionMethodComboBoxSelected))
            {
                MessageBox.Show("All ComboBoxes have to contain value for this operation", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataHandler chart;
            try
            {
                chart = Parent.GetMainSelectedSignal();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var sampledValues = chart.SamplesY;
            List<double> quants = SignalUtils.Operations.Quantize(sampledValues, QuantizationLevelsTextBox);
            
            var valuesToReconstruct = new List<double>();
            if (ReconstructComboBoxSelected.Contains("From Samples"))
            {
                valuesToReconstruct = sampledValues;
            }
            if (ReconstructComboBoxSelected.Contains("From Quanta"))
            {
                valuesToReconstruct = quants;
            }


            List<double> reconstructedSignal = new List<double>();
            if (ReconstructionMethodComboBoxSelected.Contains("Sinus Cardinalis"))
            {
                reconstructedSignal = Operations.ReconstructCard(chart.SamplesX, valuesToReconstruct, NumOfSamplesTextBox, chart.SamplingFrequency, chart.StartTime, chart.StartTime + chart.Duration); ;
            }
            if (ReconstructionMethodComboBoxSelected.Contains("First Order"))
            {
                reconstructedSignal = Operations.ReconstructFirstOrder(chart.SamplesX, valuesToReconstruct, chart.SamplingFrequency, chart.StartTime, chart.StartTime + chart.Duration);
            }

            QuantizedStatictics stats = new QuantizedStatictics(chart.Y, reconstructedSignal);
            MseTextBox = stats.MSE;
            SnrTextBox = stats.SNR;
            MdTextBox = stats.MD;
            PsnrTextBox = stats.PSNR;
            EnobTextBox = 0;

            if (ReconstructComboBoxSelected.Contains("From Quanta"))
            {
                EnobTextBox = stats.ENOB;
            }


            if (DrawReconstructedIsChecked)
            {
                ChartValues<ObservablePoint> reconstructedValues = new ChartValues<ObservablePoint>();
                for (int i = 0; i < chart.X.Count; i++)
                {
                    reconstructedValues.Add(new ObservablePoint(chart.X[i], reconstructedSignal[i]));
                }
                _chartHistogramVm.SeriesCollection.Add(new LineSeries()
                {
                    Fill = Brushes.Transparent,
                    Title = "Reconstructed",
                    Values = reconstructedValues,
                    PointGeometry = null,
                    LineSmoothness = 0
                });
            }


            if (DrawQuantaIsChecked)
            {
                ChartValues<ObservablePoint> quantsValues = new ChartValues<ObservablePoint>();
                for (int i = 0; i < chart.SamplesX.Count; i++)
                {
                    quantsValues.Add(new ObservablePoint(chart.SamplesX[i], quants[i]));
                }
                _chartHistogramVm.SeriesCollection.Add(new LineSeries()
                {
                    Fill = Brushes.Transparent,
                    Title = "Quanta",
                    Values = quantsValues,
                    PointGeometry = null,
                    LineSmoothness = 0

                });
            }

            if (DrawSamplesIsChecked)
            {
                ChartValues<ObservablePoint> sampleValues = new ChartValues<ObservablePoint>();
                for (int i = 0; i < chart.SamplesX.Count; i++)
                {
                    sampleValues.Add(new ObservablePoint(chart.SamplesX[i], chart.SamplesY[i]));
                }
                Parent.ChartHistogramVm.SeriesCollection.Add(new ScatterSeries()
                {
                    PointGeometry = new EllipseGeometry(),
                    StrokeThickness = 8,
                    Title = "Samples",
                    Values = sampleValues
                });
            }

        }

    }
}