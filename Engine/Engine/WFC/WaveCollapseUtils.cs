using System.Linq;

namespace Engine.WFC
{
    public static class WaveCollapseUtils
    {
        public static int Random(this double[] a, double r)
        {
            double sum = a.Sum();
            for (int j = 0; j < a.Length; j++)
            {
                a[j] /= sum;
            }

            int i = 0;
            double x = 0;

            while (i < a.Length)
            {
                x += a[i];
                if (r <= x)
                {
                    return i;
                }

                i++;
            }

            return 0;
        }

        public static long Power(int a, int n)
        {
            long product = 1;
            for (int i = 0; i < n; i++)
            {
                product *= a;
            }

            return product;
        }
    }
}