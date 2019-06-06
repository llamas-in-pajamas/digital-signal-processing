using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.IntegralTransforms;
using SignalGenerators;


namespace ComplexUtils
{
    public class Signals
    {
        public static DataHandler GetCustomSignal(double duration, double sampling = 16)
        {
            var signal1 = new DataHandler(2.0, duration, 0, 2.0, "Sinus", 0, 0, 0, sampling);
            signal1.Call();
            var signal2 = new DataHandler(1.0, duration, 0, 1.0, "Sinus", 0, 0, 0, sampling);
            signal2.Call();
            var signal3 = new DataHandler(5.0, duration, 0, 0.5, "Sinus", 0, 0, 0, sampling);
            signal3.Call();
            var combined = SignalUtils.Operations.Add(signal1.SamplesY, SignalUtils.Operations.Add(signal2.SamplesY, signal3.SamplesY));
            return new DataHandler()
            {
                Signal = "Custom",
                SamplesY = combined,
                Y = combined,
                SamplesX = signal1.SamplesX,
                X = signal1.SamplesX,
                IsScattered = true,
                SamplingFrequency = signal1.SamplingFrequency,
                StartTime = signal1.StartTime,
                Duration = signal1.Duration,
            };
        }
    }
}
