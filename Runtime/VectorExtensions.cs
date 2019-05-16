using UnityEngine;

namespace Extensions
{
    public static class VectorExtensions
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

        public static bool TryParse(string rawString, out Vector3 vector3)
        {
            vector3 = Vector3.zero;
            var shards = rawString.Split(',');
            if (!float.TryParse(shards[0].Trim(), out var xValue))
                return false;
            if (!float.TryParse(shards[1].Trim(), out var yValue))
                return false;
            if (!float.TryParse(shards[2].Trim(), out var zValue))
                return false;

            vector3 = new Vector3(xValue, yValue, zValue);
            return true;
        }
    }
}
