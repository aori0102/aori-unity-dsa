using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Aori.DSA.Threshold
{
    [CustomPropertyDrawer(typeof(Threshold<>), true)]
    public class ThresholdDrawer : PropertyDrawer
    {
        private const float BUTTON_WIDTH = 22f;
        private const float UTILITY_BUTTON_WIDTH = 60f;
        private const float BUTTON_OFFSET = 10f;
        private const float SPACING = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var thresholdList = property.FindPropertyRelative("m_thresholdList");

            var count = thresholdList.arraySize;
            var lineCount = count + 2; // label + items + add button

            return lineCount * EditorGUIUtility.singleLineHeight +
                   (lineCount - 1) * SPACING;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueList = property.FindPropertyRelative("m_valueList");
            var thresholdList = property.FindPropertyRelative("m_thresholdList");

            // Get min max of threshold
            var thresholdRangeAttribute
                = fieldInfo.GetCustomAttribute<ThresholdRangeAttribute>();

            EditorGUI.BeginProperty(position, label, property);

            var y = position.y;

            // Header
            EditorGUI.LabelField(
                new Rect(
                    position.x,
                    y,
                    position.width,
                    EditorGUIUtility.singleLineHeight
                ),
                label
            );

            y += EditorGUIUtility.singleLineHeight + SPACING;

            // Draw rows
            for (var i = 0; i < thresholdList.arraySize; i++)
            {
                DrawRow(position, ref y, thresholdList, valueList, thresholdRangeAttribute, i);
            }

            // Add button and utility buttons
            DrawAddButton(position, y, thresholdList, valueList);
            DrawUtilityButtons(position, y, thresholdList, valueList);

            EditorGUI.EndProperty();
        }

        private static void DrawRow(
            Rect position,
            ref float y,
            SerializedProperty thresholdList,
            SerializedProperty valueList,
            ThresholdRangeAttribute rangeAttribute,
            int index
        )
        {
            var rowHeight = EditorGUIUtility.singleLineHeight;
            var thresholdMin = rangeAttribute?.Min ?? float.MinValue;
            var thresholdMax = rangeAttribute?.Max ?? float.MaxValue;

            var thresholdProp = thresholdList.GetArrayElementAtIndex(index);
            var valueProp = valueList.GetArrayElementAtIndex(index);

            var thresholdWidth = position.width * 0.35f;
            var valueWidth = position.width - thresholdWidth - BUTTON_WIDTH - SPACING * 2;

            var thresholdRect
                = new Rect(position.x, y, thresholdWidth, rowHeight);

            var valueRect
                = new Rect(
                    position.x + thresholdWidth + SPACING,
                    y,
                    valueWidth,
                    rowHeight
                );

            var removeRect
                = new Rect(
                    position.x + thresholdWidth + valueWidth + SPACING * 2,
                    y,
                    BUTTON_WIDTH,
                    rowHeight
                );

            thresholdProp.floatValue
                = Mathf.Clamp(
                    EditorGUI.FloatField(thresholdRect, thresholdProp.floatValue),
                    thresholdMin,
                    thresholdMax
                );

            EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);

            if (GUI.Button(removeRect, "-"))
            {
                thresholdList.DeleteArrayElementAtIndex(index);
                valueList.DeleteArrayElementAtIndex(index);
                return;
            }

            y += rowHeight + SPACING;
        }

        private static void DrawUtilityButtons(
            Rect position,
            float y,
            SerializedProperty thresholdList,
            SerializedProperty valueList
        )
        {
            var x = position.x + BUTTON_WIDTH + BUTTON_OFFSET;
            var sortRect
                = new Rect(
                    x,
                    y,
                    UTILITY_BUTTON_WIDTH,
                    EditorGUIUtility.singleLineHeight
                );
            var clearRect
                = new Rect(
                    x + UTILITY_BUTTON_WIDTH + BUTTON_OFFSET,
                    y,
                    UTILITY_BUTTON_WIDTH,
                    EditorGUIUtility.singleLineHeight
                );

            if (GUI.Button(sortRect, "Sort"))
            {
                Sort(thresholdList, valueList);
            }

            if (GUI.Button(clearRect, "Clear"))
            {
                Clear(thresholdList, valueList);
            }
        }

        private static void DrawAddButton(
            Rect position,
            float y,
            SerializedProperty thresholdList,
            SerializedProperty valueList
        )
        {
            var addRect
                = new Rect(
                    position.x,
                    y,
                    BUTTON_WIDTH,
                    EditorGUIUtility.singleLineHeight
                );


            // Add button
            if (!GUI.Button(addRect, "+"))
            {
                return;
            }

            var index = thresholdList.arraySize;

            thresholdList.InsertArrayElementAtIndex(index);
            valueList.InsertArrayElementAtIndex(index);

            thresholdList
                .GetArrayElementAtIndex(index)
                .floatValue = 0f;
        }

        private static void Sort(
            SerializedProperty thresholdList,
            SerializedProperty valueList
        )
        {
            var count = thresholdList.arraySize;

            for (var i = 0; i < count - 1; i++)
            {
                var minIndex = i;
                var min
                    = thresholdList.GetArrayElementAtIndex(i).floatValue;
                for (var j = i + 1; j < count; j++)
                {
                    var value
                        = thresholdList.GetArrayElementAtIndex(j).floatValue;

                    if (value >= min)
                    {
                        continue;
                    }

                    min = value;
                    minIndex = j;
                }

                thresholdList.MoveArrayElement(minIndex, i);
                valueList.MoveArrayElement(minIndex, i);
            }
        }

        private static void Clear(
            SerializedProperty thresholdList,
            SerializedProperty valueList
        )
        {
            thresholdList.ClearArray();
            valueList.ClearArray();
        }
    }
}