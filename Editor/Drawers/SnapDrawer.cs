using System;
using Toolbox.Runtime.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SnapAttribute))]
    public class SnapDrawer : PropertyDrawer
    {
        private const string _invalidTypeWarning = "Invalid type for MinMaxSlider on field {0}: MinMaxSlider can only be applied to a float or int fields";

        public static float Snap(float value, float snap)
        {
            return snap > 0.0f ? Mathf.Round(value / snap) * snap : value;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!(attribute is SnapAttribute snap))
            {
                throw new NullReferenceException("Snapped property does not inherit from float or int");
            }

            EditorGUI.PropertyField(position, property, label);

            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    property.floatValue = Snap(property.floatValue, snap.SnapValue);
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = Mathf.RoundToInt(Snap(property.intValue, snap.SnapValue));
                    break;
                
                default:
                    Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
                    break;
            }
        }
    }
}
