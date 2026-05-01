using UnityEngine;

namespace Aori.DSA.Unity
{
    public class FloatRangeAttribute : PropertyAttribute
    {
        public readonly float MinLimit;
        public readonly float MaxLimit;

        public FloatRangeAttribute(float min, float max)
        {
            MinLimit = min;
            MaxLimit = max;
        }
    }
}