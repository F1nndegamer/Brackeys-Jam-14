using UnityEngine;

public class PosMid : MonoBehaviour
{
    public enum Positionindex
    {
        Top,
        Middle,
        Bottom
    }
    public Positionindex positionIndex;
    [HideInInspector]public Vector3 position;

    void Awake()
    {
        position = transform.position;
    }

}
