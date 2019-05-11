using SignalUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SignalGenerators
{
    public class DataHandler
    {
        //ARGUMENTS TAKEN FROM USER
        private double _duration;
        public double StartTime { get; set; }
        private double _period;
        public string Signal;
        public bool IsScattered = false;


        //GENERATED
        private double _delta;
        public double EndTime { get; set; }
        private Generator _generator = new Generator();

        public List<double> X = new List<double>();
        public List<double> Y = new List<double>();
        public List<double> SamplesX = new List<double>();
        public List<double> SamplesY = new List<double>();

        public double Mean { get; set; }
        public double AbsMean { get; set; }
        public double AvgPower { get; set; }
        public double Rms { get; set; }
        public double Variance { get; set; }

        private List<double> _ysToSave = new List<double>();
        private double _samplingFrequency;

        public DataHandler()
        {
           
        }

        public DataHandler(DataHandler other)
        {
           X = new List<double>(other.X);
           Y = new List<double>(other.Y);
           SamplesX = new List<double>(other.SamplesX);
           SamplesY = new List<double>(other.SamplesY);
           IsScattered = other.IsScattered;
           _period = other._period;
        }

        public DataHandler(double amplitude, double duration, double startTime, double period, string signal, double fillFactor, double sTime, double probability, double samplingFrequency)
        {
            _generator.Amplitude = amplitude;
            _generator.StartTime = startTime;
            _generator.Period = period;
            _generator.FillFactor = fillFactor;
            _generator.STime = sTime;
            _generator.Probability = probability;
            _samplingFrequency = samplingFrequency;
            _period = period;
            _duration = duration;
            StartTime = startTime;
            Signal = signal;
            EndTime = StartTime + _duration;

            if (signal == "Impulse Noise" || signal == "Unit Impulse")
            {
                IsScattered = true;
            }
        }

        public void Load(string path)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                Y = new List<double>();
                StartTime = reader.ReadDouble();
                double frequency = reader.ReadDouble();
                _period = 1.0 / frequency;

                for (int i = 0, n = reader.ReadInt32(); i < n; i++)
                {
                    Y.Add(reader.ReadDouble());
                }
                X = new List<double>();
                for (int i = 0; i < Y.Count; i++)
                {
                    X.Add(StartTime + i / frequency);
                }
                EndTime = X.Last();
                IsScattered = true;
                Signal = Path.GetFileName(path);
                GenerateStats();
            }
        }

        public void Save(string path, double samplingFrequency)
        {
            GenerateDataToSave(samplingFrequency);
            SaveToBinary(path);
            SaveToTxt(path);
        }

        public void GenerateDataToSave(double samplingFrequency)
        {
            if (IsScattered)
            {
                _samplingFrequency = 1.0 / _period;
                _ysToSave = Y;
                return;
            }

            _samplingFrequency = samplingFrequency;
            CalculateSamplesToSave(samplingFrequency);

        }

        private void CalculateSamplesToSave(double samplingFrequency)
        {
            double period = 1.0 / samplingFrequency;
            for (double i = StartTime; i <= EndTime; i += period)
            {
                for (int j = 1; j < X.Count; j++)
                {

                    if (i >= X[j - 1] && i < X[j])
                    {
                        _ysToSave.Add(Y[j - 1]);
                        break;
                    }
                }
            }
        }

        private void SaveToBinary(string path)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
            {
                writer.Write(StartTime);
                writer.Write(_samplingFrequency);
                writer.Write(_ysToSave.Count);
                foreach (double sample in _ysToSave) writer.Write(sample);
            }
        }

        private void SaveToTxt(string path)
        {
            path = Path.ChangeExtension(path, ".txt");
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("t_1[s]: " + StartTime);
                writer.WriteLine("f[Hz]: " + _samplingFrequency);
                writer.WriteLine("Y.Count: " + _ysToSave.Count);
                foreach (double sample in _ysToSave) writer.WriteLine(sample);
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
                case "Uniform Noise":
                    GenerateUniformNoise();
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
            GenerateStats();

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

        public void GenerateStats()
        {
            Mean = Statistics.AvgSignal(StartTime, EndTime, Y);
            AbsMean = Statistics.AbsAvgSignal(StartTime, EndTime, Y);
            Variance = Statistics.SignalVariance(StartTime, EndTime, Y);
            Rms = Statistics.RMSSignal(StartTime, EndTime, Y);
            AvgPower = Statistics.AvgSignalPower(StartTime, EndTime, Y);
        }

        private int CalculateNumberOfSamples()
        {
            if (IsScattered)
            {
                return (int)(Math.Abs(EndTime - StartTime) * _period);
            }

            return 5000;
        }

        //CALL METHODS
        private void GenerateUnitImpulse() //STime Needed
        {
            for (double i = StartTime; i < EndTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.UnitImpulse(i));
            }
            for (double i = StartTime; i < EndTime; i += 1 / _samplingFrequency)
            {
                SamplesX.Add(i);
                SamplesY.Add(_generator.UnitImpulse(i));
            }

        }
        private void GenerateImpulseNoise() //Probability needed
        {
            for (double i = StartTime; i < EndTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.ImpulseNoise());
            }
            for (double i = StartTime; i < EndTime; i += 1 / _samplingFrequency)
            {
                SamplesX.Add(i);
                SamplesY.Add(_generator.ImpulseNoise());
            }
        }
        private void GenerateGaussianNoise()
        {
            for (double i = StartTime; i < EndTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.GaussianNoise());
            }
            for (double i = StartTime; i < EndTime; i += 1 / _samplingFrequency)
            {
                SamplesX.Add(i);
                SamplesY.Add(_generator.GaussianNoise());
            }
        }
        private void GenerateUniformNoise()
        {
            for (double i = StartTime; i < EndTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.UniformNoise());
            }
            for (double i = StartTime; i < EndTime; i += 1 / _samplingFrequency)
            {
                SamplesX.Add(i);
                SamplesY.Add(_generator.UniformNoise());
            }
        }

        private void GenerateTriangular() //FillFactor needed
        {
            for (double i = StartTime; i < EndTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.Triangular(i));
            }
            for (double i = StartTime; i < EndTime; i += 1 / _samplingFrequency)
            {
                SamplesX.Add(i);
                SamplesY.Add(_generator.Triangular(i));
            }
        }
        private void GenerateUnitJump() //STime Needed
        {
            for (double i = StartTime; i < EndTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.UnitJump(i));
            }
            for (double i = StartTime; i < EndTime; i += 1 / _samplingFrequency)
            {
                SamplesX.Add(i);
                SamplesY.Add(_generator.UnitJump(i));
            }
        }
        private void GenerateSymetricRectangular() //FillFactor needed
        {
            for (double i = StartTime; i < EndTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.RectanguralSymetrical(i));
            }
            for (double i = StartTime; i < EndTime; i += 1 / _samplingFrequency)
            {
                SamplesX.Add(i);
                SamplesY.Add(_generator.RectanguralSymetrical(i));
            }
        }

        private void GenerateRectangular() //FillFactor needed
        {
            for (double i = StartTime; i < EndTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.Rectangural(i));
            }
            for (double i = StartTime; i < EndTime; i += 1 / _samplingFrequency)
            {
                SamplesX.Add(i);
                SamplesY.Add(_generator.Rectangural(i));
            }
        }

        private void GenerateSinus2P()
        {
            for (double i = StartTime; i < EndTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.Sinusoidal2P(i));
            }
            for (double i = StartTime; i < EndTime; i += 1 / _samplingFrequency)
            {
                SamplesX.Add(i);
                SamplesY.Add(_generator.Sinusoidal2P(i));
            }
        }

        private void GenerateSinus1P()
        {
            for (double i = StartTime; i < EndTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.Sinusoidal1P(i));
            }
            for (double i = StartTime; i < EndTime; i += 1 / _samplingFrequency)
            {
                SamplesX.Add(i);
                SamplesY.Add(_generator.Sinusoidal1P(i));
            }
        }

        private void GenerateSinus()
        {
            for (double i = StartTime; i < EndTime; i += _delta)
            {
                X.Add(i);
                Y.Add(_generator.Sinusoidal(i));
            }
            for (double i = StartTime; i < EndTime; i += 1 / _samplingFrequency)
            {
                SamplesX.Add(i);
                SamplesY.Add(_generator.Sinusoidal(i));
            }
        }
    }
}