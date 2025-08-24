using UnityEngine;

public static class Vision2D
{
    public static bool IsInFOV(Transform origin, Vector2 targetPos, float maxAngle, float maxDist)
    {
        Vector2 dir = (targetPos - (Vector2)origin.position);
        if (dir.magnitude > maxDist) return false;
        float angle = Vector2.Angle(origin.right, dir); // FOV
        return angle <= maxAngle * 0.5f;
    }

    public static bool HasLineOfSight(Vector2 origin, Vector2 target, LayerMask occluders)
    {
        var hit = Physics2D.Linecast(origin, target, occluders);
        return hit.collider == null;
    }
}
