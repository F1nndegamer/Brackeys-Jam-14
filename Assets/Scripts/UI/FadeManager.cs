using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class FadeManager : MonoBehaviour
{
    Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        FadeOut();
    }
    public void FadeIn()
    {
        anim.SetTrigger("FadeIn");
        StartCoroutine(LoadNextSceneAfterFadeIn());
    }
    private IEnumerator LoadNextSceneAfterFadeIn()
    {
        yield return new WaitForSeconds(1f); // wait for fade in to complete
        SceneManager.LoadScene("Player");
    }
    public void FadeOut()
    {
        anim.SetTrigger("FadeOut");
    }
}
