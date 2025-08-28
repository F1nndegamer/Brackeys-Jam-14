using UnityEngine;

public class SecurityGuard : MonoBehaviour, IDetector
{
    public float DetectionRange = 7f;
    public float StartDetectionRange = 7f;
    public float FOV = 70f;
    public float Speed = 2.0f;
    public Transform[] Waypoints;
    public LayerMask Occluders;

    enum State { Patrol, Investigate, Chase, Return }
    State _state;
    int _wp;
    Vector2 _lastKnown;
    PathMover2D _mover;

    void Awake()
    {
        _mover = GetComponent<PathMover2D>();
        if (_mover == null) _mover = gameObject.AddComponent<PathMover2D>();
        _mover.Occluders = Occluders; 
    }
    float IDetector.DetectionRange => DetectionRange;

    void Update()
    {
        DetectionRange = StartDetectionRange * Player.Instance.RangeMultiplier;
        var player = FindFirstObjectByType<PlayerDetectable>();
        if (player != null && !player.IsHidden)
        {
            bool sees = Vision2D.IsInFOV(transform, player.GetPosition(), FOV, DetectionRange) &&
                        Vision2D.HasLineOfSight(transform.position, player.GetPosition(), Occluders);
            if (sees) { _lastKnown = player.GetPosition(); _state = State.Chase; }
        }

        switch (_state)
        {
            case State.Patrol: MoveTo(Waypoints[_wp].position, () => { _wp = (_wp + 1) % Waypoints.Length; }); break;
            case State.Investigate: MoveTo(_lastKnown, () => _state = State.Return); break;
            case State.Chase:
                if (player == null) { _state = State.Return; break; }
                _lastKnown = player.GetPosition();
                MoveTo(_lastKnown, () => RaiseAlarm(player));
                if (!WithinSight(player)) _state = State.Investigate;
                break;
            case State.Return: MoveTo(Waypoints[_wp].position, () => _state = State.Patrol); break;
        }
    }

    bool WithinSight(IDetectable t) =>
        Vision2D.IsInFOV(transform, t.GetPosition(), FOV, DetectionRange) &&
        Vision2D.HasLineOfSight(transform.position, t.GetPosition(), Occluders);

    void MoveTo(Vector2 target, System.Action onReach)
    {
        _mover.MoveTo(target, Speed);
        if (_mover.Reached(target)) onReach?.Invoke();
    }
    public void SetWaypoint(Transform[] selected)
    {
        Waypoints = selected ?? System.Array.Empty<Transform>();
    }
    public void OnSuspiciousNoise(Vector2 where)
    {
        _lastKnown = where;
        if (_state == State.Patrol) _state = State.Investigate;
    }
    public bool CanSee(IDetectable t) => WithinSight(t);
    public void RaiseAlarm(IDetectable tgt) => ObstaclesManagers.Instance.OnDetection(this, tgt, 1f);
}
