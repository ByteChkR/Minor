using System;
using System.Collections.Generic;
using OpenTK;

namespace Engine.Core
{
    public static class Interpolations
    {

        public static float FakePow(float basis, float exp)
        {
            float pow = IntPow(basis, (int)exp);
            float powPlus1 = pow * basis;
            return Mix(pow, powPlus1, (int)exp - exp); //Mix by the remainder: 99% accurate in range [0, 1]
        }

        public static float IntPow(float basis, int exp)
        {
            float r = 1;
            for (int i = 0; i < exp; i++)
            {
                r *= basis;
            }

            return r;
        }

        public static float InverseLerp(float min, float max, float value)
        {
            if (Math.Abs(max - min) < float.Epsilon) return min;
            return (value - min) / (max - min);
        }

        public static float Slerp(float t)
        {
            return t * t;
        }

        public static float Flip(float t)
        {
            return 1 - t;
        }

        public static float Scale(float t, float scale)
        {
            return t * scale;
        }

        public static float Mix(float a, float b, float weightB)
        {
            return a + weightB * (b - a);
        }

        public static float SmoothStart(float t, int smoothness = 1)
        {
            float ret = t;
            for (int i = 0; i < smoothness; i++)
            {
                ret *= t;
            }

            return ret;
        }

        public static float SmoothStart(float t, float smoothness = 1)
        {
            int pow = (int)smoothness;
            return Mix(SmoothStart(t, pow), SmoothStart(t, pow + 1), smoothness - pow);
        }

        public static float SmoothStop(float t, int smoothness = 1)
        {
            return Flip(SmoothStart(Flip(t), smoothness));
        }

        public static float SmoothStop(float t, float smoothness = 1)
        {
            int pow = (int)smoothness;
            return Mix(SmoothStop(t, pow), SmoothStop(t, pow + 1), smoothness - pow);
        }

        public static float SmoothStep(float t, float smoothnessStart = 1, float smoothnessStop = 1)
        {
            return Mix(SmoothStart(t, smoothnessStart), SmoothStop(t, smoothnessStop), t);
        }


        public static float Arch2(float t)
        {

            return Flip(t) * t;
        }

        public static float SmoothStopArch3(float t)
        {
            return Scale(Arch2(t), Flip(t));
        }


        public static float SmoothStartArch3(float t)
        {
            return Scale(Arch2(t), t);
        }

        public static float SmoothStepArch4(float t)
        {
            return Scale(SmoothStartArch3(t), Flip(t));
        }

        public static float BellCurve(float t, float smoothness = 1)
        {
            return SmoothStart(t, smoothness) * SmoothStop(t, smoothness);
        }

        public static float NormalizedBezier(float B, float C, float t)
        {
            float s = 1f - t;
            float t2 = t * t;
            float s2 = s * s;
            float t3 = t2 * t;
            return (3f * B * s2 * t) + (3f * C * s * t2) + t3;
        }

        public static Vector3[] Chaikin(Vector3[] pts, int smoothness)
        {
            if (smoothness < 1) return pts;

            Vector3[] ret = pts;
            for (int i = 0; i < smoothness; i++)
            {
                ret = Chaikin(ret);
            }
            return ret;
        }

        public static Vector3[] Chaikin(Vector3[] pts)
        {
            Vector3[] newPts = new Vector3[(pts.Length - 2) * 2 + 2];
            newPts[0] = pts[0];
            newPts[newPts.Length - 1] = pts[pts.Length - 1];

            int j = 1;
            for (int i = 0; i < pts.Length - 2; i++)
            {
                newPts[j] = pts[i] + (pts[i + 1] - pts[i]) * 0.75f;
                newPts[j + 1] = pts[i + 1] + (pts[i + 2] - pts[i + 1]) * 0.25f;
                j += 2;
            }

            return newPts;
        }

        public static List<Vector3> SmoothLine(List<Vector3> cornerPoints, float smoothness)
        {
            if (smoothness < 1) smoothness = 1;
            List<Vector3> smoothedPoints;
            List<Vector3> ret;
            int cornerCount = cornerPoints.Count;
            int curvedLength = (int)(Math.Round((cornerCount * smoothness))) - 1;
            ret = new List<Vector3>(curvedLength);
            float t = 0;
            for (int pointOnCurve = 0; pointOnCurve < curvedLength + 1; pointOnCurve++)
            {
                t = InverseLerp(0, curvedLength, pointOnCurve);
                smoothedPoints = new List<Vector3>(cornerPoints);
                for (int j = cornerCount - 1; j > 0; j--)
                {
                    for (int i = 0; i < j; i++)
                    {
                        smoothedPoints[i] = (1 - t) * smoothedPoints[i] + t * smoothedPoints[i + 1];
                    }
                }
                ret.Add(smoothedPoints[0]);
            }
            return ret;
        }
    }
}