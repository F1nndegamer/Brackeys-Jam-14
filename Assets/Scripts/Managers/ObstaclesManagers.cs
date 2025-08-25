using System.Collections;
using UnityEngine;

public class ObstaclesManagers : MonoBehaviour
{
    public static ObstaclesManagers Instance;
    public bool GlobalAlarm;

    void Awake() => Instance = this;

    public void OnDetection(IDetector src, IDetectable target, float strength = 1f)
    {
        if (GlobalAlarm) return;

        GlobalAlarm = true;
        Debug.Log($"ALARM! by {src.GetType().Name} on {target}");
        Player.Instance.OnDetected();

        StartCoroutine(AlarmCooldown());
    }

    private IEnumerator AlarmCooldown()
    {
        yield return new WaitForSeconds(5f);
        GlobalAlarm = false;
        Debug.Log("Alarm reset, can trigger again.");
    }

    public void ResetAlarm()
    {
        StopAllCoroutines();
        GlobalAlarm = false;
    }
}
