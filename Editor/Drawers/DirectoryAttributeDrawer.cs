using DeepFreeze.Packages.Toolbox.Runtime;
using UnityEditor;
using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(DirectoryAttribute))]
    public class DirectoryAttributeDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 24f;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            //Remove indent
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            
            var fieldRect = new Rect(position.x, position.y, position.width - 34, position.height);
            var buttonRect = new Rect(position.x + position.width - 29, position.y, ButtonWidth, position.height);

            EditorGUI.PropertyField(fieldRect, property, GUIContent.none);
            var buttonValue = EditorGUI.Toggle(buttonRect, false, GUIStyle.none);
            if (buttonValue)
            {
                var rawPath = EditorUtility.OpenFolderPanel("Select Target Folder", Application.dataPath, "Assets");
                if (CheckPath(rawPath, out var newPath))
                {
                    property.stringValue = newPath;
                }
                buttonValue = false;
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private bool CheckPath(string input, out string output)
        {

            output = input;
            return true;
        }
    }
}
