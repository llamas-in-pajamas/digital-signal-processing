using System;

namespace Signal_generators
{
    public class Generator
    {
        public double Amplitude { get; set; }
        public double Period { get; set; }
        public double StartTime { get; set; }
        public double STime { get; set; } //For Unit Jump and Unit Impulse
        public double FillFactor { get; set; }  //For Rectangular and Triangular
        private int KFactor { get; set; }

        private Random rand = new Random();


        //TODO: All Discrete Signals + Gaussian noise
        //https://stackoverflow.com/questions/218060/random-gaussian-variables - Gaussian noise
        public double SteadyNoise()
        {
            return rand.NextDouble() * 2 * Amplitude - Amplitude;
        }

        //TODO: Make this crap working with our Amplitude
        public double GaussianNoise()
        {
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            return  Math.Sqrt(-2.0 * Math.Log(u1)) *
                                   Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        }
        private double Sinus(double time)
        {
            return Math.Sin(2 * Math.PI / Period * (time - StartTime));
        }

        public double Sinusoidal(double time)
        {
            return Amplitude * Sinus(time);
        }

        public double Sinusoidal1P(double time)
        {
            return 0.5 * Amplitude * (Sinus(time) + Math.Abs(Sinus(time)));
        }
        public double Sinusoidal2P(double time)
        {
            return Amplitude * Math.Abs(Sinus(time));
        }

        private bool Rect(double time)
        {
            KFactor = (int)(time / Period);
            if (time >= KFactor * Period + StartTime && time < FillFactor * Period + KFactor * Period + StartTime)
            {
                return true;
            }

            return false;
        }

        public double Rectangural(double time)
        {
            if (Rect(time)) return Amplitude;
            return 0;
        }
        public double RectanguralSimetrical(double time)
        {
            if (Rect(time)) return Amplitude;
            return -Amplitude;
        }

        public double Triangular(double time)
        {
            if (Rect(time))
            {
                return Amplitude / (FillFactor * Period) * (time - KFactor * Period - StartTime);
            }
            return -Amplitude / (Period * (1 - FillFactor)) * (time - KFactor * Period - StartTime) +
                   (Amplitude / (1 - FillFactor));
        }

        public double UnitJump(double time)
        {
            if (time > STime)
            {
                return Amplitude;
            }

            if (time == STime)
            {
                return 0.5 * Amplitude;
            }
            return 0;
        }

        //Discrete Signals

        public double UnitImpulse(double time)
        {
            if (time == STime) return Amplitude;
            return 0;
        }

        public double ImpulseNoise(double probability)
        {
            double temp = rand.NextDouble();
            if (probability > temp)
            {
                return Amplitude;
            }

            return 0;
        }

    }
}
