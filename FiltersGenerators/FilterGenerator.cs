using System;
using System.Collections.Generic;
using SignalGenerators;

namespace FiltersGenerators
{
    public static class FilterGenerator
    {
        public static DataHandler LowPass(int m, double samplingFrequency, double cutFrequency)
        {
            if (m % 2 == 0)
            {
                throw new ArgumentException("M parameter has to be odd");
            }
            var temp = new DataHandler();
            var k = getK(samplingFrequency, cutFrequency);
            temp.IsScattered = true;
            for (int i = 0; i < m; i++)
            {
                temp.X.Add(i);
                if (i == (m - 1) / 2)
                {
                    temp.Y.Add(2 / k);
                    continue;
                }
                temp.Y.Add(innerFunction(i, m, k));
            }

            return temp;
        }

        private static double getK(double samplingFrequency, double cutFrequency)
        {
            return samplingFrequency / cutFrequency;
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
