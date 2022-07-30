using UnityEditor;
using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Editor.Windows
{
    public class LookupAssetWindow : EditorWindow
    {
        private static string _guid;
        private static Object _asset;
        private static string _path;
        
        public static void Open()
        {
            var window = GetWindow<LookupAssetWindow>(false);
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 800, 150);
            window.ShowPopup();
            _guid = "";
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Enter asset GUID below", EditorStyles.wordWrappedLabel);
            GUILayout.Space(20);

            EditorGUI.BeginChangeCheck();
            _guid = EditorGUILayout.TextField("GUID", _guid);
            var guidChanged = EditorGUI.EndChangeCheck();

            EditorGUI.BeginChangeCheck();
            _guid = EditorGUILayout.TextField("Asset", _guid);
            var pathChanged = EditorGUI.EndChangeCheck();

            EditorGUI.BeginChangeCheck();
            _asset = EditorGUILayout.ObjectField("Asset", _asset, typeof(Object), false);
            var assetChanged = EditorGUI.EndChangeCheck();

            GUILayout.Space(20);
            if (GUILayout.Button("Close"))
                this.Close();

            if(guidChanged)
            {
                _path = AssetDatabase.GUIDToAssetPath(_guid);
                _asset = AssetDatabase.LoadAssetAtPath(_path, typeof(Object));
            }
            else if (pathChanged)
            {
                _asset = AssetDatabase.LoadAssetAtPath(_path, typeof(Object));
                _guid = AssetDatabase.AssetPathToGUID(_path);
            }
            else if (assetChanged)
            {
                _path = AssetDatabase.GetAssetPath(_asset);
                _guid = AssetDatabase.AssetPathToGUID(_path);
            }
        }
    }
}