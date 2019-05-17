using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using FiltersGenerators;
using SignalGenerators;
using View.ViewModel.Base;

namespace View.ViewModel
{
    public class FilterVM : BaseVM
    {
        public MainWindowVM Parent { get; set; }
        private readonly AdvancedOperationsVM _advancedOperationsVm;

        public ObservableCollection<string> FilterTypeCombobox { get; }
        public string FilterTypeComboBoxSelected { get; set; }


        public double CutOffFrequencyTextBox { get; set; }
        public int MParameterTextBox { get; set; }

        public ObservableCollection<string> FiltersComboBox { get; set; }
        public string FiltersComboBoxSelected { get; set; }


        public ObservableCollection<string> WindowTypeCombobox { get; set; }
        public string WindowTypeComboBoxSelected { get; set; }
        public ICommand ApplyFilterBTN { get; }
        public ICommand GenerateFilter { get; }

        public FilterVM(MainWindowVM parent)
        {
            Parent = parent;
            _advancedOperationsVm = Parent.AdvancedOperationsVm;
            FilterTypeCombobox = new ObservableCollection<string>
            {
                "Low-pass filter",
                "Mid-pass filter",
                "High-pass filter"
            };
            WindowTypeCombobox = new ObservableCollection<string>
            {
                "No Window",
                "Hamming Window",
                "Hanning Window",
                "Blackman Window"
            };
            WindowTypeComboBoxSelected = WindowTypeCombobox[0];
            GenerateFilter = new RelayCommand(CreateFilter);
            ApplyFilterBTN = new RelayCommand(ApplyFilter);
        }

        private void ApplyFilter()
        {
            _advancedOperationsVm.IsFilter = true;
            _advancedOperationsVm.DoOperation();
        }

        private void CreateFilter()
        {
            DataHandler filter = null;
            try
            {
                switch (FilterTypeComboBoxSelected)
                {
                    case "Low-pass filter":
                        filter = FilterGenerator.LowPass(MParameterTextBox, Parent.SamplingFrequencyTextBox,
                            CutOffFrequencyTextBox);
                        break;
                    case "Mid-pass filter":
                        filter = FilterGenerator.MidPass(MParameterTextBox, Parent.SamplingFrequencyTextBox,
                            CutOffFrequencyTextBox);
                        break;
                    case "High-pass filter":
                        filter = FilterGenerator.HighPass(MParameterTextBox, Parent.SamplingFrequencyTextBox,
                            CutOffFrequencyTextBox);
                        break;
                    default:
                        throw new ArgumentException("Provide filter type!");

                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            switch (WindowTypeComboBoxSelected)
            {
                case "No Window":
                    break;
                case "Hamming Window":
                    FilterGenerator.HammingWindow(ref filter, MParameterTextBox);
                    break;
                case "Hanning Window":
                    FilterGenerator.HanningWindow(ref filter, MParameterTextBox);
                    break;
                case "Blackman Window":
                    FilterGenerator.BlackmanWindow(ref filter, MParameterTextBox);
                    break;
            }
            filter.Signal = FilterTypeComboBoxSelected + " " + WindowTypeComboBoxSelected;
            Parent.Filters.Add(filter);
            
        }

        internal void PopulateFiltersList()
        {

            var _filtersNames = new List<string>();
            foreach (var filter in Parent.Filters)
            {
                _filtersNames.Add($"{Parent.Filters.IndexOf(filter)}. {filter.Signal}");
            }

            FiltersComboBox = new ObservableCollection<string>(_filtersNames);
        }

    }
}