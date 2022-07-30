using System;
using DeepFreeze.Packages.Toolbox.Runtime;
using UnityEditor;
using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(DurationAttribute))]
    public class DurationAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int value;
            DateTime dateTime;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    value = Convert.ToInt32(property.floatValue);
                    dateTime = value.ToDateTime();
                    property.floatValue = DurationField(position, dateTime, label);
                    break;
                case SerializedPropertyType.Integer:
                    value = property.intValue;
                    dateTime = value.ToDateTime();
                    property.intValue = DurationField(position, dateTime, label);
                    break;
                default:
                    EditorGUI.LabelField(position, "Not valid");
                    return;
            }
        }
        
        public static int DurationField(Rect rect, DateTime dateTime, GUIContent label)
        {
            var width = rect.width / 2f;
            var posX = rect.position.x;
            var labelRect = new Rect(rect.position, new Vector2(width, rect.height));
            posX += width / 1.3f;
            width /= 2.35f;
            width -= 30f;
            var hourRect = new Rect(new Vector2(posX, rect.position.y), new Vector2(width, rect.height));
            posX += width;
            var hourLabelRect = new Rect(new Vector2(posX + 3, rect.position.y), new Vector2(25, rect.height));
            posX += 30;
            var minRect = new Rect(new Vector2(posX, rect.position.y), new Vector2(width, rect.height));
            posX += width;
            var minLabelRect = new Rect(new Vector2(posX + 3, rect.position.y), new Vector2(25, rect.height));
            posX += 30;
            var secondRect = new Rect(new Vector2(posX, rect.position.y), new Vector2(width, rect.height));
            posX += width;
            var secondLabelRect = new Rect(new Vector2(posX + 3, rect.position.y), new Vector2(25, rect.height));

            EditorGUI.LabelField(labelRect, label);
            var hours = EditorGUI.IntField(hourRect, dateTime.Hour);
            EditorGUI.LabelField(hourLabelRect, "H");
            var minutes = EditorGUI.IntField(minRect, dateTime.Minute);
            EditorGUI.LabelField(minLabelRect, "M");
            var seconds = EditorGUI.IntField(secondRect, dateTime.Second);
            EditorGUI.LabelField(secondLabelRect, "S");

            return seconds + (minutes * 60) + (hours * 3600);
        }
    }
}