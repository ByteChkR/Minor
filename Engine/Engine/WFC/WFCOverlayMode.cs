/*
The MIT License(MIT)
Copyright(c) mxgmn 2016.
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
The software is provided "as is", without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In no event shall the authors or copyright holders be liable for any claim, damages or other liability, whether in an action of contract, tort or otherwise, arising from, out of or in connection with the software or the use or other dealings in the software.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Engine.Debug;
using Engine.IO;

namespace Engine.WFC
{
    public class WfcOverlayMode : WaveFunctionCollapse
    {
        private readonly List<Color> colors;
        private readonly int ground;
        private readonly int n;
        private readonly byte[][] patterns;

        public WfcOverlayMode(Bitmap bitmap, int n, int width, int height, bool periodicInput, bool periodicOutput,
            int symmetry, int ground)
            : base(width, height)
        {
            n = n;
            Periodic = periodicOutput;

            Success = true;

            int smx = bitmap.Width, smy = bitmap.Height;
            byte[,] sample = new byte[smx, smy];
            colors = new List<Color>();

            for (int y = 0; y < smy; y++)
            for (int x = 0; x < smx; x++)
            {
                Color color = bitmap.GetPixel(x, y);

                int i = 0;
                foreach (Color c in colors)
                {
                    if (c == color)
                    {
                        break;
                    }

                    i++;
                }

                if (i == colors.Count)
                {
                    colors.Add(color);
                }

                sample[x, y] = (byte) i;
            }

            Logger.Log("Color Patterns found: " + colors.Count, DebugChannel.Log | DebugChannel.Wfc, 3);
            int colorsCount = colors.Count;
            long nPow = WaveCollapseUtils.Power(colorsCount, n * n);

            byte[] Pattern(Func<int, int, byte> f)
            {
                byte[] result = new byte[n * n];
                for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    result[x + y * n] = f(x, y);
                }

                return result;
            }

            byte[] PatternFromSample(int x, int y)
            {
                return Pattern((dx, dy) => sample[(x + dx) % smx, (y + dy) % smy]);
            }

            byte[] Rotate(byte[] p)
            {
                return Pattern((x, y) => p[n - 1 - y + x * n]);
            }

            byte[] Reflect(byte[] p)
            {
                return Pattern((x, y) => p[n - 1 - x + y * n]);
            }

            long Index(byte[] p)
            {
                long result = 0, power = 1;
                for (int i = 0; i < p.Length; i++)
                {
                    result += p[p.Length - 1 - i] * power;
                    power *= colorsCount;
                }

                return result;
            }

            byte[] PatternFromIndex(long ind)
            {
                long residue = ind, power = nPow;
                byte[] result = new byte[n * n];

                for (int i = 0; i < result.Length; i++)
                {
                    power /= colorsCount;
                    int count = 0;

                    if (power == 0)
                    {
                        continue;
                    }

                    while (residue >= power)
                    {
                        residue -= power;
                        count++;
                    }

                    result[i] = (byte) (count % colors.Count);
                }

                return result;
            }

            Dictionary<long, int> weights = new Dictionary<long, int>();
            List<long> ordering = new List<long>();

            for (int y = 0; y < (periodicInput ? smy : smy - n + 1); y++)
            for (int x = 0; x < (periodicInput ? smx : smx - n + 1); x++)
            {
                byte[][] ps = new byte[8][];

                ps[0] = PatternFromSample(x, y);
                ps[1] = Reflect(ps[0]);
                ps[2] = Rotate(ps[0]);
                ps[3] = Reflect(ps[2]);
                ps[4] = Rotate(ps[2]);
                ps[5] = Reflect(ps[4]);
                ps[6] = Rotate(ps[4]);
                ps[7] = Reflect(ps[6]);

                for (int k = 0; k < symmetry; k++)
                {
                    long ind = Index(ps[k]);
                    if (weights.ContainsKey(ind))
                    {
                        weights[ind]++;
                    }
                    else
                    {
                        weights.Add(ind, 1);
                        ordering.Add(ind);
                    }
                }
            }

            T = weights.Count;
            this.ground = (ground + T) % T;
            patterns = new byte[T][];
            Weights = new double[T];

            int counter = 0;
            foreach (long w in ordering)
            {
                patterns[counter] = PatternFromIndex(w);
                Weights[counter] = weights[w];
                counter++;
            }

            bool Agrees(byte[] p1, byte[] p2, int dx, int dy)
            {
                int xmin = dx < 0 ? 0 : dx,
                    xmax = dx < 0 ? dx + n : n,
                    ymin = dy < 0 ? 0 : dy,
                    ymax = dy < 0 ? dy + n : n;
                for (int y = ymin; y < ymax; y++)
                for (int x = xmin; x < xmax; x++)
                {
                    if (p1[x + n * y] != p2[x - dx + n * (y - dy)])
                    {
                        return false;
                    }
                }

                return true;
            }

            Propagator = new int[4][][];
            for (int d = 0; d < 4; d++)
            {
                Propagator[d] = new int[T][];
                for (int t = 0; t < T; t++)
                {
                    List<int> list = new List<int>();
                    for (int t2 = 0; t2 < T; t2++)
                    {
                        if (Agrees(patterns[t], patterns[t2], Dx[d], Dy[d]))
                        {
                            list.Add(t2);
                        }
                    }

                    Propagator[d][t] = new int[list.Count];
                    for (int c = 0; c < list.Count; c++)
                    {
                        Propagator[d][t][c] = list[c];
                    }
                }
            }
        }

        public WfcOverlayMode(string filename, int n, int width, int height, bool periodicInput, bool periodicOutput,
            int symmetry, int ground)
            : this(new Bitmap(IOManager.GetStream(filename)), n, width, height, periodicInput, periodicOutput, symmetry,
                ground)
        {
        }

        public bool Success { get; private set; }

        protected override bool OnBoundary(int x, int y)
        {
            return !Periodic && (x + n > Fmx || y + n > Fmy || x < 0 || y < 0);
        }

        public override Bitmap Graphics()
        {
            Bitmap result = new Bitmap(Fmx, Fmy);
            int[] bitmapData = new int[result.Height * result.Width];


            Success = true;

            if (Observed != null)
            {
                for (int y = 0; y < Fmy; y++)
                {
                    int dy = y < Fmy - n + 1 ? 0 : n - 1;
                    for (int x = 0; x < Fmx; x++)
                    {
                        int dx = x < Fmx - n + 1 ? 0 : n - 1;
                        Color c = colors[patterns[Observed[x - dx + (y - dy) * Fmx]][dx + dy * n]];
                        bitmapData[x + y * Fmx] = unchecked((int) 0xff000000 | (c.R << 16) | (c.G << 8) | c.B);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Wave.Length; i++)
                {
                    int contributors = 0, r = 0, g = 0, b = 0;
                    int x = i % Fmx, y = i / Fmx;

                    for (int dy = 0; dy < n; dy++)
                    for (int dx = 0; dx < n; dx++)
                    {
                        int sx = x - dx;
                        if (sx < 0)
                        {
                            sx += Fmx;
                        }

                        int sy = y - dy;
                        if (sy < 0)
                        {
                            sy += Fmy;
                        }

                        int s = sx + sy * Fmx;
                        if (OnBoundary(sx, sy))
                        {
                            continue;
                        }

                        for (int t = 0; t < T; t++)
                        {
                            if (Wave[s][t])
                            {
                                contributors++;
                                Color color = colors[patterns[t][dx + dy * n]];
                                r += color.R;
                                g += color.G;
                                b += color.B;
                            }
                        }
                    }

                    if (contributors == 0)
                    {
                        Success = false;
                        continue;
                    }

                    bitmapData[i] = unchecked((int) 0xff000000 | ((r / contributors) << 16) |
                                              ((g / contributors) << 8) | (b / contributors));
                }
            }

            BitmapData bits = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(bitmapData, 0, bits.Scan0, bitmapData.Length);
            result.UnlockBits(bits);

            return result;
        }

        protected override void Clear()
        {
            base.Clear();

            if (ground != 0)
            {
                for (int x = 0; x < Fmx; x++)
                {
                    for (int t = 0; t < T; t++)
                    {
                        if (t != ground)
                        {
                            Ban(x + (Fmy - 1) * Fmx, t);
                        }
                    }

                    for (int y = 0; y < Fmy - 1; y++)
                    {
                        Ban(x + y * Fmx, ground);
                    }
                }

                Propagate();
            }
        }
    }
}