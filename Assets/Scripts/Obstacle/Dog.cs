using UnityEngine;

public class Dog : MonoBehaviour, IDetector
{
    public float DetectionRange = 6f;
    public float Speed = 2.2f;
    public Transform[] Waypoints;
    public LayerMask Occluders;

    enum State { Patrol, Investigate, Chase, Return }
    State _state;
    int _wp;
    Vector2 _lastNoise;
    PathMover2D _mover;

    float IDetector.DetectionRange => DetectionRange;
    private void Awake()
    {
        _mover = GetComponent<PathMover2D>();
        if (_mover == null) _mover = gameObject.AddComponent<PathMover2D>();
        _mover.Occluders = Occluders;
    }

    void Update()
    {
        var player = FindFirstObjectByType<PlayerDetectable>();
        if (player != null && !player.IsHidden)
        {
            bool canSee = Vision2D.HasLineOfSight(transform.position, player.GetPosition(), Occluders) &&
                          Vector2.Distance(transform.position, player.GetPosition()) <= DetectionRange;
            if (canSee) { _state = State.Chase; }
        }
        switch (_state)
        {
            case State.Patrol: MoveTo(Waypoints[_wp].position, () => { _wp = (_wp + 1) % Waypoints.Length; }); break;
            case State.Investigate: MoveTo(_lastNoise, () => _state = State.Patrol); break;
            case State.Chase:
                if (player == null) { _state = State.Return; break; }
                MoveTo(player.GetPosition(), () => RaiseAlarm(player));
                if (Vector2.Distance(transform.position, player.GetPosition()) > DetectionRange * 1.2f)
                    _state = State.Return;
                break;
            case State.Return: MoveTo(Waypoints[_wp].position, () => _state = State.Patrol); break;
        }
    }

    void MoveTo(Vector2 target, System.Action onReach)
    {
        _mover.MoveTo(target, Speed);
        if (_mover.Reached(target)) onReach?.Invoke();
    }
    public void SetWaypoint(Transform[] selected)
    {
        Waypoints = selected ?? System.Array.Empty<Transform>();
    }
    public void HearNoise(Vector2 where)
    {
        _lastNoise = where;
        if (_state == State.Patrol) _state = State.Investigate;
    }

    public bool CanSee(IDetectable t) => Vector2.Distance(transform.position, t.GetPosition()) <= DetectionRange;
    public void RaiseAlarm(IDetectable tgt) => ObstaclesManagers.Instance.OnDetection(this, tgt, 1f);
}
