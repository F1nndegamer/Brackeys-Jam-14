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

    // Public property to check if currently detecting (for your custom visuals)
    public bool IsDetecting => _isDetecting;

    void Update()
    {
        UpdateDetection();
    }

    public void RaiseAlarm(IDetectable target)
    {
        // Follow the same pattern as your other detectors
        ObstaclesManagers.Instance.OnDetection(this, target, 1f);
    }

    void UpdateDetection()
    {
        // Check if we should stop detecting (timeout)
        if (_isDetecting && Time.time - _lastDetectionTime > DetectionDuration)
        {
            _isDetecting = false;
        }

        // Get player (following the same pattern as your other detectors)
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

        // Must be within range
        if (distance > DetectionRange)
        {
            _currentTarget = null;
            _hasLastPosition = false;
            return;
        }

        // Check for occlusion in the direction of the player
        if (!Vision2D.HasLineOfSight(origin, playerPos, Occluders))
        {
            _currentTarget = null;
            _hasLastPosition = false;
            return;
        }

        // Player is within range and visible, check for motion
        CheckForMotion(player);
    }

    void CheckForMotion(IDetectable target)
    {
        Vector2 currentPosition = target.GetPosition();

        // If this is a new target or we don't have a last position, just record it
        if (_currentTarget != target || !_hasLastPosition)
        {
            _currentTarget = target;
            _lastTargetPosition = currentPosition;
            _hasLastPosition = true;
            return; // Don't trigger on first detection
        }

        // Check cooldown first
        if (Time.time - _lastDetectionTime < DetectionCooldown)
        {
            return; // Still in cooldown, don't check for motion
        }

        // Calculate movement
        float distanceMoved = Vector2.Distance(_lastTargetPosition, currentPosition);

        // Only trigger if significant movement detected
        if (distanceMoved > MovementThreshold)
        {
            _isDetecting = true;
            _lastDetectionTime = Time.time;
            Debug.Log($"Motion detected! Distance moved: {distanceMoved}, Threshold: {MovementThreshold}");
            RaiseAlarm(target);
        }

        // Always update last known position
        _lastTargetPosition = currentPosition;
    }


    public bool CanSee(IDetectable target)
    {
        if (target == null || target.IsHidden) return false;

        Vector2 origin = transform.position;
        Vector2 targetPos = target.GetPosition();
        float distance = Vector2.Distance(origin, targetPos);

        if (distance > DetectionRange) return false;

        // Check line of sight in any direction (360 degrees)
        return Vision2D.HasLineOfSight(origin, targetPos, Occluders);
    }

    // Optional: Gizmos for debugging in Scene view
    void OnDrawGizmos()
    {
        Color gizmoColor = _isDetecting ? Color.red : Color.green;
        Gizmos.color = gizmoColor;
        Vector3 origin = transform.position;

        // Draw circular detection area
        Gizmos.DrawSphere(origin, DetectionRange);
    }
}