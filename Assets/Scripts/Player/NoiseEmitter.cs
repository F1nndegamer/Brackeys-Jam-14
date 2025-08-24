using UnityEngine;

public class NoiseEmitter : MonoBehaviour
{
    public float WalkDb = 0.5f;
    public float RunDb = 1.0f;
    public float Radius = 5f;

    public void Emit(bool running)
    {
        float r = running ? Radius : Radius * 0.6f;
        foreach (var dog in FindObjectsByType<Dog>(FindObjectsSortMode.None))
        {
            if (Vector2.Distance(transform.position, dog.transform.position) <= r)
                dog.HearNoise(transform.position);
        }
        // istersen Guards için Investigate tetikleyebilirsin
        foreach (var guard in FindObjectsByType<SecurityGuard>(FindObjectsSortMode.None))
        {
            if (Vector2.Distance(transform.position, guard.transform.position) <= r)
                guard.SendMessage("OnSuspiciousNoise", transform.position, SendMessageOptions.DontRequireReceiver);
        }
    }
}