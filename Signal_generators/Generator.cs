using System;

namespace Signal_generators
{
    public class Generator
    {
        public double Amplitude { get; set; }
        public double Period { get; set; }
        public double Frequency { get; set; }
        public double StartTime { get; set; }
        public double Duration { get; set; }
        public double TimeS { get; set; }

        private Random rand = new Random();

        public double SteadyNoise()
        {
            return rand.NextDouble() * 2*Amplitude - Amplitude;
        }
        private double Sinus(int time)
        {
            return Math.Sin(((2 * Math.PI) / Period) * (time - StartTime));
        }

        public double Sinusoidal(int time)
        {
            return Amplitude * Sinus(time);
        }

        public double Sinusoidal1P(int time)
        {
            return 0.5 * Amplitude * (Sinus(time) + Math.Abs(Sinus(time)));
        }
        public double Sinusoidal2P(int time)
        {
            return  Amplitude * Math.Abs(Sinus(time));
        }

        public double Rectangural(int time)
        {

        }

        public double UnitJump(int time)
        {
            if (time > TimeS)
            {
                return Amplitude;
            }

            if (time == TimeS)
            {
                return 0.5 * Amplitude;
            }

            if (time < TimeS)
            {
                return 0;
            }

            return 0;
        }
       
    }
}
