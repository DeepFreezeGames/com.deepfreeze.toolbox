using UnityEngine;

namespace Toolbox.Runtime
{
    public class MathUtils
    {
        #region CLAMPING
        public static float ClampAngle(float angle, float minAngle, float maxAngle)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, minAngle, maxAngle);
        }

        public static float ClampAngle(float angle, Vector2 constraints)
        {
            return ClampAngle(angle, constraints.x, constraints.y);
        }

        public static double Clamp(double value, double minValue, double maxValue)
        {
            if (value < minValue)
                return minValue;
            if (value > maxValue)
                return maxValue;
            return value;
        }

        public static long Clamp(long value, long minValue, long maxValue)
        {
            if (value < minValue)
                return minValue;
            if (value > maxValue)
                return maxValue;
            return value;
        }
        #endregion

        public static float ClosestDistance(Collider a, Collider b)
        {
            return Vector3.Distance(a.ClosestPointOnBounds(b.transform.position),
                b.ClosestPointOnBounds(a.transform.position));
        }
    }
}