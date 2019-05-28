using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using SignalGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using FiltersGenerators;
using View.ViewModel.Base;

namespace View.ViewModel
{
    public class MainWindowVM : BaseVM
    {
        #region Other VMs

        public SonarVM SonarVm { get; set; }
        public SignalGeneratorVM SignalGeneratorVm { get; set; }
        public ChartHistogramVM ChartHistogramVm { get; set; }
        public ReconstructVM ReconstructVm { get; set; }
        public AdvancedOperationsVM AdvancedOperationsVm { get; set; }
        public FilterVM FilterVm { get; set; }
        public FourierVM FourierVm { get; set; }

        #endregion

        internal ObservableCollection<DataHandler> DataHandlers = new ObservableCollection<DataHandler>();
        internal ObservableCollection<DataHandler> Filters = new ObservableCollection<DataHandler>();

        //VM FIELDS
        private string _mainComboBoxSelected;
        private bool _isDarkTheme = true;
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string RegistryValueName = "AppsUseLightTheme";

        public double SamplingFrequencyTextBox { get; set; } = 1000;

        public ObservableCollection<string> SignalsComboBox { get; set; }
        
        public string MainComboBoxSelected
        {
            get => _mainComboBoxSelected;
            set
            {
                if (_mainComboBoxSelected == value) return;
                _mainComboBoxSelected = value;
                ChartHistogramVm.DrawHistogram();
                LoadStatistics();
                OnPropertyChanged(nameof(SignalsComboBox));
            }
        }

        public ObservableCollection<string> AdditionalSignalsComboBox { get; set; }
        public string AdditionalComboBoxSelected { get; set; }

        #region Signal Stats

        public double SignalAverageTextBox { get; set; }
        public double SignalVarianceTextBox { get; set; }
        public double SignalAbsoluteAverageTextBox { get; set; }
        public double SignalAveragePowerTextBox { get; set; }
        public double SignalRMSTextBox { get; set; }
        #endregion

      
        /// <summary>
        /// CTOR
        /// </summary>
        public MainWindowVM()
        {
            SonarVm = new SonarVM(this);
            SignalGeneratorVm = new SignalGeneratorVM(this);
            ChartHistogramVm = new ChartHistogramVM(this);
            ReconstructVm = new ReconstructVM(this);
            AdvancedOperationsVm = new AdvancedOperationsVM(this);
            FilterVm = new FilterVM(this);
            FourierVm = new FourierVM(this);

            ReadWindowsSetting();
            ApplyBase(_isDarkTheme);

            DataHandlers.CollectionChanged += (e, v) => OnDataHandlersChanged();
            Filters.CollectionChanged += (e, v) => OnFiltersChanged();

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

        private void OnDataHandlersChanged()
        {
            PopulateSignalsList();
            ChartHistogramVm.PopulateDrawableList();
        }

        private void OnFiltersChanged()
        {
            FilterVm.PopulateFiltersList();
            ChartHistogramVm.PopulateDrawableList();
        }

        internal DataHandler GetMainSelectedSignal()
        {
            var option = int.Parse(MainComboBoxSelected.Substring(0, 1));

            return DataHandlers[option];
        }
        internal DataHandler GetSelectedAdditionalSignal()
        {
            var option = int.Parse(AdditionalComboBoxSelected.Substring(0, 1));

            return DataHandlers[option];
        }
        internal DataHandler GetSelectedFilter()
        {
            var option = int.Parse(FilterVm.FiltersComboBoxSelected.Substring(0, 1));

            return Filters[option];
        }

        internal void PopulateSignalsList()
        {

            var signals = new List<string>();
            foreach (var handler in DataHandlers)
            {
                signals.Add($"{DataHandlers.IndexOf(handler)}. {handler.Signal}");
            }

            SignalsComboBox = new ObservableCollection<string>(signals);
            AdditionalSignalsComboBox = new ObservableCollection<string>(signals);

            if (signals.Count > 0)
            {
                MainComboBoxSelected = signals.Last();
            }

        }

        #region Zadanie 1

        private void LoadStatistics() //Only Method in Main Window
        {

            if (DataHandlers.Count > 0)
            {
                try
                {
                    var option = int.Parse(MainComboBoxSelected.Substring(0, 1));
                    SignalAverageTextBox = DataHandlers[option].Mean;
                    SignalVarianceTextBox = DataHandlers[option].Variance;
                    SignalAbsoluteAverageTextBox = DataHandlers[option].AbsMean;
                    SignalAveragePowerTextBox = DataHandlers[option].AvgPower;
                    SignalRMSTextBox = DataHandlers[option].Rms;

                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }
        #endregion

    }
}
