using System;
using DeepFreeze.Packages.Toolbox.Runtime.Attributes;
using DeepFreeze.Packages.Toolbox.Runtime;
using UnityEditor;
using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(EnumBitFieldAttribute))]
    public class EnumBitFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is not EnumBitFieldAttribute enumFlagsAttribute)
            {
                return;
            }
            
            var names = Enum.GetNames(enumFlagsAttribute.EnumType);
            property.intValue = EditorGUI.MaskField( position, label, property.intValue, names );
        }
    }
}
