﻿using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Signal_generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
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

        public ICommand GenerateButton { get; }
        public ICommand RemoveButton { get; }
        public ICommand DoOperationButton { get; }

        #region props

        private ChartValues<ObservablePoint> Values { get; set; } = new ChartValues<ObservablePoint>();
        private ChartValues<double> HistogramValues { get; set; } = new ChartValues<double>();
        public RadioButtonsEnum DrawModeRadioBTN
        {
            get => _drawModeRadioBTN;
            set
            {
                _drawModeRadioBTN = value;
                OnPropertyChanged(nameof(DrawModeRadioBTN));
                ColumnsTBVisibility = DrawModeRadioBTN == RadioButtonsEnum.Histogram;
                DrawChart();
            }
        }

        public int NumberOfColumnsTB
        {
            get => _numberOfColumnsTb;
            set
            {
                _numberOfColumnsTb = value;
                OnPropertyChanged(nameof(NumberOfColumnsTB));
                DrawChart();
            }
        }



        public CollectionView SignalsComboBox { get; set; }
        public CollectionView AdditionalSignalsComboBox { get; set; }

        public CollectionView SignalComboBox { get; }
        public CollectionView OperationsComboBox { get; }
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
                if (DrawModeRadioBTN == RadioButtonsEnum.Histogram)
                {
                    DrawChart();
                }

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
        public bool ColumnsTBVisibility { get; set; }

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
                "Steady Noise",
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
            DrawModeRadioBTN = RadioButtonsEnum.LineSeries;
            RemoveButton = new RelayCommand(RemoveChart);
            DoOperationButton = new RelayCommand(Operations);

            AmplitudeTextBox = 1.0;
            PeriodTextBox = 1.0;
            DurationTextBox = 10.0;

        }

        #region methods

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
                AdditionalComboBoxSelected = _signals.Last();
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

        /*private void RemoveSignal(int key)
        {
            _dict.Remove(key);
        }*/

        private void Operations()
        {
            List<double> result = new List<double>();
            var first = int.Parse(MainComboBoxSelected.Substring(0, 1));
            var second = int.Parse(AdditionalComboBoxSelected.Substring(0, 1));
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
                    Y = result
                });
                DrawChart();

            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            PopulateSignalsList();

        }

        private void DrawChart()
        {
            Labels = null;
            try
            {
                SeriesCollection = new SeriesCollection();


                switch (DrawModeRadioBTN)
                {
                    case RadioButtonsEnum.LineSeries:
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
                                    PointGeometry = null
                                });

                            }
                        }

                        break;

                    case RadioButtonsEnum.Histogram:
                        var option = int.Parse(MainComboBoxSelected.Substring(0, 1));
                        HistogramValues = new ChartValues<double>();
                        var HistData = _dataHandlers[option].ExtractHistogramData(NumberOfColumnsTB);
                        Labels = new string[HistData.Count];
                        for (int i = 0; i < HistData.Count; i++)
                        {
                            Labels[i] = $"<{Math.Round(HistData[i][0], 2) }, {Math.Round(HistData[i][1], 2) }>";
                            HistogramValues.Add(HistData[i][2]);
                        }
                        SeriesCollection = new SeriesCollection
                        {

                            new ColumnSeries()
                            {
                                Title = MainComboBoxSelected,
                                Values = HistogramValues
                            }

                        };
                        break;

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
                case "Steady Noise":
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