using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Aori.DSA.FloatRange
{
    [CustomPropertyDrawer(typeof(FloatRange))]
    public class FloatRangeDrawer : PropertyDrawer
    {
        private const float SPACING = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3 + 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var minProp = property.FindPropertyRelative("m_min");
            var maxProp = property.FindPropertyRelative("m_max");

            var min = minProp.floatValue;
            var max = maxProp.floatValue;

            var minLimit = 0f;
            var maxLimit = 1f;
            var floatRangeAttribute = fieldInfo.GetCustomAttribute<FloatRangeAttribute>();

            if (floatRangeAttribute != null)
            {
                minLimit = floatRangeAttribute.MinLimit;
                maxLimit = floatRangeAttribute.MaxLimit;
            }

            var sliderRect
                = new Rect(
                    position.x,
                    position.y,
                    position.width,
                    EditorGUIUtility.singleLineHeight
                );

            var fieldRect
                = new Rect(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight + 2,
                    position.width,
                    EditorGUIUtility.singleLineHeight
                );

            EditorGUI.BeginProperty(position, label, property);

            // Slider
            EditorGUI.MinMaxSlider(sliderRect, label, ref min, ref max, minLimit, maxLimit);

            var fieldWidth = (fieldRect.width - SPACING) * 0.5f;

            var minRect
                = new Rect(fieldRect.x, fieldRect.y, fieldWidth, fieldRect.height);
            var maxRect
                = new Rect(
                    fieldRect.x,
                    fieldRect.y + EditorGUIUtility.singleLineHeight + SPACING,
                    fieldWidth,
                    fieldRect.height
                );

            min = EditorGUI.FloatField(minRect, "Min", min);
            max = EditorGUI.FloatField(maxRect, "Max", max);

            // Clamp logic
            min = Mathf.Clamp(min, minLimit, maxLimit);
            max = Mathf.Clamp(max, minLimit, maxLimit);

            if (min > max)
            {
                min = max;
            }

            if (max < min)
            {
                max = min;
            }

            minProp.floatValue = min;
            maxProp.floatValue = max;

            EditorGUI.EndProperty();
        }
    }
}