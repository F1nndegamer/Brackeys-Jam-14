using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonSounds : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioClip hoverSound;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => PlayClickSound(button));
            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = button.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener((eventData) => PlayHoverSound());
            trigger.triggers.Add(entry);
        }
    }

    void PlayClickSound(Button button)
    {
        audioSource.PlayOneShot(clickSound);
    }

    void PlayHoverSound()
    {
        audioSource.PlayOneShot(hoverSound);
    }
}
