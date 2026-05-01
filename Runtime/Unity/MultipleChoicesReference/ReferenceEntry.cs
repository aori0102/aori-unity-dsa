using System;
using UnityEngine;

namespace Aori.DSA.Unity
{
    [Serializable]
    public sealed class ReferenceEntry
    {
        [SerializeField]
        private string m_label;

        [SerializeField]
        private string m_typeName;

        [SerializeField]
        private UnityEngine.Object m_reference;

        public string Label
        {
            get => m_label;
            set => m_label = value;
        }

        public string TypeName
        {
            get => m_typeName;
            set => m_typeName = value;
        }

        public UnityEngine.Object Reference
        {
            get => m_reference;
            set => m_reference = value;
        }

        public Type GetReferenceType()
        {
            return string.IsNullOrWhiteSpace(m_typeName)
                ? typeof(UnityEngine.Object)
                : Type.GetType(m_typeName, false) ?? typeof(UnityEngine.Object);
        }
    }
}