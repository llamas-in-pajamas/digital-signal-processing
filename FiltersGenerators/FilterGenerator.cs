using System;
using System.Collections.Generic;
using SignalGenerators;

namespace FiltersGenerators
{
    public static class FilterGenerator
    {
        public static DataHandler LowPass(int m, double samplingFrequency, double cutFrequency, double customK = 0)
        {
            if (m % 2 == 0)
            {
                throw new ArgumentException("M parameter has to be odd");
            }
            var temp = new DataHandler();
            double k;
            if (customK == 0)
            {
                k = samplingFrequency / cutFrequency;
            }
            else
            {
                k = customK;

            }

            temp.IsScattered = true;
            for (int i = 1; i <= m; i++)
            {
                temp.X.Add(i);
                if (i == (m - 1) / 2)
                {
                    temp.Y.Add(2.0 / k);
                    continue;
                }
                temp.Y.Add(innerFunction(i, m, k));
            }

            return temp;
        }

        public static DataHandler MidPass(int m, double samplingFrequency, double cutFrequency)
        {
            var k = samplingFrequency / (samplingFrequency / 4 - cutFrequency);
            var lowPass = LowPass(m, samplingFrequency, cutFrequency, k);
            for (int i = 0; i < lowPass.Y.Count; i++)
            {
                var temp = lowPass.Y[i];
                lowPass.Y[i] = temp * 2 * Math.Sin(Math.PI * i / 2.0);
            }

            return lowPass;
        }
        public static DataHandler HighPass(int m, double samplingFrequency, double cutFrequency)
        {
            var k = samplingFrequency / (samplingFrequency / 2 - cutFrequency);
            var lowPass = LowPass(m, samplingFrequency, cutFrequency, k);
            for (int i = 0; i < lowPass.Y.Count; i++)
            {
                var temp = lowPass.Y[i];
                lowPass.Y[i] = temp * Math.Pow(-1.0, i);
            }

            return lowPass;
        }


        public static void HammingWindow(ref DataHandler filter, int m)
        {
            for (int i = 0; i < filter.Y.Count; i++)
            {
                var factor = 0.53836 - 0.46164 * Math.Cos(2 * Math.PI * i / m);
                var temp = filter.Y[i];
                filter.Y[i] = temp * factor;
            }
        }

        public static void HanningWindow(ref DataHandler filter, int m)
        {
            for (int i = 0; i < filter.Y.Count; i++)
            {
                var factor = 0.5 - 0.5 * Math.Cos(2 * Math.PI * i / m);
                var temp = filter.Y[i];
                filter.Y[i] = temp * factor;
            }
        }
        public static void BlackmanWindow(ref DataHandler filter, int m)
        {
            for (int i = 0; i < filter.Y.Count; i++)
            {
                var factor = 0.42 - 0.5 * Math.Cos(2 * Math.PI * i / m) + 0.08 * Math.Cos(4 * Math.PI * i / m);
                var temp = filter.Y[i];
                filter.Y[i] = temp * factor;
            }
        }


        private static double innerFunction(int n, int m, double k)
        {
            var func = n - (m - 1) / 2;
            var upper = Math.Sin(2 * Math.PI * func / k);
            var lower = Math.PI * func;
            return upper / lower;
        }
    }
}
