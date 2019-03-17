using System;
using System.Collections.Generic;

namespace SignalUtils
{
    public static class Statistics
    {
        public static bool IsScattered;
        public static double AvgSignal(double t1, double t2, List<double> values)
        {
            if (IsScattered)
            {
                return 1.0 / values.Count * Sum(values);
            }
            return 1 / (t2 - t1) * Integral(Math.Abs((t2 - t1) / values.Count), values);
        }

        public static double SignalVariance(double t1, double t2, List<double> values)
        {
            if (IsScattered)
            {
                return 1.0 / values.Count * Sum(values, d => Math.Pow(d - AvgSignal(t1, t2, values), 2));
            }
            return 1 / (t2 - t1) * Integral(Math.Abs((t2 - t1) / values.Count), values, d => Math.Pow(d - AvgSignal(t1, t2, values), 2));

        }
        public static double AbsAvgSignal(double t1, double t2, List<double> values)
        {
            if (IsScattered)
            {
                return 1.0 / values.Count * Sum(values, Math.Abs);
            }
            return 1 / (t2 - t1) * Integral(Math.Abs((t2 - t1) / values.Count), values, Math.Abs);
        }

        public static double AvgSignalPower(double t1, double t2, List<double> values)
        {
            if (IsScattered)
            {
                return 1.0 / values.Count * Sum(values, d => d * d);
            }
            return 1 / (t2 - t1) * Integral(Math.Abs((t2 - t1) / values.Count), values, d => d * d);
        }

        public static double RMSSignal(double t1, double t2, List<double> values)
        {
            return Math.Sqrt(AvgSignalPower(t1, t2, values));
        }

        private static double Integral(double dx, List<double> values, Func<double, double> additionalFunc = null)
        {
            double integral = 0;
            foreach (var value in values)
            {
                if (additionalFunc != null)
                    integral += additionalFunc(value);
                else
                    integral += value;
            }
            integral *= dx;

            return integral;
        }

        private static double Sum(List<double> values, Func<double, double> additionalFunc = null)
        {
            double sum = 0;
            foreach (var value in values)
            {
                if (additionalFunc != null)
                    sum += additionalFunc(value);
                else
                    sum += value;
            }

            return sum;
        }
    }
}