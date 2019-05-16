using UnityEngine;

namespace Toolbox.Runtime
{
    public static class RendererExtensions
    {
        private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            var screenBounds = new Rect(0f, 0f, Screen.width, Screen.height); // Screen space bounds (assumes camera renders across the entire screen)
            var objectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(objectCorners);
 
            var visibleCorners = 0;
            Vector3 tempScreenSpaceCorner; // Cached
            
            foreach (var corner in objectCorners)
            {
                tempScreenSpaceCorner = camera.WorldToScreenPoint(corner); // Transform world space position of corner to screen space
                if (screenBounds.Contains(tempScreenSpaceCorner)) // If the corner is inside the screen
                {
                    visibleCorners++;
                }
            }
            
            return visibleCorners;
        }
 
        public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            return CountCornersVisibleFrom(rectTransform, camera) == 4; // True if all 4 corners are visible
        }

        public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            return CountCornersVisibleFrom(rectTransform, camera) > 0; // True if any corners are visible
        }
    }
}