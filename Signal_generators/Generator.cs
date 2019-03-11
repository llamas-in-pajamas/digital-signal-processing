using System;

namespace Signal_generators
{
    public class Generator
    {
        public double Amplitude { get; set; }
        public double Period { get; set; }
        public double StartTime { get; set; }
        public double Probability { get; set; } //For Impulse Noise
        public double STime { get; set; } //For Unit Jump and Unit Impulse
        public double FillFactor { get; set; }  //For Rectangular and Triangular
        private int KFactor { get; set; }

        private Random rand = new Random();


        //TODO: Gaussian noise check
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
            //TODO: BUT WHY I HAVE TO DO DIS?????
            time = Math.Round(time, 6);
            if (FillFactor < 0 || FillFactor > 1)
            {
                throw new ArgumentException("Fill Factor value has to be in range <0,1>");
            }
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
        public double RectanguralSymetrical(double time)
        {
            if (Rect(time)) return Amplitude;
            return -Amplitude;
        }

        public double Triangular(double time)
        {
            if (FillFactor <= 0 || FillFactor >= 1)
            {
                throw new ArgumentException("Fill Factor value has to be in range (0,1)");
            }
            if (Rect(time))
            {
                return Amplitude / (FillFactor * Period) * (time - KFactor * Period - StartTime);
            }
            return -Amplitude / (Period * (1 - FillFactor)) * (time - KFactor * Period - StartTime) +
                   (Amplitude / (1 - FillFactor));
        }

        public double UnitJump(double time)
        {
            //TODO: BUT WHY I HAVE TO DO DIS?????
            time = Math.Round(time, 6);
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
            //TODO: BUT WHY I HAVE TO DO DIS?????
            time = Math.Round(time, 6);
            if (time .Equals(STime) ) return Amplitude;
            return 0;
        }

        public double ImpulseNoise()
        {
            if (Probability < 0 || Probability > 1)
            {
                throw new ArgumentException("Probability value has to be in range <0,1>");
            }
            double temp = rand.NextDouble();
            if (Probability > temp)
            {
                return Amplitude;
            }

            return 0;
        }

    }
}
