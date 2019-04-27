using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using SignalGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using SignalUtils;
using View.Helper;
using View.ViewModel.Base;

namespace View.ViewModel
{
    public class MainWindowVM : BaseVM
    {
        private string _signalComboBoxSelected;

        private List<DataHandler> _dataHandlers = new List<DataHandler>();
        private List<string> _signals = new List<string>();

        //VM FIELDS
        private RadioButtonsEnum _drawModeRadioBTN;
        private DataHandler dataHandler;
        private int _numberOfColumnsTb = 5;
        private string _mainComboBoxSelected;
        private string _additionalComboBoxSelected;
        private string _operationComboBoxSelected;
        private bool _isDarkTheme = true;
        private string _reconstructComboBoxSelected;
        private string _reconstructionMethodComboBoxSelected;
        private string _operationTypeComboBoxSelected;
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string RegistryValueName = "AppsUseLightTheme";


        public ICommand GenerateButton { get; }
        public ICommand RemoveButton { get; }
        public ICommand DoOperationButton { get; }
        public ICommand SaveButton { get; }
        public ICommand LoadButton { get; }
        public ICommand ReconstructButton { get; }
        public ICommand RemoveLatestButton { get; }


        #region props

        private ChartValues<ObservablePoint> Values { get; set; } = new ChartValues<ObservablePoint>();
        private ChartValues<double> HistogramValues { get; set; } = new ChartValues<double>();

        public int NumberOfColumnsTB
        {
            get => _numberOfColumnsTb;
            set
            {
                _numberOfColumnsTb = value;
                OnPropertyChanged(nameof(NumberOfColumnsTB));
                DrawHistogram();
            }
        }

        public double SamplingFrequencyTextBox { get; set; } = 1000;

        public CollectionView SignalsComboBox { get; set; }
        public CollectionView AdditionalSignalsComboBox { get; set; }

        public CollectionView SignalComboBox { get; }
        public CollectionView OperationsComboBox { get; }

        //Chart Props

        #region chart props

        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public SeriesCollection SeriesCollectionHistogram { get; set; } = new SeriesCollection();

        public string[] Labels { get; set; }
        public string[] LabelsHistogram { get; set; }

        public Func<double, string> YFormatter { get; set; }

        #endregion
        

        public double FillFactorTextBox { get; set; }

        public string SignalComboBoxSelected
        {
            get => _signalComboBoxSelected;
            set
            {
                if (_signalComboBoxSelected == value) return;
                _signalComboBoxSelected = value;
                ResolveAdditionalValues(value);
                OnPropertyChanged(nameof(SignalComboBox));

            }
        }

        public string MainComboBoxSelected
        {
            get => _mainComboBoxSelected;
            set
            {
                if (_mainComboBoxSelected == value) return;
                _mainComboBoxSelected = value;
                DrawHistogram();
                LoadStatistics();
                OnPropertyChanged(nameof(SignalsComboBox));
            }
        }

        public string AdditionalComboBoxSelected
        {
            get => _additionalComboBoxSelected;
            set
            {
                if (_additionalComboBoxSelected == value) return;
                _additionalComboBoxSelected = value;
                OnPropertyChanged(nameof(AdditionalSignalsComboBox));
            }
        }

        public string OperationComboBoxSelected
        {
            get => _operationComboBoxSelected;
            set
            {
                if (_operationComboBoxSelected == value) return;
                _operationComboBoxSelected = value;
                OnPropertyChanged(nameof(OperationComboBoxSelected));
            }
        }


        public double AmplitudeTextBox { get; set; }

        public double PeriodTextBox { get; set; }

        public double DurationTextBox { get; set; }

        public double StartTimeTextBox { get; set; }

        public double UnitEventTextBox { get; set; }

        public double ProbabilityTextBox { get; set; }
        #region Signal Stats

        public double SignalAverageTextBox { get; set; }
        public double SignalVarianceTextBox { get; set; }
        public double SignalAbsoluteAverageTextBox { get; set; }
        public double SignalAveragePowerTextBox { get; set; }
        public double SignalRMSTextBox { get; set; }
        #endregion

        #region SignalOperationProps

        public int QuantizationLevelsTextBox { get; set; } = 4;
        public int NumOfSamplesTextBox { get; set; } = 50;
        public CollectionView ReconstructComboBox { get; set; }
        public CollectionView ReconstructionMethodComboBox { get; set; }
        public string ReconstructionMethodComboBoxSelected
        {
            get => _reconstructionMethodComboBoxSelected;
            set
            {
                if (_reconstructionMethodComboBoxSelected == value) return;
                _reconstructionMethodComboBoxSelected = value;
                OnPropertyChanged(nameof(ReconstructionMethodComboBox));
            }
        }

