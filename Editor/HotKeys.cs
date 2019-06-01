using System.Collections.Generic;
using System.IO;
using Toolbox.Editor.Windows;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolbox.Editor
{
    [InitializeOnLoad]
    public class HotKeys
    {
        static HotKeys()
        {
            //_showRenderBounds = EditorPrefs.GetBool(ShowRenderBoundsKey, false);

            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        //private static Ray _lastMouseRay;
        private static void OnSceneGUI(SceneView sceneView)
        {
            //_lastMouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            /*if (_showRenderBounds && Selection.activeGameObject != null)
            {
                var renderer = Selection.activeGameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    var oldColor = Gizmos.color;
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireCube(renderer.bounds.center, renderer.bounds.extents);
                    Handles.Label(new Vector3(renderer.bounds.center.x, renderer.bounds.center.y - renderer.bounds.extents.y, renderer.bounds.center.z),
                        $"X:{renderer.bounds.extents.x.ToString()} Y: {renderer.bounds.extents.y.ToString()} Z: {renderer.bounds.extents.z.ToString()}");
                    Gizmos.color = oldColor;
                }
            }*/
        }

        private const string EditorPrefScreenShotPath = "Screenshots";

        [MenuItem("Tools/Take Screenshot")]
        public static void CaptureScreenshot()
        {
            var defaultPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, EditorPrefScreenShotPath);
            var path = UnityEditor.EditorPrefs.GetString(EditorPrefScreenShotPath, defaultPath);
            var filename = EditorUtility.SaveFilePanel("Save screenshot", path, "sample_shot.png", "png");
            // Check if user cancelled
            if (filename == "")
                return;

            UnityEditor.EditorPrefs.SetString(EditorPrefScreenShotPath, System.IO.Path.GetDirectoryName(filename));
            ScreenCapture.CaptureScreenshot(filename, 1);
        }

        [MenuItem("Tools/Hotkeys/Lookup asset GUID %&l", priority = 0)]
        public static void LookUpAsset()
        {
            LookupAssetWindow.Open();
        }

        #region CUT AND PASTE
        public static List<Object> ObjectsSelectedForCut;

        [MenuItem("Tools/Hotkeys/Cut GameObjects _%#X", priority = 50)]
        public static void Cut()
        {
            if (Selection.objects.Length > 0)
            {
                ObjectsSelectedForCut = new List<Object>(Selection.objects);
                foreach (var o in ObjectsSelectedForCut)
                {
                    EditorUtility.SetDirty(o);
                }
                Debug.Log($"Marked {ObjectsSelectedForCut.Count.ToString()} for movement. Press Ctrl+V to move.");
            }
        }

        [MenuItem("Tools/Hotkeys/Paste GameObjects _%#V", priority = 50)]
        public static void Paste()
        {
            if (ObjectsSelectedForCut == null)
            {
                Debug.Log("Use Ctrl+Shift+X first to mark objects for moving.");
                return;
            }

            Transform newParent = null;
            var moveToDestScene = false;

            // Fill dest_scene with random stuff because it is a struct and hence non-nullable
            var destScene = SceneManager.GetActiveScene();

            if (Selection.activeGameObject != null && Selection.objects.Length == 1)
            {
                // In this case, we parent under another object
                newParent = Selection.activeGameObject.transform;
            }
            else if (Selection.activeGameObject == null && Selection.instanceIDs.Length == 1)
            {
                // In this case, we may have selected a scene
                var method = typeof(EditorSceneManager).GetMethod("GetSceneByHandle", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                var obj = method.Invoke(null, new object[] { Selection.instanceIDs[0] });
                if (obj is Scene scene)
                {
                    if (scene.isLoaded)
                    {
                        destScene = scene;
                        moveToDestScene = true;
                    }
                }
            }
            else
            {
                Debug.Log("You must select exactly one GameObject or one scene to be the parent of the pasted object(s).");
                return;
            }

            // Perform move
            foreach (var obj in ObjectsSelectedForCut)
            {
                var go = obj as GameObject;
                if (go == null)
                {
                    continue;
                }
                Undo.SetTransformParent(go.transform, newParent, "Moved objects");
                if (moveToDestScene)
                {
                    // Moving to root of scene.
                    SceneManager.MoveGameObjectToScene(go, destScene);
                }
            }
            ObjectsSelectedForCut = null;
        }
        #endregion

        [MenuItem("Tools/Hotkeys/Deselect &d", priority = 100)]
        public static void Deselect()
        {
            Selection.activeGameObject = null;
        }

        #region PlayerPrefs
        [MenuItem("Tools/Player Prefs/Clear All Player Prefs", priority = 100)]
        public static void ClearPlayerPrefs()
        {
            //Delete all keys
        }

        [MenuItem("Tools/Player Prefs/Delete Player Pref", priority = 100)]
        public static void DeletePlayerPrefByName()
        {
            //Get all player pref keys
            //Delete selected
        }
        #endregion

        #region EditorPrefs
        [MenuItem("Tools/Editor Prefs/Clear All Editor Prefs", priority = 200)]
        public static void ClearEditorPrefs()
        {
            EditorPrefs.DeleteAll();
        }
        #endregion

        [MenuItem("Tools/Don't Panic", priority = 1000)]
        public static void ForceReimport()
        {
            AssetDatabase.Refresh();
        }
    }
}
