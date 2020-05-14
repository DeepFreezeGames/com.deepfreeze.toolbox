using UnityEngine;

namespace Toolbox.Runtime.Extensions
{
    public static class SpriteExtensions
    {
        /// <summary>
        /// Returns the pivot point in normalized space
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public static Vector2 NormalizedPivot(this Sprite sprite)
        {
            var imageSize = sprite.rect.size;
            return new Vector2(
                sprite.pivot.x / imageSize.x,
                sprite.pivot.y / imageSize.y
            );
        }

        /// <summary>
        /// Returns the aspect ratio of the sprite in H/W
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public static float AspectRatio(this Sprite sprite)
        {
            return sprite.rect.height/ sprite.rect.width;
        }
    }
}