        public string ReconstructComboBoxSelected
        {
            get => _reconstructComboBoxSelected;
            set
            {
                if (_reconstructComboBoxSelected == value) return;
                _reconstructComboBoxSelected = value;
                OnPropertyChanged(nameof(ReconstructComboBox));
            }
        }

        public bool DrawSamplesIsChecked { get; set; } = true;
        public bool DrawQuantaIsChecked { get; set; }
        public bool DrawReconstructedIsChecked { get; set; }

        //DATA
        public double MseTextBox { get; set; }
        public double SnrTextBox { get; set; }
        public double MdTextBox { get; set; }
        public double PsnrTextBox { get; set; }
        public double EnobTextBox { get; set; }



        #endregion

        #region Advanced Operation Props
        public ICommand DoItButton { get; }

        public CollectionView OperationTypeComboBox { get; }

        public string OperationTypeComboBoxSelected
        {
            get => _operationTypeComboBoxSelected;
            set
            {
                if (_operationTypeComboBoxSelected == value) return;
                _operationTypeComboBoxSelected = value;
                OnPropertyChanged(nameof(OperationTypeComboBoxSelected));

            }
        }

        #endregion


        #endregion


        #region Visibilty props

        public bool FillFactorTBVisibility { get; set; }

        public bool UnitEventTBVisibility { get; set; }

        public bool ProbabilityTBVisibility { get; set; }


        #endregion


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
                "Uniform Noise",
                "Gaussian Noise",
                "Impulse Noise",
                "Unit Impulse"
            };
            IList<string> list1 = new List<string>()
            {
                "Add",
                "Subtract",
                "Multiply",
                "Divide"
            };

            OperationsComboBox = new CollectionView(list1);
            OperationComboBoxSelected = list1[0];
            SignalComboBox = new CollectionView(list);
            SignalComboBoxSelected = list[0];
            RemoveButton = new RelayCommand(RemoveChart);
            DoOperationButton = new RelayCommand(Operations);
            SaveButton = new RelayCommand(async () => await Task.Run(() => SaveSignal()));
            LoadButton = new RelayCommand(LoadSignal);

            AmplitudeTextBox = 1.0;
            PeriodTextBox = 0.01;
            DurationTextBox = 0.02;

            ReadWindowsSetting();
            ApplyBase(_isDarkTheme);

            ReconstructComboBox = new CollectionView(new List<string>()
            {
                "From Samples",
                "From Quanta"
            });

            ReconstructionMethodComboBox = new CollectionView(new List<string>()
            {
                "Sinus Cardinalis",
                "First Order"
            });

            ReconstructButton = new RelayCommand(Reconstruct);
            RemoveLatestButton = new RelayCommand(RemoveLatest);

            #region Advanced Operations

            OperationTypeComboBox = new CollectionView(new List<string>()
            {
                "Convolution",
                "Correlation"
            });
            DoItButton = new RelayCommand(DoOperation);

