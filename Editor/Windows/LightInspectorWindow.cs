using System;
using System.Collections.Generic;
using EditorGUIExtensions.Editor;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Windows
{
    public class LightInspectorWindow : EditorWindow
    {
        private enum Modes
        {
            Lights,
            ReflectionProbes
        }

        private static LightInspectorWindow _window;
        private Light[] _lights;
        private ReflectionProbe[] _reflectionProbes;

        private Modes _currentMode = Modes.Lights;
        private Vector2 _scrollPosMain = Vector2.zero;

        [MenuItem("Tools/Light Inspector")]
        public static void Initialize()
        {
            _window = GetWindow<LightInspectorWindow>();
            _window.titleContent = new GUIContent("Light Inspector");
            _window.Show();
        }

        private void OnEnable()
        {
            Refresh();
        }

        public void OnInspectorUpdate()
        {
            if (_lights.Length > 0)
            {
                Array.Clear(_lights, 0, _lights.Length);
            }

            if (_reflectionProbes.Length > 0)
            {
                Array.Clear(_reflectionProbes, 0, _reflectionProbes.Length);
            }

            Refresh();
        }

        private void Refresh()
        {
            _lights = FindObjectsOfType<Light>();
            _reflectionProbes = FindObjectsOfType<ReflectionProbe>();
        }

        private void OnGUI()
        {
            using (new HorizontalBlock(GUILayout.ExpandWidth(true)))
            {
                GUI.enabled = _currentMode != Modes.Lights;
                if (GUILayout.Button("Lights"))
                {
                    _currentMode = Modes.Lights;
                    _scrollPosMain = Vector2.zero;
                }

                GUI.enabled = _currentMode != Modes.ReflectionProbes;
                if (GUILayout.Button("Reflection Probes"))
                {
                    _currentMode = Modes.ReflectionProbes;
                    _scrollPosMain = Vector2.zero;
                }
                GUI.enabled = true;
            }

            switch (_currentMode)
            {
                case Modes.Lights:
                    DrawLightWindow();
                    break;
                case Modes.ReflectionProbes:
                    DrawReflectionProbeWindow();
                    break;
                default:
                    _currentMode = Modes.Lights;
                    Repaint();
                    return;
            }
        }

        #region LIGHTS
        private void DrawLightWindow()
        {
            if (_lights.Length == 0)
            {
                EditorGUILayoutHelper.CenteredMessage("No Lights Found In The Current Scene");
                return;
            }

            using (new HorizontalBlock("box"))
            {
                EditorGUILayout.LabelField("Enabled", GUILayout.MinWidth(100), GUILayout.Width(50));
                EditorGUILayout.LabelField("Name", GUILayout.MinWidth(100), GUILayout.Width(140));
                EditorGUILayout.LabelField("Type", GUILayout.MinWidth(100), GUILayout.Width(100));
                EditorGUILayout.LabelField("Mode", GUILayout.MinWidth(100), GUILayout.Width(100));
                EditorGUILayout.LabelField("Color", GUILayout.MinWidth(100), GUILayout.Width(50));
                EditorGUILayout.LabelField("Intensity", GUILayout.MinWidth(100), GUILayout.Width(200));
            }

            using (new ScrollviewBlock(ref _scrollPosMain))
            {
                foreach (var light in _lights)
                {
                    DrawLightField(light);
                }
            }
        }

        private void DrawLightField(Light light)
        {
            using (new HorizontalBlock("box"))
            {
                light.enabled = EditorGUILayout.Toggle(light.enabled, GUILayout.MinWidth(100), GUILayout.Width(50));
                light.name = EditorGUILayout.TextField(light.name, GUILayout.MinWidth(100), GUILayout.Width(140));
                light.type = (LightType)EditorGUILayout.EnumPopup(light.type, GUILayout.MinWidth(100), GUILayout.Width(100));
                light.lightmapBakeType = (LightmapBakeType)EditorGUILayout.EnumPopup(light.lightmapBakeType, GUILayout.MinWidth(100), GUILayout.Width(100));
                light.color = EditorGUILayout.ColorField(light.color, GUILayout.MinWidth(100), GUILayout.Width(50));
                light.intensity = EditorGUILayout.Slider(light.intensity, 0, 10, GUILayout.MinWidth(100), GUILayout.Width(200));
                if (GUILayout.Button("Select", EditorStyles.miniButton))
                {
                    Selection.activeGameObject = light.gameObject;
                }
            }
        }
        #endregion

        #region REFLECTION PROBES
        private void DrawReflectionProbeWindow()
        {
            if (_reflectionProbes.Length == 0)
            {
                EditorGUILayoutHelper.CenteredMessage("No Reflection Probes Found In The Current Scene");
                return;
            }

            using (new HorizontalBlock("box"))
            {
                EditorGUILayout.LabelField("Enabled", GUILayout.MinWidth(100), GUILayout.Width(50));
                EditorGUILayout.LabelField("Name", GUILayout.MinWidth(100), GUILayout.Width(140));
                EditorGUILayout.LabelField("Mode", GUILayout.MinWidth(100), GUILayout.Width(100));
                EditorGUILayout.LabelField("Intensity", GUILayout.MinWidth(100), GUILayout.Width(200));
            }

            using (new ScrollviewBlock(ref _scrollPosMain))
            {
                foreach (var reflectionProbe in _reflectionProbes)
                {
                    DrawReflectionProbeField(reflectionProbe);
                }
            }
        }

        private void DrawReflectionProbeField(ReflectionProbe reflectionProbe)
        {
            using (new HorizontalBlock("box"))
            {
                reflectionProbe.enabled = EditorGUILayout.Toggle(reflectionProbe.enabled, GUILayout.MinWidth(100), GUILayout.Width(50));
                reflectionProbe.name = EditorGUILayout.TextField(reflectionProbe.name, GUILayout.MinWidth(100), GUILayout.Width(140));
                reflectionProbe.mode = (UnityEngine.Rendering.ReflectionProbeMode)EditorGUILayout.EnumPopup(reflectionProbe.mode, GUILayout.MinWidth(100), GUILayout.Width(100));
                reflectionProbe.intensity = EditorGUILayout.Slider(reflectionProbe.intensity, 0, 10, GUILayout.MinWidth(100), GUILayout.Width(200));
                if (GUILayout.Button("Select", EditorStyles.miniButton))
                {
                    Selection.activeGameObject = reflectionProbe.gameObject;
                }
            }
        }
        #endregion
    }
}
