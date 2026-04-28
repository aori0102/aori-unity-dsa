using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aori.DSA.Threshold
{
    [Serializable]
    public sealed class Threshold<T>
    {
        [SerializeField]
        [HideInInspector]
        private List<T> m_valueList = new();

        [SerializeField]
        [HideInInspector]
        private List<float> m_thresholdList = new();
        
        public int Count => m_valueList.Count;

        public void AddThreshold(T value, float threshold)
        {
            m_valueList.Add(value);
            m_thresholdList.Add(threshold);
        }

        public bool RemoveThreshold(T value)
        {
            var index = m_valueList.IndexOf(value);
            if (index < 0)
            {
                return false;
            }

            m_valueList.RemoveAt(index);
            m_thresholdList.RemoveAt(index);

            return true;
        }

        public T GetValue(float input)
        {
            var result = default(T);
            var minDifference = float.MaxValue;
            for (var i = 0; i < Count; i++)
            {
                var threshold = m_thresholdList[i];
                var diff = threshold > input
                    ? float.MaxValue
                    : input - threshold;
                if (diff >= minDifference)
                {
                    continue;
                }

                minDifference = diff;
                result = m_valueList[i];
            }

            return result;
        }
        
        public List<T> GetValues()
        {
            return m_valueList.ToList();
        }
    }
}