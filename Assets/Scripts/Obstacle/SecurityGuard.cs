using System.Threading;
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
    float _timer = 0;

    void Awake()
    {
        _mover = GetComponent<PathMover2D>();
        if (_mover == null) _mover = gameObject.AddComponent<PathMover2D>();
        _mover.Occluders = Occluders;
    }

    float IDetector.DetectionRange => DetectionRange;

    void Update()
    {
        if (Waypoints == null || Waypoints.Length == 0)
        {
            Destroy(gameObject);
            return;
        }

        _timer += Time.deltaTime;
        DetectionRange = StartDetectionRange * (Player.Instance != null ? Player.Instance.RangeMultiplier : 1f);

        var player = FindFirstObjectByType<PlayerDetectable>();

        if (player != null && !player.Equals(null) && !player.IsHidden)
        {
            bool sees = Vision2D.IsInFOV(transform, player.GetPosition(), FOV, DetectionRange) &&
                        Vision2D.HasLineOfSight(transform.position, player.GetPosition(), Occluders);
            if (sees)
            {
                _lastKnown = player.GetPosition();
                _state = State.Chase;
            }
        }

        float volume = 1f;

        switch (_state)
        {
            case State.Patrol:
                if (ValidWaypoint())
                {
                    MoveTo(Waypoints[_wp].position, () => { _wp = (_wp + 1) % Waypoints.Length; });
                }
                if (_timer >= 0.7f)
                {
                    SoundManager.Instance?.PlayIdleSoundGuard(transform.position, volume);
                }
                break;

            case State.Investigate:
                MoveTo(_lastKnown, () => _state = State.Return);
                break;

            case State.Chase:
                if (player == null || player.Equals(null))
                {
                    _state = State.Return;
                    break;
                }
                if (_timer >= 0.7f)
                {
                    SoundManager.Instance?.PlayIdleSoundGuard(transform.position, volume);
                }
                _lastKnown = player.GetPosition();
                MoveTo(_lastKnown, () => RaiseAlarm(player));

                if (_timer >= 0.7f)
                {
                    SoundManager.Instance?.PlayChaseSoundGuard(transform.position, volume);
                    _timer = 0f;
                }

                if (!WithinSight(player)) _state = State.Investigate;
                break;

            case State.Return:
                if (ValidWaypoint())
                {
                    MoveTo(Waypoints[_wp].position, () => _state = State.Patrol);
                    SoundManager.Instance?.PlayIdleSoundGuard(transform.position, volume);
                }
                break;
        }

        // idle sounds on a timer
        if (_timer >= 0.7f)
        {
            SoundManager.Instance?.PlayIdleSoundGuard(transform.position, volume);
            _timer = 0;
        }
    }

    bool WithinSight(IDetectable t)
    {
        if (t == null || t.Equals(null)) return false;
        return Vision2D.IsInFOV(transform, t.GetPosition(), FOV, DetectionRange) &&
               Vision2D.HasLineOfSight(transform.position, t.GetPosition(), Occluders);
    }

    void MoveTo(Vector2 target, System.Action onReach)
    {
        if (_mover == null || _mover.Equals(null)) return;
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

    public void RaiseAlarm(IDetectable tgt)
    {
        if (tgt == null || tgt.Equals(null)) return;
        ObstaclesManagers.Instance?.OnDetection(this, tgt, 1f);
    }

    bool ValidWaypoint()
    {
        return _wp < Waypoints.Length && Waypoints[_wp] != null && !Waypoints[_wp].Equals(null);
    }
}
