using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class EndGame : MonoBehaviour
{
    Image im;
    [SerializeField] private float alphaMultiplier = 0.5f;
    bool isFading;
    bool isUnfading;
    void Awake()
    {
        im = GetComponent<Image>();
    }
    void Start()
    {
        im.color = new Color(255, 255, 255, 0);
    }
    public void Fade()
    {
        isFading = true;
        StartCoroutine(FadeCoroutine());
    }
    public void Unfade()
    {
        isUnfading = true;
        StartCoroutine(UnfadeCoroutine());
    }
    public IEnumerator FadeCoroutine()
    {
        Debug.Log("Fading");
        Color c = im.color;
        while (im.color.a < 1)
        {
            c.a += alphaMultiplier * Time.deltaTime;
            im.color = c;
            yield return null;
        }
        isFading = false;
        shopManager.Instance.shopCanvas.SetActive(true);
    }
    public IEnumerator UnfadeCoroutine()
    {
        Debug.Log("Unfading");
        Color c = im.color;
        while (im.color.a > 0)
        {
            c.a -= alphaMultiplier * Time.deltaTime;
            im.color = c;
            yield return null;
        }
        isUnfading = false;
    }
}
