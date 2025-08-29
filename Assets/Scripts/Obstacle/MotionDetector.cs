using UnityEngine;

public class MotionDetector : MonoBehaviour, IDetector
{
    [Header("Detection Settings")]
    public float DetectionRange = 10f;
    public float StartDetectionRange = 10f;
    public LayerMask Occluders;

    [Header("Motion Detection")]
    public float MovementThreshold = 0.1f;
    public float DetectionDuration = 1f;
    public float DetectionCooldown = 0.5f;

    private IDetectable _currentTarget;
    private Vector2 _lastTargetPosition;
    private bool _hasLastPosition;
    private float _lastDetectionTime;
    private bool _isDetecting;

    float IDetector.DetectionRange => DetectionRange;

    public bool IsDetecting => _isDetecting;

    void Update()
    {
        UpdateDetection();
    }

    public void RaiseAlarm(IDetectable target)
    {
        ObstaclesManagers.Instance.OnDetection(this, target, 1f);
    }

    void UpdateDetection()
    {
        DetectionRange = StartDetectionRange * Player.Instance.RangeMultiplier;
        if (_isDetecting && Time.time - _lastDetectionTime > DetectionDuration)
        {
            _isDetecting = false;
        }

        var player = FindFirstObjectByType<PlayerDetectable>();
        if (player == null || player.IsHidden)
        {
            _currentTarget = null;
            _hasLastPosition = false;
            return;
        }

        Vector2 origin = transform.position;
        Vector2 playerPos = player.GetPosition();
        float distance = Vector2.Distance(origin, playerPos);

        if (distance > DetectionRange)
        {
            _currentTarget = null;
            _hasLastPosition = false;
            return;
        }

        if (!Vision2D.HasLineOfSight(origin, playerPos, Occluders))
        {
            _currentTarget = null;
            _hasLastPosition = false;
            return;
        }

        CheckForMotion(player);
    }

    void CheckForMotion(IDetectable target)
    {
        Vector2 currentPosition = target.GetPosition();

        if (_currentTarget != target || !_hasLastPosition)
        {
            _currentTarget = target;
            _lastTargetPosition = currentPosition;
            _hasLastPosition = true;
            return;
        }

        if (Time.time - _lastDetectionTime < DetectionCooldown)
        {
            return; // Still in cooldown, don't check for motion
        }

        float distanceMoved = Vector2.Distance(_lastTargetPosition, currentPosition);

        if (distanceMoved > MovementThreshold)
        {
            _isDetecting = true;
            _lastDetectionTime = Time.time;
            Debug.Log($"Motion detected! Distance moved: {distanceMoved}, Threshold: {MovementThreshold}");
            RaiseAlarm(target);
        }

        _lastTargetPosition = currentPosition;
    }


    public bool CanSee(IDetectable target) => _isDetecting;

    void OnDrawGizmos()
    {
        Color gizmoColor = _isDetecting ? Color.red : Color.green;
        Gizmos.color = gizmoColor;
        Vector3 origin = transform.position;

        Gizmos.DrawSphere(origin, DetectionRange);
    }
}