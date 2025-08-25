using UnityEngine;

public class GizmoNoise : MonoBehaviour
{
    public float Radius;
    public bool isRunning;
    void OnDrawGizmosSelected()
    {
        float r = isRunning ? Radius : Radius * 0.6f;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, r);
    }
}
