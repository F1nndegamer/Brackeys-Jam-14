using UnityEngine;

public class GizmoFOV : MonoBehaviour
{
    public float Range = 7f;
    public float FOV = 70f;
    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, Range);
        Vector3 left = Quaternion.Euler(0, 0, -FOV * 0.5f) * Vector3.right * Range;
        Vector3 right = Quaternion.Euler(0, 0, FOV * 0.5f) * Vector3.right * Range;
        Gizmos.DrawLine(Vector3.zero, left);
        Gizmos.DrawLine(Vector3.zero, right);
    }
}
