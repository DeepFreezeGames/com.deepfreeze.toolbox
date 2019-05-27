using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    public class SceneCameraWindow : EditorWindow
    {
        private static SceneCameraWindow _instance = null;

        [SerializeField] private Camera targetCamera = null;
        [SerializeField] private bool syncCamera = false;
        [SerializeField] private bool showSceneCameraData = true;
        [SerializeField] private CameraProperties cameraProperties = new CameraProperties();

        private bool _resetFovOnce = false;

        private Transform TargetParent => targetCamera != null ? targetCamera.transform.parent : null;

        private EditorWindow _gameView = null;
        private EditorWindow GameView
        {
            get
            {
                if (_gameView != null)
                {
                    return _gameView;
                }

                var assembly = typeof(UnityEditor.EditorWindow).Assembly;
                var type = assembly.GetType("UnityEditor.GameView");
                _gameView = EditorWindow.GetWindow<EditorWindow>(type);
                return _gameView;
            }
        }

        [MenuItem("Tools/Windows/Scene Camera")]
        private static void Initialize()
        {
            var window = GetWindow<SceneCameraWindow>();
            window.titleContent = new GUIContent("Scene Camera");
            window.Show();
            _instance = window;
        }

        private void OnEnable()
        {
            _instance = this;
            targetCamera = Camera.main;

            SceneViewApi.AddOnPreSceneGuiDelegate(OnPreSceneGUI);
        }

        private void OnDisable()
        {
            SceneViewApi.RemoveOnPreSceneGuiDelegate(OnPreSceneGUI);
        }

        private static void RepaintWindow()
        {
            if (_instance != null)
            {
                _instance.Repaint();
            }
        }

        private static void OnPreSceneGUI(SceneView sceneView)
        {
            if (SceneView.lastActiveSceneView != sceneView)
            {
                return;
            }

            _instance.OnPreSceneGUI();
        }

        private void OnPreSceneGUI()
        {
            var sceneViewCamera = SceneView.lastActiveSceneView.camera;

            if (_resetFovOnce)
            {
                _resetFovOnce = false;
            }
            else
            {
                sceneViewCamera.fieldOfView = cameraProperties.fov;
            }

            if (sceneViewCamera.transform.hasChanged)
            {
                RepaintWindow();
            }

            cameraProperties.Copy(sceneViewCamera, TargetParent);

            if (syncCamera && targetCamera != null)
            {
                cameraProperties.Paste(targetCamera);
                GameView.Repaint();
            }
        }

        private void OnGUI()
        {
            if (SceneView.lastActiveSceneView == null)
            {
                return;
            }

            using (new HorizontalBlock())
            {
                GUI.enabled = false;

                EditorGUILayout.ObjectField(
                    SceneView.lastActiveSceneView.camera,
                    typeof(Camera),
                    true,
                    GUILayout.MaxWidth(150)
                    );

                GUI.enabled = (targetCamera != null) && !syncCamera;
                if (GUILayout.Button("<-"))
                {
                    cameraProperties.Copy(targetCamera);
                    SetSceneCameraTransformData();
                }

                GUI.enabled = targetCamera != null;

                syncCamera = EditorGUILayout.Toggle(syncCamera, "IN LockButton", GUILayout.MaxWidth(15));

                GUI.enabled = targetCamera != null && !syncCamera;
                if (GUILayout.Button("->"))
                {
                    cameraProperties.Paste(targetCamera);
                }

                GUI.enabled = true;

                EditorGUIUtility.labelWidth = 80;
                targetCamera = (Camera)EditorGUILayout.ObjectField(targetCamera, typeof(Camera), true, GUILayout.MaxWidth(150));
                EditorGUIUtility.labelWidth = -1;
            }

            if (GUI.changed)
            {
                SceneView.lastActiveSceneView.Repaint();
            }

            HorizontalLine.Draw();
            EditorGUILayout.LabelField(EditorGUIUtility.ObjectContent(SceneView.lastActiveSceneView.camera, typeof(Camera)));

            EditorGUI.indentLevel++;
            if (showSceneCameraData)
            {
                GUI.changed = false;

                EditorGUIUtility.wideMode = true;
                DrawTransformData();
                using (new HorizontalBlock())
                {
                    cameraProperties.fov = EditorGUILayout.Slider(new GUIContent("Field of View"), cameraProperties.fov,
                        0.1f, 200f, GUILayout.ExpandWidth(true));

                    if (GUILayout.Button("Reset", GUILayout.MaxWidth(50f)))
                    {
                        _resetFovOnce = true;
                    }
                }

                if (GUI.changed)
                {
                    SetSceneCameraTransformData();
                    SceneView.lastActiveSceneView.Repaint();
                }
            }
        }

        private void DrawTransformData()
        {
            cameraProperties.localPosition = EditorGUILayout.Vector3Field("Position", cameraProperties.localPosition);
            var newLocalRotationEuler = EditorGUILayout.Vector3Field("Rotation", cameraProperties.localRotationEuler);
            if (newLocalRotationEuler != cameraProperties.localRotationEuler)
            {
                cameraProperties.localRotationEuler = newLocalRotationEuler;
                cameraProperties.localRotation = Quaternion.Euler(newLocalRotationEuler);
            }
        }

        private void SetSceneCameraTransformData()
        {
            var globalPosition = cameraProperties.localPosition;
            if (TargetParent != null)
            {
                globalPosition = TargetParent.TransformPoint(cameraProperties.localPosition);
            }

            var globalRotation = cameraProperties.localRotation;
            if (TargetParent != null)
            {
                globalRotation = TargetParent.transform.rotation * globalRotation;
            }

            SetSceneCameraTransformData(globalPosition, globalRotation);
        }

        private void SetSceneCameraTransformData(Vector3 cameraPosition, Quaternion cameraRotation)
        {
            var sceneView = SceneView.lastActiveSceneView;
            sceneView.rotation = cameraRotation;
            sceneView.pivot = cameraPosition + cameraRotation * new Vector3(0, 0, sceneView.cameraDistance);
        }
    }

    static class SceneViewApi
    {
        private static readonly Type TypeSceneView = typeof(SceneView);
        private static readonly FieldInfo FiOnPreSceneGuiDelegateFieldInfo = TypeSceneView.GetField(
            "onPreSceneGUIDelegate"
            , BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public
        );

        private static Action OnPreSceneGuiDelegate
        {
            get
            {
                if (FiOnPreSceneGuiDelegateFieldInfo == null)
                    return null;
                return (Action) FiOnPreSceneGuiDelegateFieldInfo.GetValue(null);
            }

            set
            {
                if (FiOnPreSceneGuiDelegateFieldInfo == null)
                    return;
                FiOnPreSceneGuiDelegateFieldInfo.SetValue(null, value);
            }
        }

        // Add delegate to UnityEditor.SceneView.onPreSceneGUIDelegate
        public static void AddOnPreSceneGuiDelegate(Action func)
        {
            OnPreSceneGuiDelegate = (Action) Delegate.Combine(func, OnPreSceneGuiDelegate);
        }

        // Remove delegate from UnityEditor.SceneView.onPreSceneGUIDelegate
        public static void RemoveOnPreSceneGuiDelegate(Action func)
        {
            OnPreSceneGuiDelegate = (Action) Delegate.Remove(func, OnPreSceneGuiDelegate);
        }
    }

    [Serializable]
    internal class CameraProperties
    {
        public float fov = 60f;
        public Vector3 localPosition = Vector3.zero;
        public Quaternion localRotation = Quaternion.identity;
        public Vector3 localRotationEuler = Vector3.zero;

        private SerializedObject targetTransformProperty;
        private SerializedProperty eulerHintProperty;

        public void Copy(Camera sceneCamera, Transform relativeParent)
        {
            fov = sceneCamera.fieldOfView;
            var targetTransform = sceneCamera.transform;

            Quaternion newLocalRotation;
            if (relativeParent != null)
            {
                localPosition = relativeParent.InverseTransformPoint(targetTransform.position);
                newLocalRotation = Quaternion.Inverse(relativeParent.rotation) * targetTransform.rotation;
            }
            else
            {
                localPosition = targetTransform.position;
                newLocalRotation = targetTransform.rotation;
            }

            if (localRotation != newLocalRotation)
            {
                var newLocalEuler = newLocalRotation.eulerAngles;

                localRotationEuler.x += Mathf.DeltaAngle(localRotationEuler.x, newLocalEuler.x);
                localRotationEuler.y += Mathf.DeltaAngle(localRotationEuler.y, newLocalEuler.y);
                localRotationEuler.z += Mathf.DeltaAngle(localRotationEuler.z, newLocalEuler.z);

                localRotation = newLocalRotation;
            }
        }

        public void Copy(Camera target)
        {
            fov = target.fieldOfView;

            var targetTransform = target.transform;
            localPosition = targetTransform.localPosition;

            PrepareProperty(targetTransform);
            eulerHintProperty.vector3Value = localRotationEuler;
            targetTransformProperty.ApplyModifiedProperties();

            targetTransform.localEulerAngles = localRotationEuler;
        }

        public void Paste(Camera target)
        {
            target.fieldOfView = fov;

            var targetTransform = target.transform;
            targetTransform.localPosition = localPosition;

            PrepareProperty(targetTransform);
            eulerHintProperty.vector3Value = localRotationEuler;
            targetTransformProperty.ApplyModifiedProperties();

            targetTransform.localEulerAngles = localRotationEuler;
        }

        private void PrepareProperty(Transform targetTransform)
        {
            if (targetTransformProperty != null && targetTransformProperty.targetObject == targetTransform)
            {
                return;
            }

            targetTransformProperty = new SerializedObject(targetTransform);
            eulerHintProperty = targetTransformProperty.FindProperty("m_LocalEulerAnglesHint");
        }
    }

    public static class HorizontalLine
    {
        private static GUIStyle _line = null;

        static HorizontalLine()
        {
            _line = new GUIStyle("box");
            _line.border.top = _line.border.bottom = 1;
            _line.margin.top = _line.margin.bottom = 1;
            _line.padding.top = _line.padding.bottom = 1;
            _line.border.left = _line.border.right = 1;
            _line.margin.left = _line.margin.right = 1;
            _line.padding.left = _line.padding.right = 1;
        }

        public static void Draw()
        {
            GUILayout.Box(GUIContent.none, _line, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
        }
    }
}
