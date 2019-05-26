using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using SignalGenerators;
using View.ViewModel.Base;

namespace View.ViewModel
{
    public class ChartHistogramVM : BaseVM
    {
        public MainWindowVM Parent;

        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public SeriesCollection SeriesCollectionHistogram { get; set; } = new SeriesCollection();

        public string[] Labels { get; set; }
        public string[] LabelsHistogram { get; set; }

        public Func<double, string> YFormatter { get; set; }

        public ObservableCollection<string> DrawableCombobox { get; set; }
        public string DrawableComboBoxSelected { get; set; }

        public ObservableCollection<string> DrawedComboBox { get; set; } = new ObservableCollection<string>();
        public string DrawedComboBoxSelected { get; set; }

        private int _numberOfColumnsTb = 5;
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

        public ICommand DrawCommand { get; }
        public ICommand RemoveChartCommand { get; }
        public ICommand RemoveLatestButton { get; }

        public ChartHistogramVM(MainWindowVM parent)
        {
            Parent = parent;
            DrawCommand = new RelayCommand(DrawChart);
            RemoveChartCommand = new RelayCommand(RemoveChart);
            RemoveLatestButton = new RelayCommand(RemoveLatest);
        }

        private void DrawChart()
        {

            Labels = null;
            try
            {
                var type = DrawableComboBoxSelected.Substring(0, 1);
                var index = int.Parse(DrawableComboBoxSelected.Substring(1, 1));
                DataHandler chartToDraw = null;
                switch (type)
                {
                    case "S":
                        chartToDraw = Parent.DataHandlers[index];
                        break;
                    case "F":
                        chartToDraw = Parent.Filters[index];
                        break;

                }

                ChartValues<ObservablePoint> lineValues = new ChartValues<ObservablePoint>();
                for (int i = 0; i < chartToDraw.X.Count; i++)
                {
                    lineValues.Add(new ObservablePoint(chartToDraw.X[i], chartToDraw.Y[i]));
                }
                var title = $"{type}{index}. {chartToDraw.Signal}";

                if (chartToDraw.IsScattered)
                {
                    SeriesCollection.Add(new LineSeries()
                    {
                        Fill = Brushes.Transparent,
                        Title = title,
                        Values = lineValues,
                        PointGeometry = null,

                    });
                    SeriesCollection.Add(new ScatterSeries()
                    {
                        PointGeometry = new EllipseGeometry(),
                        StrokeThickness = 8,
                        Title = title,
                        Values = lineValues
                    });
                }
                else
                {
                    SeriesCollection.Add(new LineSeries()
                    {
                        Fill = Brushes.Transparent,
                        Title = title,
                        Values = lineValues,
                        PointGeometry = null,

                    });

                }
                DrawedComboBox.Add(title);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal void DrawHistogram()
        {

            LabelsHistogram = null;
            try
            {
                var signal = Parent.GetMainSelectedSignal().ExtractHistogramData(NumberOfColumnsTB);


                var histogramValues = new ChartValues<double>();
                LabelsHistogram = new string[signal.Count];
                for (int i = 0; i < signal.Count; i++)
                {
                    LabelsHistogram[i] = $"<{Math.Round(signal[i][0], 2) }, {Math.Round(signal[i][1], 2) }>";
                    histogramValues.Add(signal[i][2]);
                }
                SeriesCollectionHistogram = new SeriesCollection
                {

                    new ColumnSeries()
                    {
                        Title = Parent.MainComboBoxSelected,
                        Values = histogramValues,
                        ColumnPadding = 0.0
                    }

                };
            }
            catch (Exception e)
            {
                
                //MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void RemoveChart()
        {
            try
            {
                var chart = SeriesCollection.Where(c => c.Title == DrawedComboBoxSelected).ToList();
                foreach (var seriesView in chart)
                {
                    SeriesCollection.Remove(seriesView);
                }

                DrawedComboBox.Remove(DrawedComboBoxSelected);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveLatest()
        {
            SeriesCollection.RemoveAt(SeriesCollection.Count - 1);
        }
        internal void PopulateDrawableList()
        {
            var drawableNames = new List<string>();
            foreach (var filter in Parent.Filters)
            {
                drawableNames.Add($"F{Parent.Filters.IndexOf(filter)}. {filter.Signal}");
            }
            foreach (var handler in Parent.DataHandlers)
            {
                drawableNames.Add($"S{Parent.DataHandlers.IndexOf(handler)}. {handler.Signal}");
            }

            DrawableCombobox = new ObservableCollection<string>(drawableNames);
        }
    }
}