using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using SignalGenerators;
using View.ViewModel.Base;

namespace View.ViewModel
{
    public class SonarVM : BaseVM
    {
        private Timer _timer;
        private DataHandler _signal;
        #region Props

        

        #region Controls

        //sonar settings
        public double SignalSpeed { get; set; } = 300;
        public double ReportFrequency { get; set; } = 1;

        //signal source settings
        public double DistanceFromSource { get; set; }
        public double SourceSpeed { get; set; } = 10;

        //Button
        public ICommand ButtonCommand { get; set; }
        public string ButtonContent { get; set; }

        //Result
        public double CalculatedDistance { get; set; }

        public bool IsRunning { get; set; }

        #endregion

        #region Charts

        public SeriesCollection OriginalSeries { get; set; }
        public SeriesCollection DelayedSeries { get; set; } 
        public ChartValues<double> DelayedValues { get; set; }
        public SeriesCollection CorrelationSeries { get; set; }
        public ChartValues<double> CorrelationValues { get; set; }

        #endregion

        #region VM props

        public MainWindowVM Parent { get; set; }

        #endregion

        #endregion


        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="parent"></param>
        public SonarVM(MainWindowVM parent)
        {
            Parent = parent;
            ButtonContent = "Start Simulation";
            ButtonCommand = new RelayCommand(Start);
        }

        #region Methods

        private void Start()
        {
            OriginalSeries = new SeriesCollection();
            DelayedSeries = new SeriesCollection();
            CorrelationSeries = new SeriesCollection();
            CalculatedDistance = 0;
            
            int signal;
            try
            {
                signal = int.Parse(Parent.MainComboBoxSelected.Substring(0, 1));
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _signal = Parent._dataHandlers[signal];
            IsRunning = true;
            ButtonCommand = new RelayCommand(Stop);
            ButtonContent = "Stop Simulation";
            DelayedValues = new ChartValues<double>(_signal.Y);
            var correlated = SignalUtils.AdvancedOperations.DiscreteCorrelation(_signal.Y, _signal.Y);
            CorrelationValues = new ChartValues<double>(correlated);
            Draw(OriginalSeries, new ChartValues<double>(_signal.Y));
            Draw(DelayedSeries, DelayedValues);
            Draw(CorrelationSeries, CorrelationValues);

            try
            {
                _timer = new Timer((1 / ReportFrequency) * 1000);
                _timer.Elapsed += OnTimer;
                _timer.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
           
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            DistanceFromSource += SourceSpeed * (1 / ReportFrequency);

            int delta = (int) (DistanceFromSource / SignalSpeed * _signal.SamplingFrequency * 2);
            int left = _signal.Y.Count - delta;
            
            var recieved = _signal.Y.Skip(left).ToList();
            var buffor = _signal.Y.Take(left).ToList();
            recieved.AddRange(buffor);
            var correlated = SignalUtils.AdvancedOperations.DiscreteCorrelation(_signal.Y, recieved);
            DrawDelayed(recieved);
            DrawCorrelated(correlated);

            List<double> rightHalf = correlated.Skip((correlated.Count - 1) / 2).ToList();
            int maximum = rightHalf.FindIndex(c => Math.Abs(c - rightHalf.Max()) < 0.000001);
            CalculatedDistance = SignalSpeed * (maximum / _signal.SamplingFrequency) / 2;
        }

        private void Stop()
        {
            _timer.Stop();
            ButtonCommand = new RelayCommand(Start);
            ButtonContent = "Start Simulation";
            IsRunning = false;
        }

        private void Draw(SeriesCollection chart, ChartValues<double> YValues)
        {
            ChartValues<double> temp = YValues;
            chart.Add(new LineSeries()
            {
                Fill = Brushes.Transparent,
                Values = temp,
                PointGeometry = null,

            });
            
        }

        private void DrawDelayed(List<double> delayedSignal)
        {
            for (int i = 0; i < delayedSignal.Count; i++)
            {
                DelayedValues[i] = delayedSignal[i];
            }
        }
        private void DrawCorrelated(List<double> correlatedSignal)
        {
            for (int i = 0; i < correlatedSignal.Count; i++)
            {
                CorrelationValues[i] = correlatedSignal[i];
            }
        }

        #endregion

    }
}