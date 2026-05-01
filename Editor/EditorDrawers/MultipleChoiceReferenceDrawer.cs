using System;
using System.Collections.Generic;
using Aori.DSA.Unity;
using UnityEditor;
using UnityEngine;

namespace Aori.DSA.EditorDrawers
{
    [CustomPropertyDrawer(typeof(MultipleChoiceReference), true)]
    public sealed class MultipleChoiceReferenceDrawer : PropertyDrawer
    {
        private const float SPACING = 4f;
        private const float LABEL_WIDTH_RATIO = 0.35f;
        private const float CLEAR_BUTTON_WIDTH = 48f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var lineCount = 1;
            if (!property.isExpanded)
            {
                return lineCount * EditorGUIUtility.singleLineHeight;
            }

            var entries = property.FindPropertyRelative("m_entryList");
            var allowMultiple = IsAllowMultiple(property, entries);

            lineCount += GetVisibleEntryCount(entries, allowMultiple);

            return lineCount * EditorGUIUtility.singleLineHeight + (lineCount - 1) * SPACING;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var entriesProp = property.FindPropertyRelative("m_entryList");
            var allowMultiple = IsAllowMultiple(property, entriesProp);

            EditorGUI.BeginProperty(position, label, property);

            var y = position.y;
            var rowHeight = EditorGUIUtility.singleLineHeight;

            var headerRect = new Rect(position.x, y, position.width, rowHeight);
            property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, label, true);

            if (!property.isExpanded)
            {
                EditorGUI.EndProperty();
                return;
            }

            y += rowHeight + SPACING;

            EditorGUI.indentLevel++;

            if (!allowMultiple)
            {
                KeepFirstAssignedReference(entriesProp);
            }

            var assignedIndex = GetFirstAssignedIndex(entriesProp);

            for (var i = 0; i < entriesProp.arraySize; i++)
            {
                if (!allowMultiple && assignedIndex >= 0 && i != assignedIndex)
                {
                    continue;
                }

                var entryProp = entriesProp.GetArrayElementAtIndex(i);
                DrawEntryRow(new Rect(position.x, y, position.width, rowHeight), entryProp, i);
                y += rowHeight + SPACING;
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        private static int GetVisibleEntryCount(SerializedProperty entries, bool allowMultiple)
        {
            if (allowMultiple)
            {
                return entries.arraySize;
            }

            return GetFirstAssignedIndex(entries) >= 0 ? 1 : entries.arraySize;
        }

        private static int GetFirstAssignedIndex(SerializedProperty entries)
        {
            for (var i = 0; i < entries.arraySize; i++)
            {
                var entryProp = entries.GetArrayElementAtIndex(i);
                var referenceProp = entryProp.FindPropertyRelative("m_reference");
                if (referenceProp.objectReferenceValue != null)
                {
                    return i;
                }
            }

            return -1;
        }

        private static void KeepFirstAssignedReference(SerializedProperty entries)
        {
            var firstAssigned = GetFirstAssignedIndex(entries);
            if (firstAssigned < 0)
            {
                return;
            }

            for (var i = 0; i < entries.arraySize; i++)
            {
                if (i == firstAssigned)
                {
                    continue;
                }

                var entryProp = entries.GetArrayElementAtIndex(i);
                entryProp.FindPropertyRelative("m_reference").objectReferenceValue = null;
            }
        }

        private static void DrawEntryRow(Rect rect, SerializedProperty entryProp, int index)
        {
            var labelProp = entryProp.FindPropertyRelative("m_label");
            var typeNameProp = entryProp.FindPropertyRelative("m_typeName");
            var referenceProp = entryProp.FindPropertyRelative("m_reference");

            var elementRect = EditorGUI.IndentedRect(rect);
            var labelWidth = elementRect.width * LABEL_WIDTH_RATIO;
            var clearRect = new Rect(
                elementRect.xMax - CLEAR_BUTTON_WIDTH,
                elementRect.y,
                CLEAR_BUTTON_WIDTH,
                elementRect.height
            );

            var fieldStartX = elementRect.x + labelWidth + SPACING;
            var fieldWidth = clearRect.x - fieldStartX - SPACING;
            if (fieldWidth < 0f)
            {
                fieldWidth = 0f;
            }

            var labelRect = new Rect(elementRect.x, elementRect.y, labelWidth, elementRect.height);
            var fieldRect = new Rect(fieldStartX, elementRect.y, fieldWidth, elementRect.height);

            var labelText = string.IsNullOrWhiteSpace(labelProp.stringValue)
                ? $"Reference {index + 1}"
                : labelProp.stringValue;

            EditorGUI.LabelField(labelRect, labelText);

            var referenceType = ResolveType(typeNameProp.stringValue);
            var currentValue = referenceProp.objectReferenceValue;
            referenceProp.objectReferenceValue =
                EditorGUI.ObjectField(fieldRect, GUIContent.none, currentValue, referenceType, true);

            if (GUI.Button(clearRect, "Clear"))
            {
                referenceProp.objectReferenceValue = null;
            }
        }

        private static Type ResolveType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return typeof(UnityEngine.Object);
            }

