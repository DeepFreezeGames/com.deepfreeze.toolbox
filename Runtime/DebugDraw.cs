using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Runtime
{
    public class DebugDraw
    {
        private const int Segments = 20;
        private const float Arc = Mathf.PI * 2.0f / Segments;
        
        public static void Circle(Vector3 center, Vector3 normal, float radius, Color color)
        {
            CalculatePlaneVectorsFromNormal(normal, out var v1, out var v2);
            CircleInternal(center, v1, v2, radius, color);
        }
        
        public static void Sphere(Vector3 center, float radius)
        {
            Sphere(center, radius, Color.white);
        }

        public static void Sphere(Vector3 center, float radius, Color color, float duration = 0)
        {
            CircleInternal(center, Vector3.right, Vector3.up, radius, color, duration);
            CircleInternal(center, Vector3.forward, Vector3.up, radius, color, duration);
            CircleInternal(center, Vector3.right, Vector3.forward, radius, color, duration);
        }
        
        public static void Cylinder(Vector3 center, Vector3 normal, float radius, float halfHeight, Color color, float duration = 0)
        {
            CalculatePlaneVectorsFromNormal(normal, out var v1, out var v2);

            var offset = normal * halfHeight;
            CircleInternal(center - offset, v1, v2, radius, color, duration);
            CircleInternal(center + offset, v1, v2, radius, color, duration);

            
            for (var i = 0; i < Segments; i++)
            {
                var p = center + v1 * Mathf.Cos(Arc * i) * radius + v2 * Mathf.Sin(Arc * i) * radius;
                Debug.DrawLine(p - offset, p + offset, color, duration);
            }
        }
        
        private static void CircleInternal(Vector3 center, Vector3 v1, Vector3 v2, float radius, Color color, float duration = 0)
        {
            var p1 = center + v1 * radius;
            for (var i = 1; i <= Segments; i++)
            {
                var p2 = center + Mathf.Cos(Arc * i) * radius * v1 + Mathf.Sin(Arc * i) * radius * v2;
                Debug.DrawLine(p1, p2, color, duration);
                p1 = p2;
            }
        }

        private static void CalculatePlaneVectorsFromNormal(Vector3 normal, out Vector3 v1, out Vector3 v2)
        {
            if (Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < 0.99)
            {
                v1 = Vector3.Cross(Vector3.up, normal).normalized;
                v2 = Vector3.Cross(normal, v1);
            }
            else
            {
                v1 = Vector3.Cross(Vector3.left, normal).normalized;
                v2 = Vector3.Cross(normal, v1);
            }
        }
        
        public static void Arrow(Vector3 pos, float angle, Color color, float length = 1f, float tipSize = 0.25f, float width = 0.5f)
        {
            var angleRot = Quaternion.AngleAxis(angle, Vector3.up);
            var dir = angleRot * Vector3.forward;
            Arrow(pos, dir, color, length, tipSize, width);           
        }
        
        public static void Arrow(Vector3 pos, Vector2 direction, Color color, float length = 1f, float tipSize = 0.25f, float width = 0.5f)
        {
            var dir = new Vector3(direction.x, 0f, direction.y);
            Arrow(pos, dir, color, length, tipSize, width);
        }
        
        public static void Arrow(Vector3 pos, Vector3 direction, Color color, float length=1f, float tipSize=0.25f, float width=0.5f)
        {
            direction.Normalize();
            
            var sideLen = length - length * tipSize;
            var widthOffset = Vector3.Cross(direction, Vector3.up) * width;

            var baseLeft = pos + widthOffset * 0.3f;
            var baseRight = pos - widthOffset * 0.3f;
            var tip = pos + direction * length;
            var upCornerInRight = pos - widthOffset * 0.3f + direction * sideLen;
            var upCornerInLeft = pos + widthOffset * 0.3f + direction * sideLen;
            var upCornerOutRight = pos - widthOffset * 0.5f + direction * sideLen;
            var upCornerOutLeft = pos + widthOffset * 0.5f + direction * sideLen;
        
            Debug.DrawLine(baseLeft, baseRight, color);
            Debug.DrawLine(baseRight, upCornerInRight, color);
            Debug.DrawLine(upCornerInRight, upCornerOutRight, color);
            Debug.DrawLine(upCornerOutRight, tip, color);
            Debug.DrawLine(tip, upCornerOutLeft, color);
            Debug.DrawLine(upCornerOutLeft, upCornerInLeft, color);
            Debug.DrawLine(upCornerInLeft, baseLeft, color);
        }
    }
}
