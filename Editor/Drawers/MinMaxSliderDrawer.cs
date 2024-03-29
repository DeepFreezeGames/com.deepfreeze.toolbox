﻿using System;
using DeepFreeze.Packages.Toolbox.Runtime;
using DeepFreeze.Packages.Toolbox.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DeepFreeze.Packages.Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : PropertyDrawer
    {
        private const string MismatchedTypeWarning = "Invalid types for MinMaxSlider on field {0}: The types don't match";
		private const string InvalidTypeWarning = "Invalid type for MinMaxSlider on field {0}: MinMaxSlider can only be applied to a float or int fields";

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Use GetEndProperty instead of NextVisible since the max property should have a HideInInspector attribute
			// which would make NextVisible skip it.

			var minProperty = property;
			var maxProperty = property.GetEndProperty(true);
			var slider = attribute as MinMaxSliderAttribute;

			if (minProperty.propertyType != maxProperty.propertyType)
			{
				Debug.LogWarningFormat(MismatchedTypeWarning, property.propertyPath);
				EditorGUI.PropertyField(position, property, label);
			}
			else if (minProperty.propertyType != SerializedPropertyType.Float &&
			         minProperty.propertyType != SerializedPropertyType.Integer)
			{
				Debug.LogWarningFormat(InvalidTypeWarning, property.propertyPath);
				EditorGUI.PropertyField(position, property, label);
			}
			else
			{
				var rect = EditorGUI.PrefixLabel(position, label);

				var minRect = new Rect(rect.x, rect.y, rect.width * 0.2f, EditorGUIUtility.singleLineHeight);
				var sliderRect = new Rect(minRect.xMax + 5, rect.y, rect.width * 0.6f - 10,
					EditorGUIUtility.singleLineHeight);
				var maxRect = new Rect(sliderRect.xMax + 5, rect.y, rect.width * 0.2f,
					EditorGUIUtility.singleLineHeight);

				switch (minProperty.propertyType)
				{
					case SerializedPropertyType.Float:
					{
						var minimum = EditorGUI.FloatField(minRect, minProperty.floatValue);
						var maximum = EditorGUI.FloatField(maxRect, maxProperty.floatValue);

						EditorGUI.MinMaxSlider(sliderRect, ref minimum, ref maximum, slider.MinimumValue,
							slider.MaximumValue);

						minimum = SnapDrawer.Snap(minimum, slider.SnapValue);
						maximum = SnapDrawer.Snap(maximum, slider.SnapValue);

						minProperty.floatValue = Mathf.Clamp(minimum, slider.MinimumValue, maximum);
						maxProperty.floatValue = Mathf.Clamp(maximum, minimum, slider.MaximumValue);
						break;
					}

					case SerializedPropertyType.Integer:
					{
						var minimum = (float) EditorGUI.IntField(minRect, minProperty.intValue);
						var maximum = (float) EditorGUI.IntField(maxRect, maxProperty.intValue);

						EditorGUI.MinMaxSlider(sliderRect, ref minimum, ref maximum, slider.MinimumValue,
							slider.MaximumValue);

						var min = Mathf.RoundToInt(SnapDrawer.Snap(minimum, slider.SnapValue));
						var max = Mathf.RoundToInt(SnapDrawer.Snap(maximum, slider.SnapValue));

						minProperty.intValue = Mathf.Clamp(min, Mathf.RoundToInt(slider.MinimumValue), max);
						maxProperty.intValue = Mathf.Clamp(max, min, Mathf.RoundToInt(slider.MaximumValue));
						break;
					}

					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
    }
}
