using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Toolbox.Runtime.Extensions
{
    public static class Vector2Extensions
    {
        public static bool TryParse(string rawString, out Vector2 vector2)
        {
            vector2 = Vector2.zero;
            var shards = rawString.Split(',');
            if (!float.TryParse(shards[0].Trim(), out var xValue))
                return false;
            if (!float.TryParse(shards[1].Trim(), out var yValue))
                return false;

            vector2 = new Vector2(xValue, yValue);
            return true;
        }

        public static Vector2 Rotate(this Vector2 vector, float angleInDegrees)
        {
            var sin = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
            var cos = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
            
            var tx = vector.x;
            var ty = vector.y;
            
            vector.x = (cos * tx) - (sin * ty);
            vector.y = (sin * tx) + (cos * ty);
            
            return vector;
        }

        public static Vector2 SetX(this Vector2 vector, float value)
        {
            vector.x = value;
            return vector;
        }

        public static Vector2 SetY(this Vector2 vector, float value)
        {
            vector.y = value;
            return vector;
        }
    }
}
