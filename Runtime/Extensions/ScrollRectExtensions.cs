using UnityEngine;
using UnityEngine.UI;

namespace Toolbox.Runtime.Extensions
{
    public static class ScrollRectExtensions
    {
        public static void ScrollToTop(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = Vector2.up;
        }

        public static void ScrollToBottom(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = Vector2.zero;
        }
    }
}