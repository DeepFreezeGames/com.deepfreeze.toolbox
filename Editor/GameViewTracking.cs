using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    public static class GameViewTracking
    {
        private static bool _isEnabled;
        private const string MenuName = "Tools/Hotkeys/Game View Tracking _%#K";

        [MenuItem(MenuName)]
        public static void ToggleGameViewTracking()
        {
            SetEnabled(!_isEnabled);
        }

        [MenuItem(MenuName, true)]
        public static bool ToggleGameViewTrackingValidate()
        {
            Menu.SetChecked(MenuName, _isEnabled);
            return true;
        }

        private static void SetEnabled(bool enabled)
        {
            if (enabled && !_isEnabled)
            {
                SceneView.duringSceneGui += SceneGuiCallback;
                _isEnabled = true;
            }
            else if (!enabled && _isEnabled)
            {
                SceneView.duringSceneGui -= SceneGuiCallback;
                _isEnabled = false;
            }
        }

        private static void SceneGuiCallback(SceneView sceneView)
        {
            if(Camera.main == null)
                return;

            if(!sceneView.camera.orthographic)
                Camera.main.transform.SetPositionAndRotation(sceneView.camera.transform.position - 0.1f * sceneView.camera.transform.forward, sceneView.camera.transform.rotation);
        }
    }
}
