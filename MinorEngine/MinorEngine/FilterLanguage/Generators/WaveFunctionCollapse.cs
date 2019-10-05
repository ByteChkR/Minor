using System;
using Common;
using MinorEngine.debug;

/*
The MIT License(MIT)
Copyright(c) mxgmn 2016.
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
The software is provided "as is", without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In no event shall the authors or copyright holders be liable for any claim, damages or other liability, whether in an action of contract, tort or otherwise, arising from, out of or in connection with the software or the use or other dealings in the software.
*/

namespace MinorEngine.FilterLanguage.Generators
{
    public abstract class WaveFunctionCollapse
    {
        protected bool[][] Wave;

        protected int[][][] Propagator;
        private int[][][] _compatible;
        protected int[] Observed;

        private (int, int)[] _stack;
        private int _stacksize;

        protected Random Random;
        protected int Fmx, Fmy, T;
        protected bool Periodic;

        protected double[] Weights;
        private double[] _weightLogWeights;

        private int[] _sumsOfOnes;
        private double _sumOfWeights, _sumOfWeightLogWeights, _startingEntropy;
        private double[] _sumsOfWeights, _sumsOfWeightLogWeights, _entropies;

        protected WaveFunctionCollapse(int width, int height)
        {
            Fmx = width;
            Fmy = height;
        }

        //If having a fixed random buffer the size is: limit*wave.length+limit

        void Init()
        {
            Wave = new bool[Fmx * Fmy][];
            _compatible = new int[Wave.Length][][];
            for (int i = 0; i < Wave.Length; i++)
            {
                Wave[i] = new bool[T];
                _compatible[i] = new int[T][];
                for (int t = 0; t < T; t++) _compatible[i][t] = new int[4];
            }

            _weightLogWeights = new double[T];
            _sumOfWeights = 0;
            _sumOfWeightLogWeights = 0;

            for (int t = 0; t < T; t++)
            {
                _weightLogWeights[t] = Weights[t] * Math.Log(Weights[t]);
                _sumOfWeights += Weights[t];
                _sumOfWeightLogWeights += _weightLogWeights[t];
            }

            _startingEntropy = Math.Log(_sumOfWeights) - _sumOfWeightLogWeights / _sumOfWeights;

            _sumsOfOnes = new int[Fmx * Fmy];
            _sumsOfWeights = new double[Fmx * Fmy];
            _sumsOfWeightLogWeights = new double[Fmx * Fmy];
            _entropies = new double[Fmx * Fmy];

            _stack = new (int, int)[Wave.Length * T];
            _stacksize = 0;
        }

        bool? Observe()
        {
            double min = 1E+3;
            int argmin = -1;

            for (int i = 0; i < Wave.Length; i++)
            {
                if (OnBoundary(i % Fmx, i / Fmx)) continue;

                int amount = _sumsOfOnes[i];
                if (amount == 0) return false;

                double entropy = _entropies[i];
                if (amount > 1 && entropy <= min)
                {
                    double noise = 1E-6 * Random.NextDouble();
                    if (entropy + noise < min)
                    {
                        min = entropy + noise;
                        argmin = i;
                    }
                }
            }

            if (argmin == -1)
            {
                Observed = new int[Fmx * Fmy];
                for (int i = 0; i < Wave.Length; i++)
                for (int t = 0; t < T; t++)
                    if (Wave[i][t])
                    {
                        Observed[i] = t;
                        break;
                    }

                return true;
            }

            double[] distribution = new double[T];
            for (int t = 0; t < T; t++) distribution[t] = Wave[argmin][t] ? Weights[t] : 0;
            int r = distribution.Random(Random.NextDouble());

            bool[] w = Wave[argmin];
            for (int t = 0; t < T; t++)
                if (w[t] != (t == r))
                    Ban(argmin, t);

            return null;
        }

        protected void Propagate()
        {
            while (_stacksize > 0)
            {
                var e1 = _stack[_stacksize - 1];
                _stacksize--;

                int i1 = e1.Item1;
                int x1 = i1 % Fmx, y1 = i1 / Fmx;

                for (int d = 0; d < 4; d++)
                {
                    int dx = Dx[d], dy = Dy[d];
                    int x2 = x1 + dx, y2 = y1 + dy;
                    if (OnBoundary(x2, y2)) continue;

                    if (x2 < 0)
                        x2 += Fmx;
                    else if (x2 >= Fmx) x2 -= Fmx;
                    if (y2 < 0)
                        y2 += Fmy;
                    else if (y2 >= Fmy) y2 -= Fmy;

                    int i2 = x2 + y2 * Fmx;
                    int[] p = Propagator[d][e1.Item2];
                    int[][] compat = _compatible[i2];

                    for (int l = 0; l < p.Length; l++)
                    {
                        int t2 = p[l];
                        int[] comp = compat[t2];

                        comp[d]--;
                        if (comp[d] == 0) Ban(i2, t2);
                    }
                }
            }
        }


        public bool Run(int limit)
        {
            Random = new Random();
            return RunModel(limit);
        }

        private bool RunModel(int limit)
        {
            if (Wave == null) Init();

            Clear();

            for (int l = 0; l < limit || limit == 0; l++)
            {
                if (l % 250 == 0) Logger.Log("Starting Iteration: " + l, DebugChannel.Log);
                bool? result = Observe();
                if (result != null) return (bool) result;
                Propagate();
            }

            return true;
        }

        public bool Run(int seed, int limit)
        {
            Random = new Random(seed);

            return RunModel(limit);
        }

        protected void Ban(int i, int t)
        {
            Wave[i][t] = false;

            int[] comp = _compatible[i][t];
            for (int d = 0; d < 4; d++) comp[d] = 0;
            _stack[_stacksize] = (i, t);
            _stacksize++;

            _sumsOfOnes[i] -= 1;
            _sumsOfWeights[i] -= Weights[t];
            _sumsOfWeightLogWeights[i] -= _weightLogWeights[t];

            double sum = _sumsOfWeights[i];
            _entropies[i] = Math.Log(sum) - _sumsOfWeightLogWeights[i] / sum;
        }

        protected virtual void Clear()
        {
            for (int i = 0; i < Wave.Length; i++)
            {
                for (int t = 0; t < T; t++)
                {
                    Wave[i][t] = true;
                    for (int d = 0; d < 4; d++) _compatible[i][t][d] = Propagator[Opposite[d]][t].Length;
                }

                _sumsOfOnes[i] = Weights.Length;
                _sumsOfWeights[i] = _sumOfWeights;
                _sumsOfWeightLogWeights[i] = _sumOfWeightLogWeights;
                _entropies[i] = _startingEntropy;
            }
        }

        protected abstract bool OnBoundary(int x, int y);
        public abstract System.Drawing.Bitmap Graphics();

        protected static readonly int[] Dx = {-1, 0, 1, 0};
        protected static readonly int[] Dy = {0, 1, 0, -1};
        private static readonly int[] Opposite = {2, 3, 0, 1};
    }
}