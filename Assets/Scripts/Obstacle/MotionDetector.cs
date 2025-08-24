using UnityEngine;

public class MotionDetector : MonoBehaviour, IDetector
{
    public float DetectionRange = 5f;
    public float Cooldown = 2f;
    float _lock;

    float IDetector.DetectionRange => DetectionRange;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time < _lock) return;
        if (other.TryGetComponent<IDetectable>(out var det) && !det.IsHidden)
        {
            RaiseAlarm(det);
            _lock = Time.time + Cooldown;
        }
    }

    public bool CanSee(IDetectable t) => true;
    public void RaiseAlarm(IDetectable tgt) => ObstaclesManagers.Instance.OnDetection(this, tgt, 1f);
}
