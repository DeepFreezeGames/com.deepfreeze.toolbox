using System;
using Toolbox.Runtime;
using Toolbox.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(EnumBitFieldAttribute))]
    public class EnumBitFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!(attribute is EnumBitFieldAttribute enumFlagsAttribute)) 
                return;
            
            var names = Enum.GetNames(enumFlagsAttribute.EnumType);
            property.intValue = EditorGUI.MaskField( position, label, property.intValue, names );
        }
    }
}
