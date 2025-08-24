using UnityEngine;

public class ObstaclesManagers : MonoBehaviour
{
    public static ObstaclesManagers Instance;
    public bool GlobalAlarm;

    void Awake() => Instance = this;

    public void OnDetection(IDetector src, IDetectable target, float strength = 1f)
    {
        if (GlobalAlarm)
        {
            return;
        }
        GlobalAlarm = true;
        // TODO:SFX, UI, etc.
        Debug.Log($"ALARM! by {src.GetType().Name} on {target}");
    }

    public void ResetAlarm() => GlobalAlarm = false;


}
