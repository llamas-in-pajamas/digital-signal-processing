using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ComplexUtils;
using LiveCharts;
using LiveCharts.Wpf;
using SignalGenerators;
using View.ViewModel.Base;

namespace View.ViewModel
{
    public class FourierVM : BaseVM
    {
        public MainWindowVM Parent { get; set; }
        public double Duration { get; set; } = 4;
        public ICommand GenerateCustomSignal { get; set; }
        public ObservableCollection<string> TransformTypes { get; set; }
        public string SelectedTransformType { get; set; }
        public ICommand Transform { get; set; }
        public ICommand TransformBack { get; set; }

        public double SamplingFreq { get; set; } = 16;


        public SeriesCollection TopChartSeries { get; set; }
        public SeriesCollection BottomChartSeries { get; set; }
        public string TopChartName { get; set; } = "Real";
        public string BottomChartName { get; set; } = "Imaginary";

        public ObservableCollection<ComplexDataHandler> TransformedSignals { get; set; } = new ObservableCollection<ComplexDataHandler>();
        public ComplexDataHandler TransformedSignal { get; set; }

        public string LastTime { get; set; }

        public bool IsW1Checked
        {
            get => _isW1Checked;
            set
            {
                _isW1Checked = value;
                if (value)
                {
                    DrawW1(_resultOfOperation);
                }
            }
        }

        public bool IsW2Checked
        {
            get => _isW2Checked;
            set
            {
                _isW2Checked = value;
                if (value)
                {
                    DrawW2(_resultOfOperation);
                }
            }
        }


        private List<Complex> _resultOfOperation = new List<Complex>();
        private bool _isW1Checked = true;
        private bool _isW2Checked;


        public FourierVM(MainWindowVM parent)
        {
            Parent = parent;
            GenerateCustomSignal = new RelayCommand(GenerateSignal);
            TransformTypes = new ObservableCollection<string>
            {
                "Discrete Fourier Transform",
                "Fast Fourier Transform",
                "Wavelet Transform"
            };
            Transform = new RelayCommand(OnTransform);
            TransformBack = new RelayCommand(OnTransformBack);
        }

        private void OnTransformBack()
        {
            var signal = TransformedSignal;
            List<double> transformedBack = new List<double>();
            Stopwatch timer = new Stopwatch();
            switch (SelectedTransformType)
            {

                case "Discrete Fourier Transform":
                    timer.Start();
                    transformedBack = DiscreteFourierTranform.TransformBack(signal.Values);
                    timer.Stop();
                    break;
                case "Fast Fourier Transform":
                    timer.Start();
                    transformedBack = FastFourierTransform.TransformBack(signal.Values);
                    timer.Stop();
                    break;
                case "Wavelet Transform":
                    timer.Start();
                    transformedBack = Wavelet.TransformBack(signal.Values);
                    timer.Stop();
                    break;
                default:
                    MessageBox.Show("Select Transform");
                    return;

            }
            LastTime = $"{timer.ElapsedTicks} ticks";
            Parent.DataHandlers.Add(new DataHandler()
            {
                Signal = signal.Name + " back",
                IsScattered = true,
                SamplesY = transformedBack,
                Y = transformedBack
            });
        }

        private void GenerateSignal()
        {
            var signal = Signals.GetCustomSignal(Duration, SamplingFreq);
            Parent.DataHandlers.Add(signal);
        }

        private void OnTransform()
        {
            List<Complex> transformed = new List<Complex>();
            var signal = Parent.GetMainSelectedSignal();
            var values = Utils.ConvertRealToComplex(signal.SamplesY);
            Stopwatch timer = new Stopwatch();
            switch (SelectedTransformType)
            {
                case "Discrete Fourier Transform":
                    timer.Start();
                    transformed = DiscreteFourierTranform.Transform(values);
                    timer.Stop();
                    break;
                case "Fast Fourier Transform":
                    timer.Start();
                    transformed = FastFourierTransform.Transform(values);
                    //transformed = DITFFT.Transform(signal.SamplesY);
                    timer.Stop();
                    break;
                case "Wavelet Transform":
                    timer.Start();
                    transformed = Wavelet.Transform(signal.SamplesY);
                    timer.Stop();
                    break;
                default:
                    MessageBox.Show("Select Transform");
                    return;

            }

            LastTime = $"{timer.ElapsedTicks} ticks";
            _resultOfOperation = transformed;
            TransformedSignals.Add(new ComplexDataHandler(transformed, signal.Signal));

            if (IsW1Checked)
            {
                DrawW1(_resultOfOperation);
                return;
            }
            DrawW2(_resultOfOperation);
        }

        private void DrawW1(List<Complex> transformed)
        {
            TopChartSeries = new SeriesCollection();
            BottomChartSeries = new SeriesCollection();
            TopChartName = "Real";
            BottomChartName = "Imaginary";
            var real = transformed.Select(c => c.Real).ToList();
            var imaginary = transformed.Select(c => c.Imaginary).ToList();
            TopChartSeries.Add(new LineSeries()
            {
                Fill = Brushes.Transparent,
                Values = new ChartValues<double>(real),
                PointGeometry = null,
            });
            TopChartSeries.Add(new ScatterSeries()
            {
                PointGeometry = new EllipseGeometry(),
                StrokeThickness = 8,
                Values = new ChartValues<double>(real),
            });
            BottomChartSeries.Add(new LineSeries()
            {
                Fill = Brushes.Transparent,
                Values = new ChartValues<double>(imaginary),
                PointGeometry = null,
            });
            BottomChartSeries.Add(new ScatterSeries()
            {
                PointGeometry = new EllipseGeometry(),
                StrokeThickness = 8,
                Values = new ChartValues<double>(imaginary),
            });

        }

        private void DrawW2(List<Complex> transformed)
        {
            TopChartSeries = new SeriesCollection();
            BottomChartSeries = new SeriesCollection();
            TopChartName = "Magnitude";
            BottomChartName = "Phase";
            var magnitude = transformed.Select(c => c.Magnitude).ToList();
            var phase = transformed.Select(c => c.Phase).ToList();
            TopChartSeries.Add(new LineSeries()
            {
                Fill = Brushes.Transparent,
                Values = new ChartValues<double>(magnitude),
                PointGeometry = null,
            });
            TopChartSeries.Add(new ScatterSeries()
            {
                PointGeometry = new EllipseGeometry(),
                StrokeThickness = 8,
                Values = new ChartValues<double>(magnitude),
            });
            BottomChartSeries.Add(new LineSeries()
            {
                Fill = Brushes.Transparent,
                Values = new ChartValues<double>(phase),
                PointGeometry = null,
            });
            BottomChartSeries.Add(new ScatterSeries()
            {
                PointGeometry = new EllipseGeometry(),
                StrokeThickness = 8,
                Values = new ChartValues<double>(phase),
            });

        }



    }
}