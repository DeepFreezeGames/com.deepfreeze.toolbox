using UnityEngine;

namespace Toolbox.Runtime.Extensions
{
    public static class Vector3Extensions
    {
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

        public static Vector3 SetX(this Vector3 vector, float value)
        {
            vector.x = value;
            return vector;
        }

        public static Vector3 SetY(this Vector3 vector, float value)
        {
            vector.y = value;
            return vector;
        }

        public static Vector3 SetZ(this Vector3 vector, float value)
        {
            vector.z = value;
            return vector;
        }

        public static Vector3 Invert(this Vector3 vector)
        {
            return new Vector3
            (
                1.0f / vector.x,
                1.0f / vector.y,
                1.0f / vector.z
            );
        }

        public static Vector3 Project(this Vector3 vector, Vector3 projectedVector)
        {
            var dot = Vector3.Dot(vector, projectedVector);
            return dot * projectedVector;
        }

        public static Vector3 Reject(this Vector3 vector, Vector3 rejectedVector)
        {
            return vector - vector.Project(rejectedVector);
        }
    }
}