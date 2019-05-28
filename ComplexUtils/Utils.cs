using System.Collections.Generic;
using System.Numerics;

namespace ComplexUtils
{
    public class Utils
    {
        public static List<Complex> ConvertRealToComplex(List<double> real)
        {
            var ret = new List<Complex>();
            foreach (var d in real)
            {
                ret.Add(new Complex(d, 0));
            }

            return ret;
        }

    }
}