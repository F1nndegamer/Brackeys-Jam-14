using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class MenuMusicManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    public Slider volumeSlider;
    void Awake()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }
    public void UpdateVolume(float volume)
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.volume = volume;
    }
}
