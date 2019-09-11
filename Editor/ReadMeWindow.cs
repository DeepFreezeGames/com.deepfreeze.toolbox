using System;
using EditorGUIExtensions.Editor;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    public class ReadMeWindow : EditorWindow
    {
        private static ReadMeWindow _window;
        private static string _fileAddress = "";
        private static bool _fileLoaded = false;

        private Vector2 _scrollPosMain = Vector2.zero;

        [MenuItem("Tools/ReadMe Editor")]
        private static void Initialize()
        {
            _window = GetWindow<ReadMeWindow>();
            _window.titleContent = new GUIContent("ReadMe Editor");
            _window.Show();
        }

        private void OnEnable()
        {

        }

        private void OnDestroy()
        {

        }

        private void OnGUI()
        {
            if (_fileLoaded)
            {
                ShowFileEditor();
            }
            else
            {
                ShowNoFileMessage();
            }
        }

        private void ShowNoFileMessage()
        {
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            using (new VerticalBlock(EditorStyles.helpBox, GUILayout.ExpandWidth(true)))
            {
                GUILayout.Label("No ReadMe Loaded", EditorStyles.boldLabel);
                using (new HorizontalBlock())
                {
                    if (GUILayout.Button("Import ReadMe"))
                    {

                    }

                    if (GUILayout.Button("Create New ReadMe"))
                    {
                        _fileLoaded = true;
                        Repaint();
                    }
                }
            }
        }

        private void ShowFileEditor()
        {
            FileToolbar();
            MainFileEditor();
        }

        private void FileToolbar()
        {
            using (new HorizontalBlock(EditorStyles.toolbar))
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                {

                }
            }
        }

        private void MainFileEditor()
        {
            using (new VerticalBlock())
            {
                using (new ScrollviewBlock(ref _scrollPosMain))
                {

                }
            }
        }
    }
}
