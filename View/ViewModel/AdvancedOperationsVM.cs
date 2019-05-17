using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using SignalGenerators;
using View.ViewModel.Base;

namespace View.ViewModel
{
    public class AdvancedOperationsVM : BaseVM
    {
        public MainWindowVM Parent { get; set; }

        public ICommand DoItButton { get; }

        public ObservableCollection<string> OperationTypeComboBox { get; }
        public string OperationTypeComboBoxSelected { get; set; }

        public bool IsFilter { get; set; } = false;


        public AdvancedOperationsVM(MainWindowVM parent)
        {
            Parent = parent;
            DoItButton = new RelayCommand(DoOperation);
            OperationTypeComboBox = new ObservableCollection<string>
            {
                "Convolution",
                "Correlation"
            };
        }

        internal void DoOperation()
        {
            if (string.IsNullOrEmpty(OperationTypeComboBoxSelected) && IsFilter == false)
            {
                MessageBox.Show($"Please choose operation type", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataHandler sig1;
            DataHandler sig2;

            try
            {
                sig1 = Parent.GetMainSelectedSignal();
                sig2 = IsFilter ? Parent.GetSelectedFilter() : Parent.GetSelectedAdditionalSignal();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error has occured: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var signal1 = new DataHandler(sig1);
            var signal2 = new DataHandler(sig2);
            if (IsFilter)
            {
                OperationTypeComboBoxSelected = "Convolution";
            }
            //Just to be sure
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

                    if (IsFilter)
                    {
                        result = result.Skip((signal2.SamplesY.Count - 1) / 2).Take(signal1.SamplesY.Count).ToList();
                    }

                    var temp = signal1.X.First() < signal2.X.First() ? signal1 : signal2;
                    var xValues = ExtendXValues(temp.SamplesX, result.Count);


                    Parent.DataHandlers.Add(new DataHandler()
                    {
                        Y = result,
                        X = xValues,
                        IsScattered = true,
                        Signal = $"{OperationTypeComboBoxSelected}: {Parent.MainComboBoxSelected} + {Parent.AdditionalComboBoxSelected}",
                        StartTime = xValues.First(),
                        EndTime = xValues.Last()
                    });

                    Parent.DataHandlers.Last().GenerateStats();
                    break;
            }
            IsFilter = false;

        }
        private List<double> ExtendXValues(List<double> existing, int target)
        {
            double delta = existing[1] - existing[0];
            List<double> temp = new List<double>(existing);
            for (int i = existing.Count; i < target; i++)
            {
                temp.Add(temp[i - 1] + delta);
            }

            return temp;
        }



    }
}