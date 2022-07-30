using System;
using DeepFreeze.Packages.Toolbox.Runtime;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DeepFreeze.Packages.Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ResourceReferenceAttribute))]
    public class ResourceReferenceAttributeDrawer : PropertyDrawer
    {
        private Object _object;
        private Type _allowedType;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            _allowedType = ((ResourceReferenceAttribute)attribute).Type;

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (!string.IsNullOrEmpty(property.stringValue))
            {
                var asset = Resources.Load(property.stringValue);
                _object = asset;
            }
            else
            {
                _object = null;
            }

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            _object = EditorGUI.ObjectField(position, _object, _allowedType, false);
            if (_object != null)
            {
                if (!AssetHelper.IsResourceAsset(_object, out var resourcePath))
                {
                    _object = null;
                }

                //Prevent constant changing/serializing
                if (property.stringValue != resourcePath)
                {
                    property.stringValue = resourcePath;
                }
            }
            else
            {
                //Prevent constant changing/serializing
                if (!string.IsNullOrEmpty(property.stringValue))
                {
                    property.stringValue = string.Empty;
                }
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}