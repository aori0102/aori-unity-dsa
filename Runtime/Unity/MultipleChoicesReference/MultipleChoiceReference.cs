using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aori.DSA.Unity
{
    /// <summary>
    /// Holds a configurable list of typed object references for derived data classes. This is
    /// useful when a field can be assigned with many types related in concept and does not have
    /// a single "best" type.
    /// </summary>
    /// <remarks>
    /// Slot definitions are discovered from class attributes, then synchronized into
    /// serialized entries so Unity can persist assigned references.
    /// </remarks>
    [Serializable]
    public class MultipleChoiceReference : ISerializationCallbackReceiver, IEquatable<MultipleChoiceReference>
    {
        [SerializeField]
        [HideInInspector]
        [Tooltip("Whether multiple references can be assigned to this field. When unchecked, other" +
                 "reference fields will be hidden when a field is assigned.")]
        private bool m_allowMultiple = true;

        [SerializeField]
        private List<ReferenceEntry> m_entryList = new();

        [NonSerialized]
        private Dictionary<Type, UnityEngine.Object> m_referenceMap = new();

        /// <summary>
        /// Ensures runtime and serialized collections are always available,
        /// including during Unity serialization callbacks.
        /// </summary>
        private void EnsureCollectionsInitialized()
        {
            m_entryList ??= new List<ReferenceEntry>();
            m_referenceMap ??= new Dictionary<Type, UnityEngine.Object>();
        }

        /// <summary>
        /// Serialized entries shown in the inspector.
        /// </summary>
        public IReadOnlyList<ReferenceEntry> EntryList => m_entryList;

        /// <summary>
        /// Rebuilt map from declared entry type to assigned object.
        /// </summary>
        public IReadOnlyDictionary<Type, UnityEngine.Object> ReferenceMap
        {
            get
            {
                EnsureInitializedFromAttributes();
                RebuildReferenceMap();
                return m_referenceMap;
            }
        }

        /// <summary>
        /// Finds a reference assigned to the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type used to find the reference.</param>
        /// <param name="reference">The first reference with the exact type, or <c>null</c>
        /// if none is found.</param>
        /// <returns><c>true</c> if a reference is found otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="type"/>
        /// is <c>null</c>.</exception>
        public bool TryGet(Type type, out UnityEngine.Object reference)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            EnsureInitializedFromAttributes();
            RebuildReferenceMap();
            return m_referenceMap.TryGetValue(type, out reference);
        }

        /// <summary>
        /// Generic helper for <see cref="TryGet(Type, out UnityEngine.Object)"/>.
        /// </summary>
        /// <param name="reference">The first reference of the exact type, or <c>null</c>
        /// if none is found.</param>
        /// <typeparam name="T">The type used to find the reference.</typeparam>
        /// <returns><c>true</c> if a reference is found, otherwise <c>false</c>.</returns>
        public bool TryGet<T>(out T reference) where T : UnityEngine.Object
        {
            if (TryGet(typeof(T), out var obj))
            {
                reference = obj as T;
                return reference != null;
            }

            reference = null;
            return false;
        }

        /// <summary>
        /// Inserts or updates a reference for the exact configured <paramref name="type"/>.
        /// If this instance has no configured slots, a new slot is created.
        /// </summary>
        /// <param name="type">Exact slot type to assign.</param>
        /// <param name="reference">Reference to assign. Pass <c>null</c> to clear the slot.</param>
        /// <returns>
        /// <c>true</c> when assignment succeeds, or <c>false</c> when the slot is not configured.
        /// </returns>
        public bool TryInsertReference(Type type, UnityEngine.Object reference)
        {
            ValidateSupportedReferenceType(type, nameof(type));

            if (reference != null && !type.IsInstanceOfType(reference))
            {
                throw new ArgumentException(
                    $"Assigned object of type {reference.GetType().Name} does not match slot type {type.Name}.",
                    nameof(reference)
                );
            }

            EnsureInitializedFromAttributes();

            var typeName = GetStableTypeName(type);
            var entryIndex = FindEntryIndexByType(type);

            if (entryIndex < 0)
            {
                // Attribute-configured schemas are fixed; unknown runtime types are rejected.
                if (HasConfiguredFields(GetType()))
                {
                    return false;
                }

                m_entryList.Add(new ReferenceEntry
                {
                    Label = type.Name,
                    TypeName = typeName,
                    Reference = null
                });

                entryIndex = m_entryList.Count - 1;
            }

            m_entryList[entryIndex].Reference = reference;

            if (!m_allowMultiple && reference != null)
            {
                ClearReferencesExcept(entryIndex);
            }

            EnsureUniqueEntriesByType();
            RebuildReferenceMap();
            return true;
        }

        /// <summary>
        /// Generic helper for <see cref="TryInsertReference(Type,UnityEngine.Object)"/>.
        /// </summary>
        public bool TryInsertReference<T>(T reference) where T : UnityEngine.Object
        {
            return TryInsertReference(typeof(T), reference);
        }

        /// <summary>
        /// Returns a copy of the current type-reference map.
        /// </summary>
        public Dictionary<Type, UnityEngine.Object> ToTypeMap()
        {
            EnsureInitializedFromAttributes();
            RebuildReferenceMap();
            return new Dictionary<Type, UnityEngine.Object>(m_referenceMap);
        }

        /// <summary>
        /// Returns all assigned non-null references as a new array.
        /// </summary>
        public UnityEngine.Object[] GetAll()
        {
            EnsureInitializedFromAttributes();

            var references = new List<UnityEngine.Object>();
            for (var i = 0; i < m_entryList.Count; i++)
            {
                var reference = m_entryList[i]?.Reference;
                if (reference != null)
                {
                    references.Add(reference);
                }
            }

            return references.ToArray();
        }

        /// <summary>
        /// Returns all assigned non-null references compatible with <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Reference type to filter by.</typeparam>
        /// <exception cref="ArgumentException">Throws when <typeparamref name="T"/>
        /// is not a supported Unity reference type.</exception>
        public T[] GetAll<T>() where T : UnityEngine.Object
        {
            ValidateSupportedReferenceType(typeof(T), nameof(T));
            EnsureInitializedFromAttributes();

            var references = new List<T>();
            for (var i = 0; i < m_entryList.Count; i++)
            {
                var reference = m_entryList[i]?.Reference;
                if (reference is T typedReference)
                {
                    references.Add(typedReference);
                }
            }

            return references.ToArray();
        }

        /// <summary>
        /// Validates that a type can be used as a reference slot type.
        /// Generic types are rejected to keep slot typing explicit and predictable.
        /// </summary>
        public static void ValidateSupportedReferenceType(Type referenceType, string paramName = "referenceType")
        {
            if (referenceType == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (referenceType.IsGenericTypeDefinition || referenceType.ContainsGenericParameters)
            {
                throw new ArgumentException(
                    $"{referenceType.Name} cannot be an open generic type.",
                    paramName
                );
            }

            if (!typeof(UnityEngine.Object).IsAssignableFrom(referenceType))
            {
                throw new ArgumentException(
                    $"{referenceType.Name} must derive from {nameof(UnityEngine.Object)}.",
                    paramName
                );
            }
        }

        internal static string GetTypeSignatureKey(Type referenceType)
        {
            if (referenceType == null)
            {
                return string.Empty;
            }

            if (!referenceType.IsGenericType)
            {
                return GetStableTypeName(referenceType);
            }

            var genericDefinitionName = GetStableTypeName(referenceType.GetGenericTypeDefinition());
            var genericArguments = referenceType.GetGenericArguments();
            var argumentKeys = new string[genericArguments.Length];

            for (var i = 0; i < genericArguments.Length; i++)
            {
                argumentKeys[i] = GetTypeSignatureKey(genericArguments[i]);
            }

            return $"{genericDefinitionName}<{string.Join(",", argumentKeys)}>";
        }

        public virtual void OnBeforeSerialize()
        {
            EnsureCollectionsInitialized();
            EnsureInitializedFromAttributes();
            if (!m_allowMultiple)
            {
                KeepFirstAssignedReference();
            }
        }

        public virtual void OnAfterDeserialize()
        {
            EnsureCollectionsInitialized();
            EnsureInitializedFromAttributes();
            if (!m_allowMultiple)
            {
                KeepFirstAssignedReference();
            }

            RebuildReferenceMap();
        }

        /// <summary>
        /// Synchronizes serialized entries from class-level field attributes.
        /// Existing assignments are preserved by matching on the configured type name.
        /// </summary>
        private void EnsureInitializedFromAttributes()
        {
            EnsureCollectionsInitialized();

            var referenceType = GetType();
            ApplyConfiguredMode(referenceType);

            var configuredFields = GetConfiguredFields(referenceType);

            if (configuredFields.Count == 0)
            {
                EnsureUniqueEntriesByType();
                return;
            }

            var previousByType = new Dictionary<string, UnityEngine.Object>();
            for (var i = 0; i < m_entryList.Count; i++)
            {
                var entry = m_entryList[i];
                if (entry == null || string.IsNullOrWhiteSpace(entry.TypeName))
                {
                    continue;
                }

                // Preserve existing references when the slot schema is rebuilt.
                previousByType[entry.TypeName] = entry.Reference;
            }

            m_entryList.Clear();

            for (var i = 0; i < configuredFields.Count; i++)
            {
                var configuredField = configuredFields[i];
                var typeName = GetStableTypeName(configuredField.ReferenceType);

                UnityEngine.Object previousReference = null;
                if (previousByType.ContainsKey(typeName))
                {
                    previousReference = previousByType[typeName];
                }

                m_entryList.Add(new ReferenceEntry
                {
                    Label = configuredField.Label,
                    TypeName = typeName,
                    Reference = previousReference
                });
            }

            EnsureUniqueEntriesByType();
        }

        private static List<MultipleChoiceConfiguredField> GetConfiguredFields(Type referenceType)
        {
            var configuredFields = new List<MultipleChoiceConfiguredField>();
            var attributes = (MultipleChoiceReferenceFieldAttribute[])referenceType.GetCustomAttributes(
                typeof(MultipleChoiceReferenceFieldAttribute),
                true
            );
            var uniqueTypes = new HashSet<string>(StringComparer.Ordinal);

            for (var i = 0; i < attributes.Length; i++)
            {
                var attribute = attributes[i];
                if (attribute == null)
                {
                    continue;
                }

                ValidateSupportedReferenceType(attribute.ReferenceType);

                var typeKey = GetTypeSignatureKey(attribute.ReferenceType);
                if (!uniqueTypes.Add(typeKey))
                {
                    throw new InvalidOperationException(
                        $"Duplicate reference slot type '{attribute.ReferenceType.Name}' is not allowed on {referenceType.Name}. " +
                        "Use a Unity object that contains a List<T> when a concept needs multiple values."
                    );
                }

                configuredFields.Add(new MultipleChoiceConfiguredField(attribute.ReferenceType, attribute.Label));
            }

            return configuredFields;
        }

        private static bool HasConfiguredFields(Type referenceType)
        {
            return referenceType.GetCustomAttributes(typeof(MultipleChoiceReferenceFieldAttribute), true).Length > 0;
        }

        private static string GetStableTypeName(Type referenceType)
        {
            var typeName = referenceType.AssemblyQualifiedName ?? referenceType.FullName;
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new InvalidOperationException(
                    $"Could not resolve a stable type name for {referenceType.Name}."
                );
            }

            return typeName;
        }

        private int FindEntryIndexByType(Type referenceType)
        {
            var targetTypeKey = GetTypeSignatureKey(referenceType);

            for (var i = 0; i < m_entryList.Count; i++)
            {
                var entry = m_entryList[i];
                if (entry == null)
                {
                    continue;
                }

                var entryType = entry.GetReferenceType();
                if (entryType != typeof(UnityEngine.Object))
                {
                    var entryTypeKey = GetTypeSignatureKey(entryType);
                    if (string.Equals(entryTypeKey, targetTypeKey, StringComparison.Ordinal))
                    {
                        return i;
                    }
                }

                if (string.Equals(entry.TypeName, GetStableTypeName(referenceType), StringComparison.Ordinal))
                {
                    return i;
                }
            }

            return -1;
        }

        private void EnsureUniqueEntriesByType()
        {
            var firstIndexByTypeName = new Dictionary<string, int>(StringComparer.Ordinal);
            var duplicateIndexes = new List<int>();

            for (var i = 0; i < m_entryList.Count; i++)
            {
                var entry = m_entryList[i];
                if (entry == null || string.IsNullOrWhiteSpace(entry.TypeName))
                {
                    continue;
                }

                var keyType = entry.GetReferenceType();
                var typeKey = keyType == typeof(UnityEngine.Object)
                    ? entry.TypeName
                    : GetTypeSignatureKey(keyType);

                if (!firstIndexByTypeName.TryGetValue(typeKey, out var firstIndex))
                {
                    firstIndexByTypeName[typeKey] = i;
                    continue;
                }

                var firstEntry = m_entryList[firstIndex];
                if (firstEntry != null && firstEntry.Reference == null && entry.Reference != null)
                {
                    firstEntry.Reference = entry.Reference;
                }

                duplicateIndexes.Add(i);
            }

            for (var i = duplicateIndexes.Count - 1; i >= 0; i--)
            {
                m_entryList.RemoveAt(duplicateIndexes[i]);
            }
        }

        private void ClearReferencesExcept(int keptIndex)
        {
            for (var i = 0; i < m_entryList.Count; i++)
            {
                if (i == keptIndex || m_entryList[i] == null)
                {
                    continue;
                }

                m_entryList[i].Reference = null;
            }
        }

        private void ApplyConfiguredMode(Type referenceType)
        {
            var allowMultiple = GetConfiguredAllowMultiple(referenceType);
            m_allowMultiple = allowMultiple;

            if (!m_allowMultiple)
            {
                KeepFirstAssignedReference();
            }
        }

        /// <summary>
        /// Multiple mode is enabled when the owning type has <see cref="AllowMultipleAttribute"/>.
        /// </summary>
        private static bool GetConfiguredAllowMultiple(Type referenceType)
        {
            var modeAttribute = Attribute.GetCustomAttribute(
                referenceType,
                typeof(AllowMultipleAttribute),
                true
            ) as AllowMultipleAttribute;

            return modeAttribute != null;
        }

        /// <summary>
        /// Rebuilds fast runtime lookup from the serialized entries.
        /// </summary>
        private void RebuildReferenceMap()
        {
            EnsureCollectionsInitialized();
            m_referenceMap.Clear();

            for (var i = 0; i < m_entryList.Count; i++)
            {
                var entry = m_entryList[i];
                if (entry == null || entry.Reference == null)
                {
                    continue;
                }

                var keyType = entry.GetReferenceType();
                if (keyType == typeof(UnityEngine.Object))
                {
                    continue;
                }

                m_referenceMap[keyType] = entry.Reference;

                if (!m_allowMultiple)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Enforces single-choice behavior by keeping only the first assigned entry.
        /// </summary>
        private void KeepFirstAssignedReference()
        {
            EnsureCollectionsInitialized();

            // Get the index of the first entry whose reference is not null.
            var firstAssignedIndex = -1;

            for (var i = 0; i < m_entryList.Count; i++)
            {
                if (m_entryList[i]?.Reference == null)
                {
                    continue;
                }

                firstAssignedIndex = i;
                break;
            }

            if (firstAssignedIndex < 0)
            {
                // No entries are assigned.
                return;
            }

            // Nullify all entries except for the first assigned one.
            ClearReferencesExcept(firstAssignedIndex);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MultipleChoiceReference);
        }

        /// <summary>
        /// Compares two instances by unordered entry content.
        /// Two objects are equal when they contain the same (type, reference) pairs,
        /// independent of entry order.
        /// </summary>
        public bool Equals(MultipleChoiceReference other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            EnsureInitializedFromAttributes();
            other.EnsureInitializedFromAttributes();

            var thisBag = BuildEntryBag(m_entryList);
            var otherBag = BuildEntryBag(other.m_entryList);

            if (thisBag.Count != otherBag.Count)
            {
                return false;
            }

            foreach (var pair in thisBag)
            {
                if (!otherBag.TryGetValue(pair.Key, out var otherCount) || otherCount != pair.Value)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Generates an order-independent hash based on the same bag used in equality.
        /// </summary>
        public override int GetHashCode()
        {
            EnsureInitializedFromAttributes();

            var bag = BuildEntryBagSnapshot();
            var hash = 17;

            foreach (var pair in bag)
            {
                unchecked
                {
                    hash += (pair.Key.GetHashCode() * 397) ^ pair.Value;
                }
            }

            return hash;
        }

        private Dictionary<EntryKey, int> BuildEntryBagSnapshot()
        {
            return BuildEntryBag(new List<ReferenceEntry>(m_entryList));
        }

        private static Dictionary<EntryKey, int> BuildEntryBag(List<ReferenceEntry> entries)
        {
            var bag = new Dictionary<EntryKey, int>();

            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (entry == null)
                {
                    continue;
                }

                // Count duplicates so equality behaves like a multiset, not just a set.
                var key = new EntryKey(entry.TypeName, entry.Reference);
                if (bag.TryGetValue(key, out var count))
                {
                    bag[key] = count + 1;
                }
                else
                {
                    bag[key] = 1;
                }
            }

            return bag;
        }

        public static bool operator ==(MultipleChoiceReference left, MultipleChoiceReference right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MultipleChoiceReference left, MultipleChoiceReference right)
        {
            return !Equals(left, right);
        }
    }

    internal readonly struct MultipleChoiceConfiguredField
    {
        public MultipleChoiceConfiguredField(Type referenceType, string label)
        {
            ReferenceType = referenceType;
            Label = string.IsNullOrWhiteSpace(label) ? referenceType.Name : label;
        }

        public Type ReferenceType { get; }

        public string Label { get; }
    }
}
