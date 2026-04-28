using System;
using UnityEngine;

namespace Aori.DSA.FloatRange
{
    [Serializable]
    public sealed class FloatRange
    {
        [SerializeField]
        private float m_min;

        [SerializeField]
        private float m_max;

        public float Min
        {
            get => m_min;
            set => m_min = value;
        }

        public float Max
        {
            get => m_max;
            set => m_max = value;
        }
        
        public FloatRange(float min, float max)
        {
            if (min > max)
            {
                throw new ArgumentException($"{nameof(min)} must be less than {nameof(max)}");
            }

            m_min = min;
            m_max = max;
        }
    }
}