            #endregion


        }

        #region ThemeSolvers

        private void ReadWindowsSetting()
        {
            //            var uiSettings = SystemParameters.WindowGlassBrush;
            //            var uiSettings1 = SystemParameters.WindowGlassColor;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            {
                object registryValueObject = key?.GetValue(RegistryValueName);
                if (registryValueObject == null)
                {
                    _isDarkTheme = false;
                }

                int registryValue;
                if (registryValueObject == null)
                {
                    registryValue = 0;
                }
                else
                {
                    registryValue = (int)registryValueObject;
                }

                _isDarkTheme = registryValue <= 0;
            }
        }

        private void ApplyBase(bool isDark)
        {
            new PaletteHelper().SetLightDark(isDark);
        }

        #endregion

        #region methods

        #region Advanced Operations Methods

        private void DoOperation()
        {
            if (string.IsNullOrEmpty(OperationTypeComboBoxSelected))
            {
                MessageBox.Show($"Please choose operation type", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int sig1 = 0;
            int sig2 = 0;
            try
            {
                sig1 = int.Parse(MainComboBoxSelected.Substring(0, 1));
                sig2 = int.Parse(AdditionalComboBoxSelected.Substring(0, 1));
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var signal1 = new DataHandler(_dataHandlers[sig1]);
            var signal2 = new DataHandler(_dataHandlers[sig2]);
            if (signal1.IsScattered)
            {
                signal1.SamplesY = signal1.Y;
                signal1.SamplesX = signal1.X;
            }
            if (signal2.IsScattered)
            {
                signal2.SamplesY = signal2.Y;
                signal2.SamplesX = signal2.X;
            }

            List<double> result;
            switch (OperationTypeComboBoxSelected)
            {
                case "Correlation":
                    signal2.SamplesY.Reverse();
                    goto case "Convolution";
                case "Convolution":
                
                    
                    result = SignalUtils.AdvancedOperations.DiscreteConvolution(signal1.SamplesY,
                        signal2.SamplesY);
                    /*result = SignalUtils.AdvancedOperations.DiscreteConvolution(new List<double>()
                        {
                            1,2,4,8
                        }, 
                        new List<double>()
                        {
                            5,4,3,2

                        });*/

                    //var temp = signal1.X.First() < signal2.X.First() ? signal1 : signal2;
                    //TODO: How to generate Xs?????
                    var xValues = ExtendXValues(signal1.SamplesX, result.Count);

                    _dataHandlers.Add(new DataHandler()
                    {
                        Y = result,
                        X = xValues,
                        IsScattered = true,
                        Signal = "result",
                        StartTime = xValues.First(),
                        EndTime = xValues.Last()
                    });
                    _dataHandlers.Last().GenerateStats();
                    break;
            }
            PopulateSignalsList();
            DrawChart();

        }

        private List<double> ExtendXValues(List<double> existing, int target)
        {
            double delta = existing[1] - existing[0];
            List<double> temp = new List<double>(existing);
            for (int i = existing.Count; i < target; i++)
            {
                temp.Add(temp[i-1] + delta);
            }

            return temp;
        }

        #endregion

        private void LoadSignal()
        {
            string path = String.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
            };

            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;
                DataHandler dataHandler = new DataHandler();
                dataHandler.Load(path);
                _dataHandlers.Add(dataHandler);
                PopulateSignalsList();
                DrawChart();

            }
        }

        private void SaveSignal()
        {
            try
            {
                string path = String.Empty;
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    RestoreDirectory = true,
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    path = saveFileDialog.FileName;
                    var option = int.Parse(MainComboBoxSelected.Substring(0, 1));
                    _dataHandlers[option].Save(path, SamplingFrequencyTextBox);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Reconstruct()
        {
            if (string.IsNullOrEmpty(ReconstructComboBoxSelected) || string.IsNullOrEmpty(ReconstructionMethodComboBoxSelected))
            {
                MessageBox.Show("All ComboBoxes have to contain value for this operation", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int option = 0;
            try
            {
                option = int.Parse(MainComboBoxSelected.Substring(0, 1));
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            DataHandler chart = _dataHandlers[option];

            List<double> quants = SignalUtils.Operations.Quantize(chart.SamplesY, QuantizationLevelsTextBox);
            var SampledValues = chart.SamplesY;

            

            List<double> ValuesToReconstruct = new List<double>();
            if (ReconstructComboBoxSelected.Contains("From Samples"))
            {
                ValuesToReconstruct = SampledValues;
            }
            if (ReconstructComboBoxSelected.Contains("From Quanta"))
            {
                ValuesToReconstruct = quants;
            }


            List<double> reconstructedSignal = new List<double>();
            if (ReconstructionMethodComboBoxSelected.Contains("Sinus Cardinalis"))
            {
                reconstructedSignal = SignalUtils.Operations.ReconstructCard(chart.SamplesX, ValuesToReconstruct, NumOfSamplesTextBox, SamplingFrequencyTextBox, StartTimeTextBox, StartTimeTextBox + DurationTextBox); ;
            }
            if (ReconstructionMethodComboBoxSelected.Contains("First Order"))
            {
                reconstructedSignal = SignalUtils.Operations.ReconstructFirstOrder(chart.SamplesX, ValuesToReconstruct, SamplingFrequencyTextBox, StartTimeTextBox, StartTimeTextBox + DurationTextBox);
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
                SeriesCollection.Add(new LineSeries()
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
                SeriesCollection.Add(new LineSeries()
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
                SeriesCollection.Add(new ScatterSeries()
                {
                    PointGeometry = new EllipseGeometry(),
                    StrokeThickness = 8,
                    Title = "Samples",
                    Values = sampleValues
                });
            }

        }

        private void RemoveLatest()
        {
            SeriesCollection.RemoveAt(SeriesCollection.Count-1);
        }

        private void LoadStatistics()
        {

            if (_dataHandlers.Count > 0)
            {
                var option = int.Parse(MainComboBoxSelected.Substring(0, 1));
                SignalAverageTextBox = _dataHandlers[option].Mean;
                SignalVarianceTextBox = _dataHandlers[option].Variance;
                SignalAbsoluteAverageTextBox = _dataHandlers[option].AbsMean;
                SignalAveragePowerTextBox = _dataHandlers[option].AvgPower;
                SignalRMSTextBox = _dataHandlers[option].Rms;

            }
        }

        private void PopulateSignalsList()
        {

            _signals = new List<string>();
            foreach (var handler in _dataHandlers)
            {
                _signals.Add($"{_dataHandlers.IndexOf(handler)}. {handler.Signal}");
            }

            SignalsComboBox = new CollectionView(_signals);
            AdditionalSignalsComboBox = new CollectionView(_signals);

            if (_signals.Count > 0)
            {
                MainComboBoxSelected = _signals.Last();
            }

        }

        private void RemoveChart()
        {

            try
            {
                var chosen = MainComboBoxSelected.Substring(0, 1);
                _dataHandlers.RemoveAt(int.Parse(chosen));
                _dataHandlers.TrimExcess();

            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            PopulateSignalsList();

            DrawChart();
        }

        private void GenerateChart()
        {
            try
            {
                dataHandler = new DataHandler(
                    AmplitudeTextBox,
                    DurationTextBox,
                    StartTimeTextBox,
                    PeriodTextBox,
                    SignalComboBoxSelected,
                    FillFactorTextBox,
                    UnitEventTextBox,
                    ProbabilityTextBox,
                    SamplingFrequencyTextBox

                );
                dataHandler.Call();
                _dataHandlers.Add(dataHandler);
                DrawChart();


            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            PopulateSignalsList();


        }

        private void Operations()
        {
            List<double> result = new List<double>();
            var first = int.Parse(MainComboBoxSelected.Substring(0, 1));
            var second = int.Parse(AdditionalComboBoxSelected.Substring(0, 1));
            bool isScattered = _dataHandlers[first].IsScattered;
            try
            {
                switch (OperationComboBoxSelected)
                {

                    case "Add":
                        result = SignalUtils.Operations.Add(_dataHandlers[first].Y, _dataHandlers[second].Y);
                        break;
                    case "Subtract":
                        result = SignalUtils.Operations.Subtract(_dataHandlers[first].Y, _dataHandlers[second].Y);
                        break;
                    case "Multiply":
                        result = SignalUtils.Operations.Multiply(_dataHandlers[first].Y, _dataHandlers[second].Y);
                        break;
                    case "Divide":
                        result = SignalUtils.Operations.Divide(_dataHandlers[first].Y, _dataHandlers[second].Y);
                        break;
                }
                _dataHandlers.Add(new DataHandler()
                {
                    Signal = "result",
                    X = _dataHandlers[0].X,
                    Y = result,
                    IsScattered = isScattered
                });
                DrawChart();

            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //PopulateSignalsList();
            //Zad2();

        }

        private void DrawHistogram()
        {

            LabelsHistogram = null;
            try
            {
                if (string.IsNullOrEmpty(MainComboBoxSelected))
                {
                    return;
                }
                var option = int.Parse(MainComboBoxSelected.Substring(0, 1));

                var HistData = _dataHandlers[option].ExtractHistogramData(NumberOfColumnsTB);
                HistogramValues = new ChartValues<double>();
                LabelsHistogram = new string[HistData.Count];
                for (int i = 0; i < HistData.Count; i++)
                {
                    LabelsHistogram[i] = $"<{Math.Round(HistData[i][0], 2) }, {Math.Round(HistData[i][1], 2) }>";
                    HistogramValues.Add(HistData[i][2]);
                }
                SeriesCollectionHistogram = new SeriesCollection
                {

                    new ColumnSeries()
                    {
                        Title = MainComboBoxSelected,
                        Values = HistogramValues,
                        ColumnPadding = 0.0
                    }

                };
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DrawChart()
        {
            Labels = null;
            try
            {

                SeriesCollection = new SeriesCollection();

                foreach (var entry in _dataHandlers)
                {

                    ChartValues<ObservablePoint> lineValues = new ChartValues<ObservablePoint>();
                    for (int i = 0; i < entry.X.Count; i++)
                    {
                        lineValues.Add(new ObservablePoint(entry.X[i], entry.Y[i]));
                    }
                    
                    if (entry.IsScattered)
                    {
                        SeriesCollection.Add(new ScatterSeries()
                        {
                            PointGeometry = new EllipseGeometry(),
                            StrokeThickness = 8,
                            Title = $"{_dataHandlers.IndexOf(entry)}. {entry.Signal}",
                            Values = lineValues
                        });
                    }
                    else
                    {
                        SeriesCollection.Add(new LineSeries()
                        {
                            Fill = Brushes.Transparent,
                            Title = $"{_dataHandlers.IndexOf(entry)}. {entry.Signal}",
                            Values = lineValues,
                            PointGeometry = null,

                        });


                    }


                }

            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResolveAdditionalValues(string value)
        {
            FillFactorTBVisibility = false;
            UnitEventTBVisibility = false;
            ProbabilityTBVisibility = false;

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
                case "Uniform Noise":
                    break;
                case "Gaussian Noise":
                    break;
                case "Impulse Noise":
                    ProbabilityTBVisibility = true;
                    break;
                case "Unit Impulse":
                    UnitEventTBVisibility = true;
                    break;

            }
        }

        #endregion

        
    }
}
