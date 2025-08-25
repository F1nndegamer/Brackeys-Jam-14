using UnityEngine;

public class SecurityCamera : MonoBehaviour, IDetector
{
    public float DetectionRange = 8f;
    public float StartDetectionRange = 8f;
    public float RotationSpeed = 45f;
    public float FOV = 60f;
    public LayerMask Occluders;
    public Transform pivot;

    float IDetector.DetectionRange => DetectionRange;

    void Update()
    {
        DetectionRange = StartDetectionRange * Player.Instance.RangeMultiplier;
        pivot.Rotate(0, 0, RotationSpeed * Time.deltaTime);
        var player = FindFirstObjectByType<PlayerDetectable>();
        if (player == null || player.IsHidden)
        {
            return;
        }
        if (Vision2D.IsInFOV(pivot, player.GetPosition(), FOV, DetectionRange) &&
            Vision2D.HasLineOfSight(pivot.position, player.GetPosition(), Occluders))
        {
            RaiseAlarm(player);
        }
    }

    public bool CanSee(IDetectable tgt)
    {
        return Vision2D.IsInFOV(pivot, tgt.GetPosition(), FOV, DetectionRange) &&
               Vision2D.HasLineOfSight(pivot.position, tgt.GetPosition(), Occluders);
    }

    public void RaiseAlarm(IDetectable tgt)
    {
        ObstaclesManagers.Instance.OnDetection(this, tgt, 1f);
    }
}