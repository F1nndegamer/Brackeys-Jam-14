using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject SettingsMenu;
    void Start()
    {
        SettingsMenu.SetActive(false);
    }

    void Update()
    {

    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
