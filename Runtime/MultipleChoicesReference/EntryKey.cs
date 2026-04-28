using System;

namespace Aori.DSA.MultipleChoicesReference
{
    public readonly struct EntryKey : IEquatable<EntryKey>
    {
        public EntryKey(string typeName, UnityEngine.Object reference)
        {
            TypeName = typeName ?? string.Empty;
            ReferenceId = reference == null ? 0 : reference.GetInstanceID();
        }

        private string TypeName { get; }

        private int ReferenceId { get; }

        public bool Equals(EntryKey other)
        {
            return string.Equals(TypeName, other.TypeName, StringComparison.Ordinal)
                   && ReferenceId == other.ReferenceId;
        }

        public override bool Equals(object obj)
        {
            return obj is EntryKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TypeName.GetHashCode() * 397) ^ ReferenceId;
            }
        }
    }
}