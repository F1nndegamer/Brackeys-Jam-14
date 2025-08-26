using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public GameObject tilePrefab;
    public float tileSize = 1f;

    void Start()
    {
        RoomHold hold = GetComponent<RoomHold>();
        Vector2 roomSize = hold.size;
        GameObject tile = Instantiate(tilePrefab, transform.position, Quaternion.identity, transform);
        SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
        sr.size = roomSize;
        Vector3 parentScale = transform.lossyScale;
        tile.transform.localScale = new Vector3(
            1f / parentScale.x,
            1f / parentScale.y,
            1f / parentScale.z
        );
    }
}
