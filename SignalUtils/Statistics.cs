using System;

namespace SignalUtils
{
    public static class Statistics
    {
        public static double AvgSignal(double t1, double t2, Func<double, double> func)
        {
            return 1 / (t2 - t1) * Integral(t1, t2, func);
        }

        public static double SignalVariance(double t1, double t2, Func<double, double> func)
        {
            return 1 / (t2 - t1) * Integral(t1, t2, func, d => Math.Pow(d - AvgSignal(t1, t2, func), 2));

        }
        public static double AbsAvgSignal(double t1, double t2, Func<double, double> func)
        {
            return 1 / (t2 - t1) * Integral(t1, t2, func, Math.Abs);
        }

        public static double AvgSignalPower(double t1, double t2, Func<double, double> func)
        {
            return 1 / (t2 - t1) * Integral(t1, t2, func, d => d * d);
        }

        public static double RMSSignal(double t1, double t2, Func<double, double> func)
        {
            return Math.Sqrt(AvgSignalPower(t1, t2, func));
        }

        private static double Integral(double t1, double t2, Func<double, double> func, Func<double, double> additionalFunc = null)
        {
            var dx = (t2 - t1) / 1000;

            double integral = 0;
            for (int i = 0; i < 1000; i++)
            {
                if (additionalFunc != null)
                    integral += additionalFunc(func(t1 + i * dx));
                else
                    integral += func(t1 + i * dx);

            }
            integral *= dx;

            return integral;
        }

        private static double Sum(int n1, int n2, Func<double, double> func,
            Func<double, double> additionalFunc = null)
        {
            double sum = 0;
            for (int i = n1; i < n2; i++)
            {
                if (additionalFunc != null)
                    sum += additionalFunc(func(i));
                else
                    sum += func(i);
            }

            return sum;
        }
    }
}