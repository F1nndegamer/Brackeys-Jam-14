using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public GameObject tilePrefab;
    public float tileSize = 1f;

    void Start()
    {
        RoomHold hold = GetComponent<RoomHold>();
        Vector2 roomSize = hold.size;

        int tilesX = Mathf.RoundToInt(roomSize.x / tileSize);
        int tilesY = Mathf.RoundToInt(roomSize.y / tileSize);

        for (int x = 0; x < tilesX; x++)
        {
            for (int y = 0; y < tilesY; y++)
            {
                Vector3 pos = new Vector3(x * tileSize, y * tileSize, 0);
                Instantiate(tilePrefab, pos + transform.position, Quaternion.identity, transform);
            }
        }
    }
}