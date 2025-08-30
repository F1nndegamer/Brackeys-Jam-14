using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipForObstacles", menuName = "Scriptable Objects/AudioClipForObstacles")]
public class AudioClipForObstacles : ScriptableObject
{
    public AudioClip[] idle;
    public AudioClip[] chase;
}
