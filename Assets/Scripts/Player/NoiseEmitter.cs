using UnityEngine;

public class NoiseEmitter : MonoBehaviour
{
    public float Radius = 5f;
    public float StartRadius = 5f;
    public bool isRunning;
    void Update()
    {
        Radius = StartRadius * Player.Instance.RangeMultiplier;
    }
    public void Emit(bool running)
    {
        float r = running ? Radius : Radius * 0.6f;
        foreach (var dog in FindObjectsByType<Dog>(FindObjectsSortMode.None))
        {
            if (Vector2.Distance(transform.position, dog.transform.position) <= r)
                dog.HearNoise(transform.position);
        }
        foreach (var guard in FindObjectsByType<SecurityGuard>(FindObjectsSortMode.None))
        {
            if (Vector2.Distance(transform.position, guard.transform.position) <= r)
                guard.OnSuspiciousNoise(transform.position);
        }
    }
}