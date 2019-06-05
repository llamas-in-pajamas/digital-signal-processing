using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;

namespace ComplexUtils
{
    public static class DITFFT
    {
        private static Dictionary<string, Complex> _factors = new Dictionary<string, Complex>();
        private static Dictionary<string, Complex> _factorsReverse = new Dictionary<string, Complex>();
        public static List<Complex> Transform(List<double> points)
        {
            List<Complex> transformed = new List<Complex>();
            int N = points.Count;
            List<Complex> splitted = new List<Complex>();
            int counter = 1;
            SplitSamples(points.Select(c => new Complex(c, 0)).ToList(), counter, ref splitted);
            OrderSamples(ref splitted);
            // transformed = SwitchSamples(splitted).ToList();

            return splitted.Select(c => c / N).ToList();
        }

        private static void OrderSamples(ref List<Complex> unordered)
        {
            int N = unordered.Count;
            int bits = 0;
            while (Math.Pow(2, bits) < N)
            {
                bits++;
            }
            List<int> order = new List<int>();
            for (int i = 0; i < N; i++)
            {
                byte val = (byte)i;
                BitArray bit = new BitArray(new[] { val });
                bool[] bitsarray = new bool[8];
                bit.CopyTo(bitsarray, 0);
                Array.Reverse(bitsarray);
                var delta = 8 - bits - 1;
                int k = 0;
                for (int j = bitsarray.Length - 1; k < bits / 2; j--)
                {
                    var temp = bitsarray[j];
                    bitsarray[j] = bitsarray[bitsarray.Length - j + delta];
                    bitsarray[bitsarray.Length - j + delta] = temp;
                    k++;
                }

                Array.Reverse(bitsarray);
                bit = new BitArray(bitsarray);
                byte[] temp1 = new byte[1];
                bit.CopyTo(temp1, 0);
                order.Add(temp1[0]);
            }
            var ordered = new List<Complex>();
            foreach (var i in order)
            {
                ordered.Add(unordered[i]);
            }

            unordered = ordered;

        }

        private static void SplitSamples(List<Complex> points, int counter, ref List<Complex> splitted)
        {
            List<Complex> firstHalf = points.Take(points.Count / 2).ToList();
            List<Complex> secondHalf = points.Skip(points.Count / 2).ToList();
            Complex[] result = new Complex[points.Count];
            for (int i = 0; i < firstHalf.Count; i++)
            {
                result[i] = firstHalf[i] + secondHalf[i];
                result[i + firstHalf.Count] = firstHalf[i] - secondHalf[i];
            }
            CalculateSamples(result.Take(result.Length / 2).ToList(), counter, ref splitted, false);
            CalculateSamples(result.Skip(result.Length / 2).ToList(), counter, ref splitted, true);
        }
        private static void CalculateSamples(List<Complex> points, int counter, ref List<Complex> splitted, bool calculateFactor)
        {

            if (calculateFactor)
            {
                for (int i = 1; i < points.Count; i++)
                {
                    points[i] *= CalculateFactor(i * counter, 1, points.Count);
                }
            }
            if (points.Count == 2)
            {
                splitted.Add(points[0] + points[1]);
                splitted.Add(points[0] - points[1]);
                return;
            }
            SplitSamples(points, counter * 2, ref splitted);
        }

        /*public static List<double> ReverseTransform(List<Complex> points)
        {
            List<double> transformed = new List<double>();
            int N = points.Count;

            transformed = SwitchSamples(points, true).Select(c => c.Real).ToList();
            return transformed;
        }*/


        /*public static List<Complex> SwitchSamples(List<Complex> points, bool reverse = false)
        {
            if (points.Count < 2)
            {
                return points;
            }
            List<Complex> oddPoints = new List<Complex>();
            List<Complex> evenPoints = new List<Complex>();
            for (int i = 0; i < points.Count / 2; i++)
            {
                evenPoints.Add(points[i * 2]);
                oddPoints.Add(points[i * 2 + 1]);
            }
            var result = Connect(SwitchSamples(evenPoints, reverse), SwitchSamples(oddPoints, reverse), reverse);
            return result;
        }*/
        private static List<Complex> Connect(List<Complex> evenPoints, List<Complex> oddPoints, bool reverse)
        {
            int N = oddPoints.Count * 2;
            Complex[] result = new Complex[N];
            for (int i = 0; i < oddPoints.Count; i++)
            {
                result[i] = evenPoints[i] + oddPoints[i];
                result[i + oddPoints.Count] = evenPoints[i] - oddPoints[i];
            }
            return result.ToList();
        }
        private static Complex CalculateFactor(int m, int n, int N)
        {
            return Complex.Exp(new Complex(0, -2 * Math.PI * m * n / N));
        }
        private static Complex CalculateReverseFactor(int m, int n, int N)
        {
            return Complex.Exp(new Complex(0, 2 * Math.PI * m * n / N));
        }
    }
}