using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Runtime.Attributes
{
    public class MinMaxAttribute : PropertyAttribute
    {
        public readonly float MinLimit = 0;
        public readonly float MaxLimit = 1;
        public readonly bool ShowEditRange;
        public readonly bool ShowDebugValues;
        public readonly bool ShowLimits;

        public MinMaxAttribute(int min, int max, bool showEditRange = false, bool showDebugValues = false, bool showLimits = false)
        {
            MinLimit = min;
            MaxLimit = max;
            ShowEditRange = showEditRange;
            ShowDebugValues = showDebugValues;
            ShowLimits = showLimits;
        }
    }
}
