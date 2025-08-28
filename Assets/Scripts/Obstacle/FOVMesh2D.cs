using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FOVMesh2D : MonoBehaviour
{
    [Header("Source / Orientation")]
    public Transform pivot;              
    [Header("Shape")]
    [Range(0, 360)] public float FOV = 70f;
    public float ViewRadius = 7f;
    [Range(3, 180)] public int RayCount = 50;
    [Header("Occlusion")]
    public LayerMask Occluders;
    public bool ClipWithOccluders = true;
    [Header("Perf")]
    public float RebuildInterval = 0.05f;

    Mesh _mesh;
    MeshRenderer _renderer;
    IDetector _detector;
    float _t;

    void Awake()
    {
        _mesh = new Mesh { name = "FOV Mesh" };
        GetComponent<MeshFilter>().sharedMesh = _mesh;
        _renderer = GetComponent<MeshRenderer>();
        _detector = GetComponentInParent<IDetector>();
        if (pivot == null) pivot = transform.parent != null ? transform.parent : transform;
        transform.localPosition = new Vector3(0,0,-1);
        transform.localRotation = Quaternion.identity;
    }

    void LateUpdate()
    {
        _t += Time.deltaTime;
        if (_t < RebuildInterval) return;
        _t = 0f;
        RebuildMesh();
    }

    void RebuildMesh()
    {
        if (pivot == null) return;

        Vector3 origin = pivot.position;
        int count = Mathf.Max(3, RayCount);
        float half = FOV * 0.5f;
        float start = (FOV >= 359f) ? -180f : -half;
        float step = FOV / (count - 1);

        Vector3[] verts = new Vector3[count + 1];
        int[] tris = new int[(count - 1) * 3];

        verts[0] = Vector3.zero; 

        for (int i = 0; i < count; i++)
        {
            float a = start + step * i;
            Vector3 dir = Quaternion.Euler(0, 0, a) * pivot.right;
            Vector3 end;
            if (ClipWithOccluders)
            {
                var hit = Physics2D.Raycast(origin, dir, ViewRadius, Occluders);
                end = hit.collider ? (Vector3)hit.point : origin + dir * ViewRadius;
            }
            else
            {
                end = origin + dir * ViewRadius;
            }
            verts[i + 1] = transform.InverseTransformPoint(end);
        }

        int t = 0;
        for (int i = 0; i < count - 1; i++)
        {
            tris[t++] = 0;
            tris[t++] = i + 1;
            tris[t++] = i + 2;
        }

        _mesh.Clear();
        _mesh.vertices = verts;
        _mesh.triangles = tris;
        _mesh.RecalculateBounds();
        if (_detector != null && _detector.CanSee(FindFirstObjectByType<PlayerDetectable>()))
        {
            _renderer.material.color = Color.red;
        }
        else
        {
            _renderer.material.color = Color.green;
        }
    }
}
