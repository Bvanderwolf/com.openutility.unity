using UnityEngine;

namespace OpenUtility.Hierarchy
{
    public static class TransformUtility
    {
        public static Rect GetWorldRect(this RectTransform transform)
        {
            Vector3[] corners = new Vector3[4];
            transform.GetWorldCorners(corners);
            
            float xMin = corners[0].x;
            float xMax = corners[2].x;
            float yMin = corners[0].y;
            float yMax = corners[2].y;

            return (new Rect(xMin, yMin, xMax - xMin, yMax - yMin));
        }

        public static bool Overlaps(this RectTransform transform, RectTransform other)
        {
            Rect lhs = transform.GetWorldRect();
            Rect rhs = other.GetWorldRect();
            
            return (lhs.Overlaps(rhs));
        }

        public static bool IsLookingAt(this Transform transform, Transform target, float margin)
        {
            float angle = transform.GetLookAtAngle(target);
            return (angle <= margin);
        }
        
        public static float GetLookAtAngle(this Transform from, Transform to)
        {
            Vector3 directionToTarget = (to.position - from.position).normalized;
            Vector3 forwardDirection = from.forward;
            
            return Vector3.Angle(forwardDirection, directionToTarget);
        }
    }
}
