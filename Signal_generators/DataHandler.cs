using System;
using System.Collections.Generic;

namespace Signal_generators
{
    public class DataHandler
    {
        private double _amplitude;
        private double _duration;
        private double _startTime;
        private double _period;
        private double _endTime;
        private Generator _generator = new Generator();
        public List<double> X = new List<double>();
        public List<double> Y = new List<double>();
        private string _signal;

        public double FillFactor { get; set; }
        
        public DataHandler(double amplitude, double duration, double startTime, double period, string signal)
        {
            _generator.Amplitude = amplitude;
            _generator.StartTime = startTime;
            _generator.Period = period;

            _period = period;
            _amplitude = amplitude;
            _duration = duration;
            _startTime = startTime;
            _signal = signal;
            _endTime = _startTime + _duration;
        }

        public void Call()
        {
            X.Clear();
            Y.Clear();
            int numberOfSamples = CalculateNumberOfSamples();
            switch (_signal)
            {
                case "Sinus":
                    GenerateSinus(numberOfSamples);
                    break;
                case "Sinus1P":
                    GenerateSinus1P(numberOfSamples);
                    break;
                case "Sinus2P":
                    GenerateSinus2P(numberOfSamples);
                    break;
                case "Rectangular":
                    GenerateRectangular(numberOfSamples);
                    break;
                case "Triangular":
                    GenerateTriangular(numberOfSamples);
                    break;
                case "Steady Noise":
                    GenerateSteadyNoise(numberOfSamples);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void GenerateSteadyNoise(int numberOfSamples)
        {
            double delta = _duration / numberOfSamples;
            for (double i = _startTime; i < _endTime; i += delta)
            {
                X.Add(i);
                Y.Add(_generator.SteadyNoise());
            }
        }

        private void GenerateTriangular(int numberOfSamples)
        {
            double delta = _duration / numberOfSamples;
            _generator.FillFactor = FillFactor;
            for (double i = _startTime; i < _endTime; i += delta)
            {
                X.Add(i);
                Y.Add(_generator.Triangular(i));
            }
        }

        private void GenerateRectangular(int numberOfSamples)
        {
            double delta = _duration / numberOfSamples;
            _generator.FillFactor = FillFactor;
            for (double i = _startTime; i < _endTime; i += delta)
            {
                X.Add(i);
                Y.Add(_generator.Rectangural(i));
            }
        }

        private void GenerateSinus2P(int numberOfSamples)
        {
            double delta = _duration / numberOfSamples;
            for (double i = _startTime; i < _endTime; i += delta)
            {
                X.Add(i);
                Y.Add(_generator.Sinusoidal2P(i));
            }
        }

        private void GenerateSinus1P(int numberOfSamples)
        {
            double delta = _duration / numberOfSamples;
            for (double i = _startTime; i < _endTime; i += delta)
            {
                X.Add(i);
                Y.Add(_generator.Sinusoidal1P(i));
            }
        }

        private int CalculateNumberOfSamples()
        {
            return (int)(Math.Abs(_endTime - _startTime) * _period);
        }
        
        private void GenerateSinus(int numberOfSamples)
        {
            double delta = _duration / numberOfSamples;
            for (double i = _startTime; i < _endTime; i+= delta)
            {
                X.Add(i);
                Y.Add(_generator.Sinusoidal(i));
            }

        }
    }
}