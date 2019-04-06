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

        public static List<int> Quantize(List<double> values, int numberOfLevels)
        {
            List<double> copy = new List<double>(values);
            List<int> quantizedSignal = new List<int>(values.Count);
            double maximumValue = copy.Max();
            foreach(double value in copy)
            {
                quantizedSignal.Add((int)Math.Floor((value / maximumValue) * numberOfLevels));
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
    }
}
