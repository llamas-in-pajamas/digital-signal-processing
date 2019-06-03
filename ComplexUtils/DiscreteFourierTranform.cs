using System;
using System.Collections.Generic;
using System.Numerics;

namespace ComplexUtils
{
    public class DiscreteFourierTranform
    {

        public static List<Complex> Transform(List<Complex> values)
        {
            var count = values.Count;
            if ((count != 0) && ((count & (count - 1)) != 0))
            {
                throw new ArgumentException("Number of values has to be power of 2");
            }
            var ret = new List<Complex>();

            for (int i = 0; i < count; i++)
            {
                ret.Add(TransformValue(i, values) / count);
            }

            return ret;

        }

        public static List<double> TransformBack(List<Complex> values)
        {
            var count = values.Count;
            if ((count != 0) && ((count & (count - 1)) != 0))
            {
                throw new ArgumentException("Number of values has to be power of 2");
            }
            var ret = new List<double>();

            for (int i = 0; i < count; i++)
            {
                ret.Add(TransformValueBack(i, values));
            }

            return ret;

        }

        private static Complex TransformValue(int m, List<Complex> values)
        {
            Complex ret = 0;
            var count = values.Count;
            for (int i = 0; i < count; i++)
            {
                ret += new Complex(values[i].Real, values[i].Imaginary) * CoreFactor(m, i, count);
            }

            return ret;
        }

        private static double TransformValueBack(int m, List<Complex> values)
        {
            double ret = 0;
            var count = values.Count;
            for (int i = 0; i < count; i++)
            {
                ret += (values[i] * ReverseCoreFactor(m, i, count)).Real;
            }

            return ret;
        }


        private static Complex CoreFactor(int m, int n, int N)
        {
            return Complex.Exp(new Complex(0, -2 * Math.PI * m * n / N));
        }

        private static Complex ReverseCoreFactor(int m, int n, int N)
        {
            return Complex.Exp(new Complex(0, 2 * Math.PI * m * n / N));
        }
    }
}