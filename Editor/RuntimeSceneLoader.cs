using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeepFreeze.Packages.Toolbox.Editor
{
    [InitializeOnLoad]
    public static class RuntimeSceneLoader
    {
        static RuntimeSceneLoader()
        {
            SetCurrentScene();
            EditorSceneManager.sceneOpened += GetPlayModeStartScene;
        }

        private static void SetCurrentScene()
        {
            var currentScene = SceneManager.GetActiveScene();
            GetPlayModeStartScene(currentScene, OpenSceneMode.Single);
        }

        private static void GetPlayModeStartScene(Scene scene, OpenSceneMode sceneMode)
        {
            if (EditorBuildSettings.scenes.Length == 0)
            {
                return;
            }
            
            var defaultLoadScenePath = EditorBuildSettings.scenes[0].path;
            var defaultLoadSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(defaultLoadScenePath);
            if (defaultLoadScenePath == scene.path)
            {
                return;
            }
            
            if (defaultLoadSceneAsset != null)
            {
                SetPlayModeStartScene(defaultLoadSceneAsset);
            }
        }

        private static void SetPlayModeStartScene(SceneAsset sceneAsset)
        {
            EditorSceneManager.playModeStartScene = sceneAsset;
            Debug.Log($"Default Playmode Start Scene set to:\n{sceneAsset}");
        }
    }
}