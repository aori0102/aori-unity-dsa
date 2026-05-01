using System;
using UnityEngine;

namespace Aori.DSA.Unity
{
    public sealed class ThresholdRangeAttribute : PropertyAttribute
    {
        public readonly float Min;
        public readonly float Max;

        public ThresholdRangeAttribute(float min, float max)
        {
            if (min > max)
            {
                throw new ArgumentException($"{nameof(min)} must be less than {nameof(max)}");
            }

            Min = min;
            Max = max;
        }
    }
}