using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
	public class AssetDependenciesWindow : EditorWindow
	{
		internal enum ViewMode
		{
			All = 0,
			SceneOnly = 1,
			Project = 2
		}

		private static readonly string[] ViewModeOptions = {"All", "Scene", "Project" };

		private static AssetDependenciesWindow _window;
		private const float ItemSpacing = 10f;
		private static ViewMode _viewMode = ViewMode.All;

		private string CurrentSelectionName => CurrentSelection != null ? CurrentSelection.name : "Nothing selected";

		public static string currentSelectionPath { get; private set; }
		private static Object _currentSelection { get; set; }
		public static Object CurrentSelection
		{
			get => _currentSelection;
			set
			{
				_currentSelection = value;
				currentSelectionPath = AssetDatabase.GetAssetPath(_currentSelection);
			}
		}
		private List<Object> _sceneDependencies = new List<Object>();
		private List<Object> _projectDependencies = new List<Object>();
		private List<Object> _objectsToShow = new List<Object>();

		private Vector2 _scrollPosMainArea = Vector2.zero;

		public static void Initialize(string filePath)
		{
			var objectAtPath = AssetDatabase.LoadAssetAtPath<Object>(filePath);
			if (objectAtPath == null)
			{
				Debug.LogError($"Could not load object at path ({filePath})");
				return;
			}
			Initialize(objectAtPath);
		}

		public static void Initialize(Object selection)
		{
			CurrentSelection = selection;
			Initialize();
		}

		[MenuItem("Tools/Dependency Viewer")]
		private static void Initialize()
		{
			_window = GetWindow<AssetDependenciesWindow>();
			_window.titleContent = new GUIContent("Dependencies");
			_window.Show();
		}

		private void OnEnable()
		{
			UpdateDependencies();
		}

		private void UpdateDependencies()
		{
			_objectsToShow.Clear();
			switch (_viewMode)
			{
				case ViewMode.SceneOnly:
					GetSceneDependencies();
					_objectsToShow = _sceneDependencies;
					break;
				case ViewMode.Project:
					GetProjectDependencies();
					_objectsToShow = _projectDependencies;
					break;
				case ViewMode.All:
					GetSceneDependencies();
					GetProjectDependencies();
					_objectsToShow = _sceneDependencies;
					foreach (var projectDependency in _projectDependencies)
					{
						_objectsToShow.Add(projectDependency);
					}
					break;
				default:
					_viewMode = ViewMode.Project;
					break;
			}
		}

		private void GetSceneDependencies()
		{

		}

		private void GetProjectDependencies()
		{
			_projectDependencies.Clear();
			var dependencyPaths = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(CurrentSelection));
			foreach (var path in dependencyPaths)
			{
				if(path == currentSelectionPath)
					return;
				
				_projectDependencies.Add(AssetDatabase.LoadAssetAtPath<Object>(path));
			}
		}

		private void OnSelectionChange()
		{
			//Selection Changed
			Debug.Log($"Current selection: ({Selection.activeObject.name})");
			CurrentSelection = Selection.activeObject;
			UpdateDependencies();
		}

		private void OnGUI()
		{
			ObjectLabel();
			MainArea();
			OptionToolbar();
		}

		private void ObjectLabel()
		{
			using (new HorizontalBlock(EditorStyles.helpBox, GUILayout.ExpandWidth(true)))
			{
				GUILayout.Label("Current Selection", EditorStyles.boldLabel);
				GUILayout.Label(CurrentSelectionName);
			}
		}

		private void MainArea()
		{
			using (new VerticalBlock())
			{
				using (new HorizontalBlock())
				{
					_viewMode = (ViewMode)EditorGUILayoutHelper.EnumButtonField("", (int) _viewMode, ViewModeOptions);
				}
				using (new ScrollviewBlock(ref _scrollPosMainArea))
				{
					foreach (var dependency in _objectsToShow)
					{
						DependencyField(dependency);
					}
				}
			}
		}

		private void DependencyField(Object item)
		{
			using (new VerticalBlock(EditorStyles.helpBox))
			{
				using (new HorizontalBlock())
				{
					GUILayout.Label(item.name);
				}

				using (new HorizontalBlock())
				{
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Select"))
					{

					}

					if (GUILayout.Button("Replace"))
					{

					}
				}
			}

			GUILayout.Space(ItemSpacing);
		}

		private void OptionToolbar()
		{
			using (new HorizontalBlock())
			{
				if (GUILayout.Button("Replace With"))
				{

				}

				if (GUILayout.Button("Remove Connections"))
				{

				}

				if (GUILayout.Button("Cancel"))
				{
					Close();
				}
			}
		}
	}
}
