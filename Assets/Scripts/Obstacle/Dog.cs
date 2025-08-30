using System.Linq;
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
    float _timer = 0;

    float IDetector.DetectionRange => DetectionRange;

    private void Awake()
    {
        _mover = GetComponent<PathMover2D>();
        if (_mover == null) _mover = gameObject.AddComponent<PathMover2D>();
        _mover.Occluders = Occluders;
    }

    void Update()
    {
        if (Waypoints == null || Waypoints.Length == 0)
        {
            Destroy(gameObject);
            return;
        }

        _timer += Time.deltaTime;

        var player = FindFirstObjectByType<PlayerDetectable>();

        if (player != null && !player.Equals(null) && !player.IsHidden)
        {
            bool canSee = Vision2D.HasLineOfSight(transform.position, player.GetPosition(), Occluders) &&
                          Vector2.Distance(transform.position, player.GetPosition()) <= DetectionRange;

            if (canSee) { _state = State.Chase; }
        }


        switch (_state)
        {
            case State.Patrol:
                if (ValidWaypoint())
                {
                    MoveTo(Waypoints[_wp].position, () => { _wp = (_wp + 1) % Waypoints.Length; });
                }

                if (_timer >= 1f)
                {
                    SoundManager.Instance?.PlayIdleSoundDog(transform.position);
                    _timer = 0;
                }
                break;

            case State.Investigate:
                MoveTo(_lastNoise, () => _state = State.Patrol);

                if (_timer >= 1f)
                {
                    SoundManager.Instance?.PlayChaseSoundDog(transform.position);
                    _timer = 0;
                }
                break;

            case State.Chase:
                if (player == null || player.Equals(null))
                {
                    _state = State.Return;
                    break;
                }

                MoveTo(player.GetPosition(), () => RaiseAlarm(player));

                if (_timer >= 1f)
                {
                    SoundManager.Instance?.PlayChaseSoundDog(transform.position);
                    _timer = 0f;
                }

                if (Vector2.Distance(transform.position, player.GetPosition()) > DetectionRange * 1.2f)
                {
                    _state = State.Return;
                }
                break;

            case State.Return:
                if (ValidWaypoint())
                {
                    MoveTo(Waypoints[_wp].position, () => _state = State.Patrol);
                }

                if (_timer >= 1f)
                {
                    SoundManager.Instance?.PlayIdleSoundDog(transform.position);
                    _timer = 0f;
                }
                break;
        }
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

    public void HearNoise(Vector2 where)
    {
        _lastNoise = where;
        if (_state == State.Patrol) _state = State.Investigate;
    }

    public bool CanSee(IDetectable t)
    {
        if (t == null || t.Equals(null)) return false;
        return Vector2.Distance(transform.position, t.GetPosition()) <= DetectionRange;
    }

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
