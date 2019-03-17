using System;
using System.Collections.Generic;
using System.Linq;
using SignalUtils;

namespace Signal_generators
{
    public class DataHandler
    {
        //ARGUMENTS TAKEN FROM USER
        private double _duration;
        private double _startTime;
        private double _period;
        public string Signal;
        public bool IsScattered = false;


        //GENERATED
        private double _delta;
        private double _endTime;
        private Generator _generator = new Generator();

        public List<double> X = new List<double>();
        public List<double> Y = new List<double>();

        public double Mean { get; set; }
        public double AbsMean { get; set; }
        public double AvgPower { get; set; }
        public double Rms { get; set; }
        public double Variance { get; set; }

        public DataHandler()
        {

        }
        public DataHandler(double amplitude, double duration, double startTime, double period, string signal, double fillFactor, double sTime, double probability)
        {
            _generator.Amplitude = amplitude;
            _generator.StartTime = startTime;
            _generator.Period = period;
            _generator.FillFactor = fillFactor;
            _generator.STime = sTime;
            _generator.Probability = probability;

            _period = period;
            _duration = duration;
            _startTime = startTime;
            Signal = signal;
            _endTime = _startTime + _duration;

            if (signal == "Impulse Noise" || signal == "Unit Impulse")
            {
                IsScattered = true;
            }
        }

        public void Call()
        {

            Statistics.IsScattered = IsScattered;
            X.Clear();
            Y.Clear();
            int numberOfSamples = CalculateNumberOfSamples();
            _delta = _duration / numberOfSamples;
            switch (Signal)
            {
                case "Sinus":
                    GenerateSinus();
                    break;
                case "Sinus1P":
                    GenerateSinus1P();
                    break;
                case "Sinus2P":
                    GenerateSinus2P();
                    break;
                case "Rectangular":
                    GenerateRectangular();
                    break;
                case "Symetric Rectangular":
                    GenerateSymetricRectangular();
                    break;
                case "Unit Jump":
                    GenerateUnitJump();
                    break;
                case "Triangular":
                    GenerateTriangular();
                    break;
                case "Steady Noise":
                    GenerateSteadyNoise();
                    break;
                case "Gaussian Noise":
                    GenerateGaussianNoise();
                    break;
                case "Impulse Noise":
                    GenerateImpulseNoise();
                    break;
                case "Unit Impulse":
                    GenerateUnitImpulse();
                    break;
                default:
                    throw new NotImplementedException();
            }
            GenerateStats(Y);

        }

        public List<List<double>> ExtractHistogramData(int numberOfColumns)
        {
            if (numberOfColumns < 0)
            {
                throw new ArgumentException("Number of columns must be a positive integer.");
            }
            List<List<double>> histogramData = new List<List<double>>();
            double minValue = Y.Min();
            double delta = Math.Abs(Y.Max() - Y.Min()) / numberOfColumns;
            double lowerBound = 0;
            double upperBound = 0;
            for (double i = 0; i < numberOfColumns; i++)
            {
                List<double> data = new List<double>();
                lowerBound = minValue;
                upperBound = minValue + delta;
                data.Add(lowerBound);
                data.Add(upperBound);
                data.Add(Y.Count(n => (n >= lowerBound && n <= upperBound)));
                histogramData.Add(data);
                minValue += delta;
            }

            return histogramData;
        }

        private void GenerateStats(List<double> values)
        {
            Mean = Statistics.AvgSignal(_startTime, _endTime, Y);
            AbsMean = Statistics.AbsAvgSignal(_startTime, _endTime, Y);
            Variance = Statistics.SignalVariance(_startTime, _endTime, Y);
            Rms = Statistics.RMSSignal(_startTime, _endTime, Y);
            AvgPower = Statistics.AvgSignalPower(_startTime, _endTime, Y);
        }

        private int CalculateNumberOfSamples()
        {
            if (IsScattered)
            {
                return (int)(Math.Abs(_endTime - _startTime) * _period);
            }

            return 5000;
        }

        //CALL METHODS
        private void GenerateUnitImpulse() //STime Needed
        {
            for (double i = _startTime; i < _endTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.UnitImpulse(i));
            }
        }
        private void GenerateImpulseNoise() //Probability needed
        {
            for (double i = _startTime; i < _endTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.ImpulseNoise());
            }
        }
        private void GenerateGaussianNoise()
        {
            for (double i = _startTime; i < _endTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.GaussianNoise());
            }
        }
        private void GenerateSteadyNoise()
        {
            for (double i = _startTime; i < _endTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.SteadyNoise());
            }
            
        }

        private void GenerateTriangular() //FillFactor needed
        {
            for (double i = _startTime; i < _endTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.Triangular(i));
            }
        }
        private void GenerateUnitJump() //STime Needed
        {
            for (double i = _startTime; i < _endTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.UnitJump(i));
            }
        }
        private void GenerateSymetricRectangular() //FillFactor needed
        {
            for (double i = _startTime; i < _endTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.RectanguralSymetrical(i));
            }
        }

        private void GenerateRectangular() //FillFactor needed
        {
            for (double i = _startTime; i < _endTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.Rectangural(i));
            }
        }

        private void GenerateSinus2P()
        {
            for (double i = _startTime; i < _endTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.Sinusoidal2P(i));
            }
        }

        private void GenerateSinus1P()
        {
            for (double i = _startTime; i < _endTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.Sinusoidal1P(i));
            }
        }

        private void GenerateSinus()
        {
            for (double i = _startTime; i < _endTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.Sinusoidal(i));
            }
        }
    }
}