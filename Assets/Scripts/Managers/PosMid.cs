using UnityEngine;

public class PosMid : MonoBehaviour
{
    [HideInInspector]public Vector3 position;

    void Awake()
    {
        position = transform.position;
    }

}
