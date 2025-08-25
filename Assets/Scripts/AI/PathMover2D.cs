using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Pathfinder2D))]
public class PathMover2D : MonoBehaviour
{
    public float RepathInterval = 0.4f;
    public float ReachThreshold = 0.1f;
    public LayerMask Occluders;
    public float RotationSpeed = 120f;
    public bool UseSmoothDamp = false;
    public float WallBuffer = 0.20f;
    public float SkinWidth = 0.02f;

    float _rotVel;                       
    Pathfinder2D pathfinder;
    List<Vector2> path;
    int index;
    Vector2 lastTarget;
    float lastRepath;
    float speed;

    void Awake() => pathfinder = FindFirstObjectByType<Pathfinder2D>();

    public void MoveTo(Vector2 target, float moveSpeed)
    {
        speed = moveSpeed;
        bool needRepath = path == null || (target - lastTarget).sqrMagnitude > 0.01f || (Time.time - lastRepath) >= RepathInterval;
        lastTarget = target;


        if (needRepath)
        {
            if (HasClearPathBuffered(transform.position, target))
            {
                path = new List<Vector2> { target };
                index = 0;
            }
            else if (pathfinder != null && pathfinder.TryFindPath(transform.position, target, out var newPath))
            {
                path = newPath; index = 0;
            }
            else
            {
                path = null; // fallback: direkt hareket edecek
            }
            lastRepath = Time.time;
        }

        if (path == null || index >= path.Count) { StepTowards(target); return; }

        Vector2 wp = path[index];
        if (Vector2.Distance(transform.position, wp) <= ReachThreshold)
        {
            index++;
            if (index >= path.Count) { StepTowards(target); return; }
            wp = path[index];
        }

        StepTowards(wp);
    }

    public bool Reached(Vector2 p) => Vector2.Distance(transform.position, p) <= ReachThreshold;

    void StepTowards(Vector2 p)
    {
        Vector2 pos = transform.position;
        Vector2 to = p - pos;
        if (to.sqrMagnitude < 1e-6f) return;

        float stepLen = Mathf.Min(speed * Time.deltaTime, to.magnitude);
        Vector2 desired = to.normalized * stepLen;
        Vector2 move = desired;

        // 1) Ýleri adýmý buffered CircleCast ile kontrol et
        var hit = Physics2D.CircleCast(pos, WallBuffer, desired.normalized, stepLen + SkinWidth, Occluders);
        if (hit.collider != null)
        {
            // çarpmadan hemen önce dur
            float allow = Mathf.Max(0f, hit.distance - SkinWidth);
            Vector2 first = desired.normalized * allow;

            // 2) Kalaný yüzeye paralel kaydýr (tanjante projeksiyon)
            Vector2 remaining = desired - first;
            Vector2 tangent = Vector2.Perpendicular(hit.normal).normalized;
            float proj = Vector2.Dot(remaining, tangent);
            Vector2 slide = tangent * proj;

            // slide yönünü de buffered kontrol et
            if (slide.sqrMagnitude > 1e-8f)
            {
                var hit2 = Physics2D.CircleCast(pos + first, WallBuffer, slide.normalized, slide.magnitude + SkinWidth, Occluders);
                if (hit2.collider != null)
                {
                    float allowSlide = Mathf.Max(0f, hit2.distance - SkinWidth);
                    slide = slide.normalized * allowSlide;
                }
            }
            move = first + slide;
        }

        transform.position = pos + move;

        // yumuþak dönüþ (önceki ayarýnla ayný mantýk)
        if (move.sqrMagnitude > 1e-8f)
        {
            float desiredAng = Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg; // sprite +X'i görüyor varsayýmý
            float current = transform.eulerAngles.z;
            float next = UseSmoothDamp
                ? Mathf.SmoothDampAngle(current, desiredAng, ref _rotVel, 0.15f)
                : Mathf.MoveTowardsAngle(current, desiredAng, RotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, next);
        }
    }
    bool HasClearPathBuffered(Vector2 from, Vector2 to)
    {
        Vector2 dir = to - from; float dist = dir.magnitude;
        if (dist < 1e-4f) return true;
        return !Physics2D.CircleCast(from, WallBuffer, dir.normalized, dist, Occluders);
    }
}
