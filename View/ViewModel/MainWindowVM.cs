using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Signal_generators;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using View.ViewModel.Base;


namespace View.ViewModel
{
    public class MainWindowVM : BaseVM
    {
        private string _signalComboBoxSelected;

        //VISIBILITY FIELDS

        //VM FIELDS
        private bool _isScattered;


        /// <summary>
        /// Commands
        /// </summary>
        public ICommand GenerateButton { get; }
        /// <summary>
        /// Props
        /// </summary>

        public CollectionView SignalComboBox
        {
            get;
        }
        public SeriesCollection SeriesCollection { get; set; }

        public string[] Labels { get; set; }

        public Func<double, string> YFormatter { get; set; }

        public double FillFactorTextBox { get; set; }

        public string SignalComboBoxSelected
        {
            get => _signalComboBoxSelected;
            set
            {
                if (_signalComboBoxSelected == value) return;
                _signalComboBoxSelected = value;
                ResolveAddidtionalValues(value);
                OnPropertyChanged(nameof(SignalComboBox));

            }
        }

        public double AmplitudeTextBox { get; set; }

        public double PeriodTextBox { get; set; }

        public double DurationTextBox { get; set; }

        public double StartTimeTextBox { get; set; }

        public double UnitEventTextBox { get; set; }

        public double ProbabilityTextBox { get; set; }

        //VISIBILITY PROPS
        public bool FillFactorTBVisibility { get; set; }

        public bool UnitEventTBVisibility { get; set; }

        public bool ProbabilityTBVisibility { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public MainWindowVM()
        {
            GenerateButton = new RelayCommand(GenerateChart);
            IList<string> list = new List<string>()
            {
                "Sinus",
                "Sinus1P",
                "Sinus2P",
                "Rectangular",
                "Symetric Rectangular",
                "Unit Jump",
                "Triangular",
                "Steady Noise",
                "Gaussian Noise",
                "Impulse Noise",
                "Unit Impulse"
            };
            SignalComboBox = new CollectionView(list);
            SignalComboBoxSelected = list[0];
        }

        /// <summary>
        /// Methods
        /// </summary>
        private void GenerateChart()
        {
            DataHandler dataHandler = new DataHandler(
                AmplitudeTextBox,
                DurationTextBox,
                StartTimeTextBox,
                PeriodTextBox,
                SignalComboBoxSelected,
                FillFactorTextBox,
                UnitEventTextBox,
                ProbabilityTextBox

            );

            try
            {
                dataHandler.Call();
                ChartValues<ObservablePoint> values = new ChartValues<ObservablePoint>();
                for (int i = 0; i < dataHandler.X.Count; i++)
                {
                    values.Add(new ObservablePoint(dataHandler.X[i], dataHandler.Y[i]));
                }

                if (_isScattered)
                {
                    SeriesCollection = new SeriesCollection
                    {

                        new ScatterSeries()
                        {
                            PointGeometry = new EllipseGeometry(),
                            StrokeThickness = 8,
                            Title = SignalComboBoxSelected,
                            Values = values
                        }
                    };
                }
                else
                {
                    SeriesCollection = new SeriesCollection
                    {

                        new LineSeries()
                        {
                            Fill = Brushes.Transparent,
                            Title = SignalComboBoxSelected,
                            Values = values,
                            PointGeometry = null
                        }
                    };
                }

            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }


        }

        private void ResolveAddidtionalValues(string value)
        {
            FillFactorTBVisibility = false;
            UnitEventTBVisibility = false;
            ProbabilityTBVisibility = false;
            _isScattered = false;

            switch (value)
            {
                case "Sinus":
                    
                    break;
                case "Sinus1P":
                    break;
                case "Sinus2P":
                    break;
                case "Rectangular":
                    FillFactorTBVisibility = true;
                    break;
                case "Symetric Rectangular":
                    FillFactorTBVisibility = true;
                    break;
                case "Unit Jump":
                    UnitEventTBVisibility = true;
                    break;
                case "Triangular":
                    FillFactorTBVisibility = true;
                    break;
                case "Steady Noise":
                    break;
                case "Gaussian Noise":
                    break;
                case "Impulse Noise":
                    _isScattered = true;
                    ProbabilityTBVisibility = true;
                    break;
                case "Unit Impulse":
                    _isScattered = true;
                    UnitEventTBVisibility = true;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
