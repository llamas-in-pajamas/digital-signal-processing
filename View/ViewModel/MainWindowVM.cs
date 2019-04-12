using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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
using MaterialDesignThemes.Wpf;
using View.Helper;
using View.ViewModel.Base;
using SignalUtils;

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
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string RegistryValueName = "AppsUseLightTheme";


        public ICommand GenerateButton { get; }
        public ICommand RemoveButton { get; }
        public ICommand DoOperationButton { get; }
        public ICommand SaveButton { get; }
        public ICommand LoadButton { get; }


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

        public double SamplingFrequencyTextBox { get; set; }

        public CollectionView SignalsComboBox { get; set; }
        public CollectionView AdditionalSignalsComboBox { get; set; }

        public CollectionView SignalComboBox { get; }
        public CollectionView OperationsComboBox { get; }
        //Chart Props
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public SeriesCollection SeriesCollectionHistogram { get; set; } = new SeriesCollection();

        public string[] Labels { get; set; }
        public string[] LabelsHistogram { get; set; }

        public Func<double, string> YFormatter { get; set; }

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


        public double SignalAverageTextBox { get; set; }
        public double SignalVarianceTextBox { get; set; }
        public double SignalAbsoluteAverageTextBox { get; set; }
        public double SignalAveragePowerTextBox { get; set; }
        public double SignalRMSTextBox { get; set; }

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
            PeriodTextBox = 1.0;
            DurationTextBox = 10.0;

            ReadWindowsSetting();
            ApplyBase(_isDarkTheme);

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
                    ProbabilityTextBox

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
            //List<double> result = new List<double>();
            //var first = int.Parse(MainComboBoxSelected.Substring(0, 1));
            //var second = int.Parse(AdditionalComboBoxSelected.Substring(0, 1));
            //bool isScattered = _dataHandlers[first].IsScattered;
            //try
            //{
            //    switch (OperationComboBoxSelected)
            //    {

            //        case "Add":
            //            result = SignalUtils.Operations.Add(_dataHandlers[first].Y, _dataHandlers[second].Y);
            //            break;
            //        case "Subtract":
            //            result = SignalUtils.Operations.Subtract(_dataHandlers[first].Y, _dataHandlers[second].Y);
            //            break;
            //        case "Multiply":
            //            result = SignalUtils.Operations.Multiply(_dataHandlers[first].Y, _dataHandlers[second].Y);
            //            break;
            //        case "Divide":
            //            result = SignalUtils.Operations.Divide(_dataHandlers[first].Y, _dataHandlers[second].Y);
            //            break;
            //    }
            //    _dataHandlers.Add(new DataHandler()
            //    {
            //        Signal = "result",
            //        X = _dataHandlers[0].X,
            //        Y = result,
            //        IsScattered = isScattered
            //    });
            //    DrawChart();

            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}

            //PopulateSignalsList();
            Zad2();

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

        void Zad2()
        {
            var temp = SignalUtils.Operations.Sample(_dataHandlers[0].X, _dataHandlers[0].Y, SamplingFrequencyTextBox);
            List<double> args = new List<double>();
            List<double> vals = new List<double>();

            foreach(List<double> element in temp)
            {
                args.Add(element[0]);
                vals.Add(element[1]);
            }

            List<double> quantizedValues = SignalUtils.Operations.Quantize(vals, 255);

            SignalUtils.QuantizedStatictics stats = new SignalUtils.QuantizedStatictics(vals, quantizedValues);
            double mse = stats.MSE;
            double snr = stats.SNR;
            double md = stats.MD;
            double psnr = stats.PSNR;
            double enob = stats.ENOB;

            List<double> reconstructedSignal = SignalUtils.Operations.Reconstruct(args, quantizedValues, 50, SamplingFrequencyTextBox);

            _dataHandlers.Add(new DataHandler()
            {
                X = args,
                Y = reconstructedSignal
            });
            MessageBox.Show(mse + " " + snr + " " + md + " " + psnr);
            PopulateSignalsList();
            DrawChart();
        }
    }
}