            return Type.GetType(typeName, false) ?? typeof(UnityEngine.Object);
        }

        private bool IsAllowMultiple(SerializedProperty property, SerializedProperty entries)
        {
            var allowMultipleProp = property.FindPropertyRelative("m_allowMultiple");
            EnsureInitializedFromAttributes(entries, allowMultipleProp, fieldInfo.FieldType);
            return allowMultipleProp == null || allowMultipleProp.boolValue;
        }

        private static void EnsureInitializedFromAttributes(
            SerializedProperty entries,
            SerializedProperty allowMultipleProp,
            Type referenceType
        )
        {
            if (entries == null || referenceType == null)
            {
                return;
            }

            if (allowMultipleProp != null)
            {
                var allowMultiple = GetConfiguredAllowMultiple(referenceType);
                allowMultipleProp.boolValue = allowMultiple;
            }

            var attributes = (MultipleChoiceReferenceFieldAttribute[])referenceType.GetCustomAttributes(
                typeof(MultipleChoiceReferenceFieldAttribute),
                true
            );

            var uniqueAttributes = new List<MultipleChoiceReferenceFieldAttribute>();
            var uniqueTypes = new HashSet<string>(StringComparer.Ordinal);

            for (var i = 0; i < attributes.Length; i++)
            {
                var attribute = attributes[i];
                if (attribute == null)
                {
                    continue;
                }

                MultipleChoiceReference.ValidateSupportedReferenceType(attribute.ReferenceType);

                var typeKey = MultipleChoiceReference.GetTypeSignatureKey(attribute.ReferenceType);
                if (!uniqueTypes.Add(typeKey))
                {
                    throw new InvalidOperationException(
                        $"Duplicate reference slot type '{attribute.ReferenceType.Name}' is not allowed on {referenceType.Name}."
                    );
                }

                uniqueAttributes.Add(attribute);
            }

            if (uniqueAttributes.Count == 0)
            {
                return;
            }

            var previousByType = new Dictionary<string, UnityEngine.Object>(StringComparer.Ordinal);
            for (var i = 0; i < entries.arraySize; i++)
            {
                var entry = entries.GetArrayElementAtIndex(i);
                var typeName = entry.FindPropertyRelative("m_typeName").stringValue;
                var reference = entry.FindPropertyRelative("m_reference").objectReferenceValue;
                if (string.IsNullOrWhiteSpace(typeName))
                {
                    continue;
                }

                var resolvedType = ResolveType(typeName);
                var typeKey = resolvedType == typeof(UnityEngine.Object)
                    ? typeName
                    : MultipleChoiceReference.GetTypeSignatureKey(resolvedType);
                previousByType[typeKey] = reference;
            }

            entries.arraySize = uniqueAttributes.Count;
            for (var i = 0; i < uniqueAttributes.Count; i++)
            {
                var attribute = uniqueAttributes[i];
                var typeName = attribute.ReferenceType.AssemblyQualifiedName
                               ?? attribute.ReferenceType.FullName;
                if (string.IsNullOrWhiteSpace(typeName))
                {
                    continue;
                }

                var typeKey = MultipleChoiceReference.GetTypeSignatureKey(attribute.ReferenceType);

                var entry = entries.GetArrayElementAtIndex(i);
                entry.FindPropertyRelative("m_label").stringValue = attribute.Label;
                entry.FindPropertyRelative("m_typeName").stringValue = typeName;

                UnityEngine.Object previousReference = null;
                if (previousByType.ContainsKey(typeKey))
                {
                    previousReference = previousByType[typeKey];
                }

                entry.FindPropertyRelative("m_reference").objectReferenceValue = previousReference;
            }
        }

        private static bool GetConfiguredAllowMultiple(Type referenceType)
        {
            var allowMultipleAttribute = Attribute.GetCustomAttribute(
                referenceType,
                typeof(AllowMultipleAttribute),
                true
            ) as AllowMultipleAttribute;

            return allowMultipleAttribute != null;
        }
    }
}