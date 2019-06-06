using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SignalUtils;

namespace ComplexUtils
{
    public class Wavelet
    {
        private static List<double> H = new List<double>
        {
            0.32580343,
            1.01094572,
            0.8922014,
            -0.03957503,
            -0.26450717,
            0.0436163,
            0.0465036,
            -0.01498699
        };
        
        private static List<double> G = new List<double>
        {
            H[7],
            -H[6],
            H[5],
            -H[4],
            H[3],
            -H[2],
            H[1],
            -H[0],
        };
       
        public static List<Complex> Transform(List<double> points)
        {
            List<Complex> transformed = new List<Complex>();
            List<double> xh = AdvancedOperations.DiscreteConvolution(points, H).Take(points.Count).ToList();
            List<double> xg = AdvancedOperations.DiscreteConvolution(points, G).Take(points.Count).ToList();
            
            List<double> xhHalf = new List<double>();
            List<double> xgHalf = new List<double>();

            for (int i = 0; i < xh.Count; i++)
            {
                if (i % 2 == 0)
                {
                    xhHalf.Add(xh[i]);
                }
                else
                {
                    xgHalf.Add(xg[i]);
                }
            }
            for (int i = 0; i < xgHalf.Count; i++)
            {
                transformed.Add(new Complex(xhHalf[i], xgHalf[i]));
            }
            return transformed;
        }
        public static List<double> TransformBack(List<Complex> points)
        {
            var HRevesed = new List<double>(H);
            HRevesed.Reverse();
            var GReversed = new List<double>(G);
            GReversed.Reverse();
            List<double> xh = new List<double>();
            List<double> xg = new List<double>();
            for (int i = 0; i < points.Count; i++)
            {
                xh.Add(points[i].Real);
                xh.Add(0);
                xg.Add(0);

                xg.Add(points[i].Imaginary);
            }
            List<double> xhC = AdvancedOperations.DiscreteConvolution(xh, HRevesed).Take(xh.Count).ToList();
            List<double> xgC = AdvancedOperations.DiscreteConvolution(xg, GReversed).Take(xg.Count).ToList();
            return Operations.Add(xhC, xgC);
        }
    }
}