using System;
using UnityEngine;

namespace Spiral.Core
{
    public static class MathTools
    {
        public static string MantissString(this float value, int mantiss)
        {
            return (mantiss < 0) ? value.ToString() : value.ToString($"F{mantiss}");
        }

        public static float Mantissed(this float value, int mantiss)
        {
            return Convert.ToSingle(System.Math.Round(value, mantiss));
        }

        public static float SafeRead(this float read, float safeValue)
        {
            if (float.IsNaN(read))      return safeValue;
            if (float.IsInfinity(read)) return safeValue;
            return read;
        }

        public static float ClampLow(this float value, float low)
        {
            return (value < low) ? low : value;
        }

        public static int ClampLow(this int value, int low)
        {
            return (value < low) ? low : value;
        }

        public static float ClampHigh(this float value, float high)
        {
            return (value > high) ? high : value;
        }

        public static int ClampHigh(this int value, int high)
        {
            return (value > high) ? high : value;
        }

        public static float Clamp0P(this float value)
        {
            return value.ClampLow(0);
        }

        public static int Clamp0P(this int value)
        {
            return value.ClampLow(0);
        }

        public static float Clamp0N(this float value)
        {
            return value.ClampHigh(0);
        }

        public static int Clamp0N(this int value)
        {
            return value.ClampHigh(0);
        }

        public static float Clamp(this float value, float low, float high)
        {
            return Mathf.Clamp(value, low, high);
        }

        public static int Clamp(this int value, int low, int hight)
        {
            return Mathf.Clamp(value, low, hight);
        }

        public static bool Between(this float value, float min, float max, bool include = true)
        {
            if (min > max)
            {
                float swap = max;
                max = min;
                min = swap;
            }

            if (include) return (value >= min) && (value <= max);
            else return (value > min) && (value < max);
        }

        public static bool Between(this int value, int min, int max, bool include = true)
        {
            if (include) return (value >= min) && (value <= max);
            else return (value > min) && (value < max);
        }

        public static int Round(this float value)
        {
            return Mathf.RoundToInt(value);
        }

        public static int Floor(this float value)
        {
            return Mathf.FloorToInt(value);
        }

        public static int Ceil(this float value)
        {
            return Mathf.CeilToInt(value);
        }

        public static float Abs(this float value)
        {
            return Mathf.Abs(value);
        }

        public static int Abs(this int value)
        {
            return Mathf.Abs(value);
        }
    }
}
