using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipRefsSO", menuName = "Scriptable Objects/AudioClipRefsSO")]
public class AudioClipRefsSO : ScriptableObject
{
    public AudioClip[] footstepWalk;
    public AudioClip[] footstepCrouch;
}
