using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Signal_generators;
using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using View.ViewModel.Base;

namespace View.ViewModel
{
    public class MainWindowVM : BaseVM
    {

        /// <summary>
        /// Private Fields
        /// </summary>
        private string _amplitudeTextBox;
        private string _periodTextBox;
        private string _durationTextBox;
        private string _startTimeTextBox;
        private SeriesCollection _seriesCollection;
        public string[] _labels;
        public Func<double, string> _yFormatter;
        private CollectionView _signalComboBox;
        private string _signalComboBoxSelected;
        private string _fillFactorTextBox;


        /// <summary>
        /// Commands
        /// </summary>
        public ICommand GenerateButton { get; }
        /// <summary>
        /// Props
        /// </summary>

        public string FillFactorTextBox
        {
            get => _fillFactorTextBox;
            set
            {
                _fillFactorTextBox = value;
                OnPropertyChanged(nameof(FillFactorTextBox));
            }
        }
        public CollectionView SignalComboBox
        {
            get => _signalComboBox;
        }

        public string SignalComboBoxSelected
        {
            get => _signalComboBoxSelected;
            set
            {
                if (_signalComboBoxSelected == value) return;
                _signalComboBoxSelected = value;
                OnPropertyChanged(nameof(SignalComboBox));

            }
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

        public string AmplitudeTextBox
        {
            get => _amplitudeTextBox;
            set
            {
                _amplitudeTextBox = value;
                OnPropertyChanged(nameof(AmplitudeTextBox));
            }
        }

        public string PeriodTextBox
        {
            get => _periodTextBox;
            set
            {
                _periodTextBox = value;
                OnPropertyChanged(nameof(PeriodTextBox));
            }
        }

        public string DurationTextBox
        {
            get => _durationTextBox;
            set
            {
                _durationTextBox = value;
                OnPropertyChanged(nameof(DurationTextBox));
            }

        }

        public string StartTimeTextBox
        {
            get => _startTimeTextBox; set
            {
                _startTimeTextBox = value;
                OnPropertyChanged(nameof(StartTimeTextBox));
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
        }

        /// <summary>
        /// Methods
        /// </summary>
        private void GenerateChart()
        {
            DataHandler dataHandler = new DataHandler(
                Double.Parse(_amplitudeTextBox),
                Double.Parse(DurationTextBox),
                Double.Parse(StartTimeTextBox),
                Double.Parse(PeriodTextBox),
                SignalComboBoxSelected
            );

            dataHandler.Call();
            ChartValues<ObservablePoint> values = new ChartValues<ObservablePoint>();
            for (int i = 0; i < dataHandler.X.Count; i++)
            {
                values.Add(new ObservablePoint(dataHandler.X[i], dataHandler.Y[i]));
            }
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Sinus",
                    Values = values
                }
            };
        }
    }
}
