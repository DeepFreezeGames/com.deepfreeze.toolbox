using Toolbox.Runtime.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SliderAttribute))]
    public class SliderDrawer : PropertyDrawer
    {
        private const string _invalidTypeWarning = "Invalid type for MinMaxSlider on field {0}: MinMaxSlider can only be applied to a float or int fields";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var slider = attribute as SliderAttribute;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                {
                    var value = EditorGUI.Slider(position, label, property.floatValue, slider.MinimumValue, slider.MaximumValue);
                    value = SnapDrawer.Snap(value, slider.SnapValue);
                    property.floatValue = Mathf.Clamp(value, slider.MinimumValue, slider.MaximumValue);
                    break;
                }

                case SerializedPropertyType.Integer:
                {
                    var value = EditorGUI.IntSlider(position, label, property.intValue, Mathf.RoundToInt(slider.MinimumValue), Mathf.RoundToInt(slider.MaximumValue));
                    value = Mathf.RoundToInt(SnapDrawer.Snap(value, slider.SnapValue));
                    property.intValue = Mathf.Clamp(value, Mathf.RoundToInt(slider.MinimumValue), Mathf.RoundToInt(slider.MaximumValue));
                    break;
                }

                default:
                    Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
                    EditorGUI.PropertyField(position, property, label);
                    break;
            }
        }
    }
}
