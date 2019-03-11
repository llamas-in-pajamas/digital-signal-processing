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

        /// <summary>
        /// FIELDS
        /// </summary>

        //VIEW FIELDS
        private double _amplitudeTextBox;
        private double _periodTextBox;
        private double _durationTextBox;
        private double _startTimeTextBox;
        private string _signalComboBoxSelected;
        private double _fillFactorTextBox;
        private double _unitEventTextBox;
        private double _probabilityTextBox;
        //VISIBILITY FIELDS
        private bool _fillFactorTBVisibility;
        private bool _unitEventTBVisibility;
        private bool _probabilityTBVisibility;

        //VM FIELDS
        public string[] _labels;
        public Func<double, string> _yFormatter;
        private CollectionView _signalComboBox;
        private SeriesCollection _seriesCollection;
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
            get => _signalComboBox;
        }
        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set
            {
                _seriesCollection = value;
                OnPropertyChanged(nameof(SeriesCollection));
            }
        }
        public string[] Labels
        {
            get => _labels;
            set
            {
                _labels = value;
                OnPropertyChanged(nameof(Labels));
            }
        }
        public Func<double, string> YFormatter
        {
            get => _yFormatter;
            set
            {
                _yFormatter = value;
                OnPropertyChanged(nameof(YFormatter));
            }
        }
        public double FillFactorTextBox
        {
            get => _fillFactorTextBox;
            set
            {
                _fillFactorTextBox = value;
                OnPropertyChanged(nameof(FillFactorTextBox));
            }
        }

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

        public double AmplitudeTextBox
        {
            get => _amplitudeTextBox;
            set
            {
                _amplitudeTextBox = value;
                OnPropertyChanged(nameof(AmplitudeTextBox));
            }
        }

        public double PeriodTextBox
        {
            get => _periodTextBox;
            set
            {
                _periodTextBox = value;
                OnPropertyChanged(nameof(PeriodTextBox));
            }
        }

        public double DurationTextBox
        {
            get => _durationTextBox;
            set
            {
                _durationTextBox = value;
                OnPropertyChanged(nameof(DurationTextBox));
            }

        }

        public double StartTimeTextBox
        {
            get => _startTimeTextBox; set
            {
                _startTimeTextBox = value;
                OnPropertyChanged(nameof(StartTimeTextBox));
            }
        }

        public double UnitEventTextBox
        {
            get => _unitEventTextBox;
            set
            {
                _unitEventTextBox = value;
                OnPropertyChanged(nameof(UnitEventTextBox));
            }
        }

        public double ProbabilityTextBox
        {
            get => _probabilityTextBox;
            set
            {
                _probabilityTextBox = value;
                OnPropertyChanged(nameof(ProbabilityTextBox));
            }
        }
        //VISIBILITY PROPS
        public bool FillFactorTBVisibility
        {
            get => _fillFactorTBVisibility;
            set
            {
                _fillFactorTBVisibility = value;
                OnPropertyChanged(nameof(FillFactorTBVisibility));
            }
        }

        public bool UnitEventTBVisibility
        {
            get => _unitEventTBVisibility;
            set
            {
                _unitEventTBVisibility = value;
                OnPropertyChanged(nameof(UnitEventTBVisibility));
            }
        }

        public bool ProbabilityTBVisibility
        {
            get => _probabilityTBVisibility;
            set
            {
                _probabilityTBVisibility = value;
                OnPropertyChanged(nameof(ProbabilityTBVisibility));
            }
        }

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
            _signalComboBox = new CollectionView(list);
            SignalComboBoxSelected = list[0];
        }

        /// <summary>
        /// Methods
        /// </summary>
        private void GenerateChart()
        {
            DataHandler dataHandler = new DataHandler(
                _amplitudeTextBox,
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
                            Values = values
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

            switch (value)
            {
                case "Sinus":
                    _isScattered = false;
                    break;
                case "Sinus1P":
                    _isScattered = false;
                    break;
                case "Sinus2P":
                    _isScattered = false;
                    break;
                case "Rectangular":
                    _isScattered = false;
                    FillFactorTBVisibility = true;
                    break;
                case "Symetric Rectangular":
                    _isScattered = false;
                    FillFactorTBVisibility = true;
                    break;
                case "Unit Jump":
                    _isScattered = false;
                    UnitEventTBVisibility = true;
                    break;
                case "Triangular":
                    _isScattered = false;
                    FillFactorTBVisibility = true;
                    break;
                case "Steady Noise":
                    _isScattered = false;
                    break;
                case "Gaussian Noise":
                    _isScattered = false;
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
