using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalUtils
{
    public static class Operations
    {
        private static void ListValidator(List<double> list1, List<double> list2)
        {
            if (list1.Count != list2.Count)
            {
                throw new ArgumentException();
            }
        }
        public static List<double> Add(List<double> list1, List<double> list2)
        {
            ListValidator(list1, list2);
            List<double> list = new List<double>(list1.Count);
            for (int i = 0, length = list1.Count; i < length; i++)
            {
                list.Add(list1[i] + list2[i]);
            }

            return list;
        }

        public static List<double> Subtract(List<double> list1, List<double> list2)
        {
            ListValidator(list1, list2);
            List<double> list = new List<double>(list1.Count);
            for (int i = 0, length = list1.Count; i < length; i++)
            {
                list.Add(list1[i] - list2[i]);
            }

            return list;
        }

        public static List<double> Multiply(List<double> list1, List<double> list2)
        {
            ListValidator(list1, list2);
            List<double> list = new List<double>(list1.Count);
            for (int i = 0, length = list1.Count; i < length; i++)
            {
                list.Add(list1[i] * list2[i]);
            }

            return list;
        }

        public static List<List<double>> Sample(List<double> args, List<double> values, double samplingFrequency)
        {
            double period = 1.0 / samplingFrequency;
            double min = args.Min();
            double max = args.Max();
            List<List<double>> signal = new List<List<double>>();

            for(double i = min; i <= max; i += period)
            {
                List<double> temp = new List<double>(2);
                for(int j = 1; j < args.Count; j++)
                {
                    if (i >= args[j - 1] && i < args[j])
                    {
                        temp.Add(args[j]);
                        temp.Add(values[j - 1]);
                        break;
                    }
                }
                signal.Add(temp);
            }

            return signal;
        }

        public static List<int> Quantize(List<double> values, int numberOfLevels)
        {
            List<double> copy = new List<double>(values);
            List<int> quantizedSignal = new List<int>(values.Count);
            double maximumValue = copy.Max();
            foreach(double value in copy)
            {
                double temp = value / maximumValue;
                double quntizedValue = Math.Ceiling(temp * numberOfLevels);
                if (temp.Equals(0.5) && temp % 2 != 0) temp++;
                quantizedSignal.Add((int)temp);
            }
            return quantizedSignal;
        }

        public static List<double> Divide(List<double> list1, List<double> list2)
        {
            ListValidator(list1, list2);

            List<double> list = new List<double>(list1.Count);
            for (int i = 0, length = list1.Count; i < length; i++)
            {
                list.Add(list1[i] / list2[i]);
            }

            return list;
        }

        private static double SinusCardinalis(double t)
        {
            if (t.Equals(0))
            {
                return 1;
            }

            return Math.Sin(Math.PI * t) / (Math.PI * t);
        }
    }
}
