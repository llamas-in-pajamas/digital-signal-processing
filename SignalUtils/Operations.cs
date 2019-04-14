using System;
using System.Collections.Generic;
using System.Linq;

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

        public static List<double> ReconstructCard(List<double> args, List<double> sampledSignal, int numberOfSamples, double samplingFrequency, double startTime, double endTime)
        {
            List<double> reconstructedSignal = new List<double>();
            double period = 1.0 / samplingFrequency;
            var samplePeriod = (endTime - startTime) / 5000;

            for (double i = startTime; i <= endTime; i+=samplePeriod)
            {
                double sum = 0;
                var between = FindBetween(i, args);
                if (between.Item1 == between.Item2)
                {
                    reconstructedSignal.Add(sampledSignal[between.Item1]);
                    continue;
                }

                int start = between.Item1 - numberOfSamples;
                int end = between.Item2 + numberOfSamples;
                if (start < 0)
                    start = 0;
                if (end > sampledSignal.Count - 1)
                    end = sampledSignal.Count - 1;
                for (int j = start; j <=end; j++)
                {
                    
                    //var temp = SinusCardinalis(i / period - between.Item1 + j);
                    sum += sampledSignal[j] * SinusCardinalis(i / period - j);

                }
                reconstructedSignal.Add(sum);
            }
            
            return reconstructedSignal;
        }

        public static List<double> ReconstructFirstOrder(List<double> args, List<double> sampledSignal, double samplingFrequency, double startTime, double endTime)
        {
            List<double> reconstructedSignal = new List<double>();
            double period = 1.0 / samplingFrequency;
            var samplePeriod = (endTime - startTime) / 5000;

            for (double i = startTime; i <= endTime; i += samplePeriod)
            {
                double sum = 0;
                var between = FindBetween(i, args);
                if (between.Item1 == between.Item2)
                {
                    reconstructedSignal.Add(sampledSignal[between.Item1]);
                    continue;
                }
               
                
                for (int j = 0; j < sampledSignal.Count; j++)
                {

                    //var temp = SinusCardinalis(i / period - between.Item1 + j);
                    sum += sampledSignal[j] * Math.Max(1- Math.Abs((i - j*period)/period), 0);

                }
                reconstructedSignal.Add(sum);
            }

            return reconstructedSignal;
        }

        

        private static (int, int) FindBetween(double arg, List<double> args)
        {
            for (int i = 1; i < args.Count; i++)
            {
                if (arg==args[i])
                {
                    return (i, i);
                }

                if (arg > args[i - 1] && arg < args[i])
                {
                    return (i - 1, i);
                }
            }

            return (0, 0);
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

            for (double i = min; i <= max; i += period)
            {
                List<double> temp = new List<double>(2);
                for (int j = 1; j < args.Count; j++)
                {
                    if (i >= args[j - 1] && i < args[j])
                    {
                        temp.Add(args[j - 1]);
                        temp.Add(values[j - 1]);
                        break;
                    }
                }

                signal.Add(temp);
            }

            return signal;
        }

        public static List<double> Quantize(List<double> values, int numberOfLevels)
        {
            double max = values.Max();
            double min = values.Min();
            return values.Select(n => Math.Round((n - min) / (max - min) * numberOfLevels) / numberOfLevels * (max - min) + min).ToList();
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
