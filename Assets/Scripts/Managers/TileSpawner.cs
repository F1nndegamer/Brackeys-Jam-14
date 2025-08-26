using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TileSpawner : MonoBehaviour
{
    public GameObject tilePrefab;
    public float tileSize = 1f;

    void Start()
    {
        RoomHold hold = GetComponent<RoomHold>();
        Vector2 roomSize = hold.size;
        Instantiate(tilePrefab, transform.position, Quaternion.identity).GetComponent<SpriteRenderer>().size = roomSize;
    }
}