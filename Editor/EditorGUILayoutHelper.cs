﻿using System;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
	/// <summary>
	/// Creates a vertical block for Unity Editor extensions. To use this follow the example below.
	/// using (new VerticalBlock())
	/// {
	///		your code here...
	/// }
	/// </summary>
	public class VerticalBlock : IDisposable
	{
		public VerticalBlock(params GUILayoutOption[] options)
		{
			GUILayout.BeginVertical(options);
		}
		public VerticalBlock(GUIStyle style, params GUILayoutOption[] options)
		{
			GUILayout.BeginVertical(style, options);
		}
		public void Dispose()
		{
			GUILayout.EndVertical();
		}
	}

	/// <summary>
	/// Creates a scrollview block for Unity Editor extensions. To use this follow the example below.
	/// using (new ScrollviewBlock(ref someVector2ForScrollPos))
	/// {
	///		your code here...
	/// }
	/// </summary>
	public class ScrollviewBlock : IDisposable
	{
		public ScrollviewBlock(ref Vector2 scrollPos, params GUILayoutOption[] options)
		{
			scrollPos = GUILayout.BeginScrollView(scrollPos, options);
		}
		public void Dispose()
		{
			GUILayout.EndScrollView();
		}
	}

	/// <summary>
	/// Creates a horizontal block for Unity Editor extensions. To use this follow the example below.
	/// using (new HorizontalBlock())
	/// {
	///		your code here...
	/// }
	/// </summary>
	///
	public class HorizontalBlock : IDisposable
	{
		public HorizontalBlock(params GUILayoutOption[] options)
		{
			GUILayout.BeginHorizontal(options);
		}
		public HorizontalBlock(GUIStyle style, params GUILayoutOption[] options)
		{
			GUILayout.BeginHorizontal(style, options);
		}
		public void Dispose()
		{
			GUILayout.EndHorizontal();
		}
	}

	public class ColoredBlock : IDisposable
	{
		public ColoredBlock(Color color)
		{
			GUI.color = color;
		}
		public void Dispose()
		{
			GUI.color = Color.white;
		}
	}

	public class EditorGUILayoutHelper
	{
		public static bool ToggleableButton(bool enabled, string label, params GUILayoutOption[] options)
		{
			GUI.enabled = enabled;
			if (GUILayout.Button(label, options))
			{
				GUI.enabled = true;
				return true;
			}
			GUI.enabled = true;
			return false;
		}

		public static bool ToggleableButton(bool enabled, string label, GUIStyle style, params GUILayoutOption[] options)
		{
			GUI.enabled = enabled;
			if (GUILayout.Button(label, style, options))
			{
				GUI.enabled = true;
				return true;
			}
			GUI.enabled = true;
			return false;
		}

		/// <summary>
		/// Creates a folder path textfield with a browse button.
		/// </summary>
		public static string FolderLabel(string name, float labelWidth, string path)
		{
			EditorGUILayout.BeginHorizontal();
			var filepath = EditorGUILayout.TextField(name, path);
			if (GUILayout.Button("Browse", GUILayout.MaxWidth(60)))
			{
				filepath = EditorUtility.SaveFolderPanel(name, path, "Folder");
			}
			EditorGUILayout.EndHorizontal();
			return filepath;
		}

		public static int EnumButtonField(string label, int currentSelection, string[] options)
		{
			using (new HorizontalBlock())
			{
				if (!string.IsNullOrEmpty(label))
				{
					GUILayout.Label(label);
				}
				for (var i = 0; i < options.Length; i++)
				{
					GUI.enabled = currentSelection != i;
					if (GUILayout.Button(options[i]))
					{
						return i;
					}
					GUI.enabled = true;
				}
			}

			return currentSelection;
		}

		public static void CenteredMessage(string message, params GUILayoutOption[] options)
		{
			using(new VerticalBlock())
			{
				GUILayout.FlexibleSpace();
				using(new HorizontalBlock(options))
				{
                    GUILayout.FlexibleSpace();
					GUILayout.Label(message, EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
				}
                GUILayout.FlexibleSpace();
			}
		}
	}
}
