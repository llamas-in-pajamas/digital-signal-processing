using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalUtils
{
    public class QuantizedStatictics
    {
        public List<double> Original;
        public List<double> Reconstructed;

        public double MSE { get; set; }
        public double SNR { get; set; }
        public double MD { get; set; }
        public double PSNR { get; set; }
        public double ENOB { get; set; }

        public QuantizedStatictics(List<double> originalValues, List<double> reconstructedValues)
        {
            Original = new List<double>(originalValues);
            Reconstructed = new List<double>(reconstructedValues);
            Initialize();
        }

        public void Initialize()
        {
            MeanSquaredError();
            SignalToNoiseRatio();
            MaximumDifference();
            PeakSignalToNoiseRatio();
            EffectiveNumberOfBits();
        }

        private void MeanSquaredError()
        {
            int n = Reconstructed.Count;
            double fractional = 1.0 / n;
            double sum = 0;

            for (int i = 0; i < n; i++)
            {
                sum += Math.Pow((Original[i] - Reconstructed[i]), 2);
            }
            MSE = sum * fractional;
        }

        private void SignalToNoiseRatio()
        {
            double ratio = 10;
            double numerator = 0;
            double denominator = 0;
            int n = Reconstructed.Count;
            for (int i = 0; i < n; i++)
            {
                numerator += Math.Pow(Original[i], 2);
            }

            for (int i = 0; i < n; i++)
            {
                denominator += Math.Pow(Original[i] - Reconstructed[i], 2);
            }
            SNR = ratio * Math.Log10(numerator / denominator);
        }

        private void MaximumDifference()
        {
            int n = Reconstructed.Count;
            List<double> differences = new List<double>(n);

            for (int i = 0; i < n; i++)
            {
                differences.Add(Math.Abs(Original[i] - Reconstructed[i]));
            }
            MD = differences.Max();
        }

        private void PeakSignalToNoiseRatio()
        {
            double mse = MSE;
            double nominator = Original.Max();
            double ratio = 10;
            PSNR = ratio * Math.Log10(nominator / mse);
        }

        private void EffectiveNumberOfBits()
        {
            ENOB = (SNR - 1.76) / 6.02;
        }
    }
}
