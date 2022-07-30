using System;
using DeepFreeze.Packages.Toolbox.Runtime;
using UnityEditor;
using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(DateAttribute))]
    public class DateAttributeDrawer : PropertyDrawer
    {
        private static GUIStyle _labelStyle;
        public static GUIStyle LabelStyle
        {
            get
            {
                if (_labelStyle == null)
                {
                    _labelStyle = new GUIStyle(EditorStyles.label)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }

                return _labelStyle;
            }
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var dateTimeValue = property.doubleValue.ToDateTime();
            var year = dateTimeValue.Year;
            var month = dateTimeValue.Month;
            var day = dateTimeValue.Day;
            var hour = dateTimeValue.Hour;
            var minute = dateTimeValue.Minute;
            
            var fieldWidth = position.width / 10f;

            year = EditorGUI.IntField(new Rect(position.x, position.y, fieldWidth * 2f, position.height), year);
            EditorGUI.LabelField(new Rect(position.x += (fieldWidth * 2f), position.y, fieldWidth, position.height), "Y", LabelStyle);
            month = EditorGUI.IntField(new Rect(position.x += fieldWidth, position.y, fieldWidth, position.height), month);
            EditorGUI.LabelField(new Rect(position.x += fieldWidth, position.y, fieldWidth, position.height), "M", LabelStyle);
            day = EditorGUI.IntField(new Rect(position.x += fieldWidth, position.y, fieldWidth, position.height), day);
            EditorGUI.LabelField(new Rect(position.x += fieldWidth, position.y, fieldWidth, position.height), "D", LabelStyle);
            position.x += (fieldWidth / 2f);
            hour = EditorGUI.IntField(new Rect(position.x += fieldWidth, position.y, fieldWidth, position.height), hour);
            EditorGUI.LabelField(new Rect(position.x += fieldWidth, position.y, fieldWidth /2f, position.height), ":", LabelStyle);
            minute = EditorGUI.IntField(new Rect(position.x += (fieldWidth / 2f), position.y, fieldWidth, position.height), minute);

            var output = new DateTime(year, month, day, hour, minute, 0);
            property.doubleValue = output.ToEpoch();
            
            EditorGUI.EndProperty();
        }
    }
}