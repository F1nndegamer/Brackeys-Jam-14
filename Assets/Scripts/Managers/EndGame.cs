using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class EndGame : MonoBehaviour
{
    Image im;
    [SerializeField] private float alphaMultiplier = 0.5f;
    bool isFading;
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
    }
}
