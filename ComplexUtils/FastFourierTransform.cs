using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ComplexUtils
{
    public static class FastFourierTransform
    {
        private static Dictionary<string, Complex> _factors = new Dictionary<string, Complex>();
        private static Dictionary<string, Complex> _factorsBack = new Dictionary<string, Complex>();

        public static List<Complex> Transform(List<Complex> points)
        {
            List<Complex> transformed = new List<Complex>();
            int N = points.Count;
            transformed = SwitchSamples(points);
            return transformed.Select(c => c / N).ToList();
        }
        public static List<double> TransformBack(List<Complex> points)
        {
            List<double> transformed = new List<double>();
            int N = points.Count;
            transformed = SwitchSamples(points, true).Select(c => c.Real).ToList();
            return transformed;
        }

        public static List<Complex> SwitchSamples(List<Complex> points, bool reverse = false)
        {
            if (points.Count < 2)
            {
                return points;
            }
            List<Complex> odd = new List<Complex>();
            List<Complex> even = new List<Complex>();
            for (int i = 0; i < points.Count / 2; i++)
            {
                even.Add(points[i * 2]);
                odd.Add(points[i * 2 + 1]);
            }
            var result = Connect(SwitchSamples(even, reverse), SwitchSamples(odd, reverse), reverse);
            return result;
        }
        private static List<Complex> Connect(List<Complex> evenPoints, List<Complex> oddPoints, bool reverse)
        {
            int N = oddPoints.Count * 2;
            Complex[] result = new Complex[N];
            for (int i = 0; i < oddPoints.Count; i++)
            {
                if (reverse)
                {
                    if (!_factorsBack.ContainsKey($"{i}, {N}"))
                    {
                        _factorsBack[$"{i}, {N}"] = CoreReverseFactor(i, 1, N);
                    }

                    result[i] = evenPoints[i] + (_factorsBack[$"{i}, {N}"] * oddPoints[i]);
                    result[i + oddPoints.Count] = evenPoints[i] - (_factorsBack[$"{i}, {N}"] * oddPoints[i]);
                }
                else
                {
                    if (!_factors.ContainsKey($"{i}, {N}"))
                        _factors[$"{i}, {N}"] = CoreFactor(i, 1, N);
                    result[i] = evenPoints[i] + oddPoints[i];
                    result[i + oddPoints.Count] = (evenPoints[i] - oddPoints[i]) * _factors[$"{i}, {N}"];
                }


            }
            return result.ToList();
        }
        private static Complex CoreFactor(int m, int n, int N)
        {
            return Complex.Exp(new Complex(0, -2 * Math.PI * m * n / N));
        }
        private static Complex CoreReverseFactor(int m, int n, int N)
        {
            return Complex.Exp(new Complex(0, 2 * Math.PI * m * n / N));
        }

    }
}