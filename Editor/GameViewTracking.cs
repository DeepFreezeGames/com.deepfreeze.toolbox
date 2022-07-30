using UnityEditor;
using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Editor
{
    public static class GameViewTracking
    {
        private static bool _isEnabled;
        private const string MenuName = "Tools/Hotkeys/Game View Tracking _%#K";

        private static Transform _cameraTransform;

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
            switch (enabled)
            {
                case true when !_isEnabled:
                    SceneView.duringSceneGui += SceneGuiCallback;
                    _isEnabled = true;
                    break;
                case false when _isEnabled:
                    SceneView.duringSceneGui -= SceneGuiCallback;
                    _isEnabled = false;
                    break;
            }
        }

        private static void SceneGuiCallback(SceneView sceneView)
        {
            if(Camera.main == null)
            {
                return;
            }

            if(!sceneView.camera.orthographic)
            {
                _cameraTransform = sceneView.camera.transform;
                Camera.main.transform.SetPositionAndRotation(_cameraTransform.position - 0.1f * _cameraTransform.forward, _cameraTransform.rotation);
            }
        }
    }
}
