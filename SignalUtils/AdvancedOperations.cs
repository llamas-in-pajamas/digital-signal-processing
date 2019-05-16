using System.Collections.Generic;

namespace SignalUtils
{
    public static class AdvancedOperations
    {
        public static List<double> DiscreteConvolution(List<double> signal1, List<double> signal2)
        {
            List<double> result = new List<double>();
            int resultLength = signal1.Count + signal2.Count - 1;

            for (int i = 0; i < resultLength; i++)
            {
                double temp = 0.0;
                for (int j = 0; j < signal1.Count; j++)
                {
                    if (i - j < 0 || i - j > signal2.Count - 1)
                    {
                        continue;
                    }

                    temp += signal1[j] * signal2[i - j];
                }
                result.Add(temp);
            }

            return result;
        }

        public static List<double> DiscreteCorrelation(List<double> signal1, List<double> signal2)
        {
            List<double> reversed = new List<double>(signal1);
            reversed.Reverse();
            return DiscreteConvolution(reversed, signal2);
        }
    }
}