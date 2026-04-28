using System;

namespace Aori.DSA.MultipleChoicesReference
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class MultipleChoiceReferenceFieldAttribute : Attribute
    {
        public MultipleChoiceReferenceFieldAttribute(Type referenceType, string label = null)
        {
            MultipleChoiceReference.ValidateSupportedReferenceType(referenceType);

            ReferenceType = referenceType;
            Label = string.IsNullOrWhiteSpace(label) ? referenceType.Name : label;
        }

        public Type ReferenceType { get; }

        public string Label { get; }
    }
}
