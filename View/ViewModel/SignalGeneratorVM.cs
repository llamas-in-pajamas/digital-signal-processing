using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using SignalGenerators;
using View.ViewModel.Base;

namespace View.ViewModel
{
    public class SignalGeneratorVM : BaseVM
    {
        public MainWindowVM Parent { get; set; }
        #region props
        public ObservableCollection<string> SignalComboBox { get; }

        private string _signalComboBoxSelected;
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

        public ObservableCollection<string> OperationsComboBox { get; }
        public string OperationComboBoxSelected { get; set; }

        public double FillFactorTextBox { get; set; }
        public double AmplitudeTextBox { get; set; } = 1.0;
        public double PeriodTextBox { get; set; } = 0.01;
        public double DurationTextBox { get; set; } = 0.02;
        public double StartTimeTextBox { get; set; }
        public double UnitEventTextBox { get; set; }
        public double ProbabilityTextBox { get; set; }

        #region Visibilty props
        public bool FillFactorTBVisibility { get; set; }
        public bool UnitEventTBVisibility { get; set; }
        public bool ProbabilityTBVisibility { get; set; }
        #endregion

        #endregion
        public ICommand GenerateButton { get; }
        public ICommand RemoveButton { get; }
        public ICommand DoOperationButton { get; }
        public ICommand SaveButton { get; }
        public ICommand LoadButton { get; }


        public SignalGeneratorVM(MainWindowVM parent)
        {
            Parent = parent;
            SignalComboBox = new ObservableCollection<string>
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
            OperationsComboBox = new ObservableCollection<string>
            {
                "Add",
                "Subtract",
                "Multiply",
                "Divide"
            };
            SignalComboBoxSelected = SignalComboBox[0];
            OperationComboBoxSelected = OperationsComboBox[0];
            GenerateButton = new RelayCommand(GenerateSignal);
            RemoveButton = new RelayCommand(RemoveSignal);
            DoOperationButton = new RelayCommand(Operations);
            SaveButton = new RelayCommand(async () => await Task.Run(() => SaveSignal()));
            LoadButton = new RelayCommand(LoadSignal);
        }

        private void GenerateSignal()
        {
            try
            {
                var dataHandler = new DataHandler(
                    AmplitudeTextBox,
                    DurationTextBox,
                    StartTimeTextBox,
                    PeriodTextBox,
                    SignalComboBoxSelected,
                    FillFactorTextBox,
                    UnitEventTextBox,
                    ProbabilityTextBox,
                    Parent.SamplingFrequencyTextBox

                );
                dataHandler.Call();
                Parent.DataHandlers.Add(dataHandler);

            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void RemoveSignal()
        {
            try
            {
                var chosen = Parent.GetMainSelectedSignal();
                Parent.DataHandlers.Remove(chosen);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Operations()
        {
            List<double> result = new List<double>();
            try
            {
                var first = Parent.GetMainSelectedSignal();
                var second = Parent.GetSelectedAdditionalSignal();
                switch (OperationComboBoxSelected)
                {

                    case "Add":
                        result = SignalUtils.Operations.Add(first.SamplesY, second.SamplesY);
                        break;
                    case "Subtract":
                        result = SignalUtils.Operations.Subtract(first.SamplesY, second.SamplesY);
                        break;
                    case "Multiply":
                        result = SignalUtils.Operations.Multiply(first.SamplesY, second.SamplesY);
                        break;
                    case "Divide":
                        result = SignalUtils.Operations.Divide(first.SamplesY, second.SamplesY);
                        break;
                }

                Parent.DataHandlers.Add(new DataHandler()
                {
                    Signal = "result",
                    X = first.SamplesX,
                    SamplesX = first.SamplesX,
                    Y = result,
                    SamplesY = result,
                    IsScattered = true,
                    SamplingFrequency = first.SamplingFrequency,
                    StartTime = first.StartTime,
                    Duration = first.Duration,

                });

            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveSignal()
        {
            try
            {
                string path = string.Empty;
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    RestoreDirectory = true,
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    path = saveFileDialog.FileName;
                    var option = int.Parse(Parent.MainComboBoxSelected.Substring(0, 1));
                    Parent.DataHandlers[option].Save(path, Parent.SamplingFrequencyTextBox);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSignal()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var path = openFileDialog.FileName;
                var dataHandler = new DataHandler();
                dataHandler.Load(path);
                Parent.DataHandlers.Add(dataHandler);
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
    }
}