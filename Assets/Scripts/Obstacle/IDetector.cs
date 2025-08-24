using UnityEngine;

public interface IDetector
{
    float DetectionRange { get; }
    bool CanSee(IDetectable tgt);
    void RaiseAlarm(IDetectable tgt);
}
