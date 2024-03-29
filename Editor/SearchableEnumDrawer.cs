﻿using System;
using DeepFreeze.Packages.Toolbox.Runtime;
using DeepFreeze.Packages.Toolbox.Runtime;
using UnityEditor;
using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Editor
{
	[CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
	public class SearchableEnumDrawer : PropertyDrawer 
	{
        private const string TypeError = "SearchableEnum can only be used on enum fields.";

        /// <summary>
        /// Cache of the hash to use to resolve the ID for the drawer.
        /// </summary>
        private int _idHash;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // If this is not used on an eunum, show an error
            if (property.type != "Enum")
            {
                GUIStyle errorStyle = "CN EntryErrorIconSmall";
                var r = new Rect(position)
                {
                    width = errorStyle.fixedWidth
                };
                position.xMin = r.xMax;
                GUI.Label(r, "", errorStyle);
                GUI.Label(position, TypeError);
                return;
            }
            
            // By manually creating the control ID, we can keep the ID for the
            // label and button the same. This lets them be selected together
            // with the keyboard in the inspector, much like a normal popup.
            if (_idHash == 0) 
                _idHash = "SearchableEnumDrawer".GetHashCode();
            
            var id = GUIUtility.GetControlID(_idHash, FocusType.Keyboard, position);
            
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, id, label);

            var buttonText = new GUIContent(property.enumDisplayNames[property.enumValueIndex]);
            if (DropdownButton(id, position, buttonText))
            {
                void OnSelect(int i)
                {
                    property.enumValueIndex = i;
                    property.serializedObject.ApplyModifiedProperties();
                }

                SearchablePopup.Show(position, property.enumDisplayNames,
                    property.enumValueIndex, OnSelect);
            }
            EditorGUI.EndProperty();
        }
        
        /// <summary>
        /// A custom button drawer that allows for a controlID so that we can
        /// sync the button ID and the label ID to allow for keyboard
        /// navigation like the built-in enum drawers.
        /// </summary>
        private static bool DropdownButton(int id, Rect position, GUIContent content)
        {
            var current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && current.button == 0)
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id && current.character =='\n')
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
                case EventType.Repaint:
                    EditorStyles.popup.Draw(position, content, id, false);
                    break;
            }
            return false;
        }
	}
}
