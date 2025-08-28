using UnityEngine;

public class shopManager : MonoBehaviour
{
    public static shopManager Instance;
    public GameObject shopCanvas;
    void Awake()
    {
        Instance = this;
    }
}
