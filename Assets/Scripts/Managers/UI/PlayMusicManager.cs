using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Clips")]
    public AudioClip normalMusic;
    public AudioClip shopMusic;
    public AudioClip menuMusic;

    [Header("UI")]
    public Slider volumeSlider;

    private AudioSource audioSource;
    private const string PLAYER_PREFS_SOUND_VOLUME = "SoundEffectsVolume";
    private Coroutine fadeCoroutine;
    private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gameObject.transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_VOLUME, 1f);
            volumeSlider.onValueChanged.AddListener(UpdateVolume);
        }

        audioSource.volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_VOLUME, 1f);
    }

    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Menu")
        {
            PlayMusic(GameState.Menu);
        }
    }

    public void PlayMusic(GameState state)
    {
        AudioClip clipToPlay = null;

        switch (state)
        {
            case GameState.Normal:
                clipToPlay = normalMusic;
                break;
            case GameState.Shop:
                clipToPlay = shopMusic;
                break;
            case GameState.Menu:
                clipToPlay = menuMusic;
                break;
        }

        if (clipToPlay != null && audioSource.clip != clipToPlay)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeMusic(clipToPlay, fadeDuration));
        }
    }

    private IEnumerator FadeMusic(AudioClip newClip, float duration)
    {
        float startVolume = audioSource.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.clip = newClip;
        audioSource.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, startVolume, t / duration);
            yield return null;
        }
        audioSource.volume = startVolume;
    }

    public void UpdateVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_VOLUME, volume);
    }

    public void StopMusic()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        audioSource.Stop();
    }
}

public enum GameState
{
    Normal,
    Shop,
    Menu
}
