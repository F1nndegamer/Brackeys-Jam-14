using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
            }
            if (instance == null)
            {
                //instance = new GameObject(nameof(T)).AddComponent<T>();
            }
            return instance;
        }
    }
}
