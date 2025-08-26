using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
public class RoomSpawner : MonoBehaviour
{
    public GameObject[] Rooms;
    public GameObject Parent;
    public int roomCount = 5;
    public int gridSize = 10;

    void Awake()
    {
        Rooms = Resources.LoadAll<GameObject>("Chamber");
    }

    public void SpawnRooms()
    {
        foreach (Transform child in Parent.transform)
            Destroy(child.gameObject);

        List<Vector2Int> occupied = new List<Vector2Int>();
        Vector2Int start = new Vector2Int(0, 0);
        occupied.Add(start);

        for (int i = 1; i < roomCount; i++)
        {
            Vector2Int newPos;
            int attempts = 0;

            do
            {
                Vector2Int dir = new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2));
                newPos = occupied[Random.Range(0, occupied.Count)] + dir;
                attempts++;
            } while (occupied.Contains(newPos) && attempts < 100);

            occupied.Add(newPos);
        }

        foreach (Vector2Int gridPos in occupied)
        {
            Vector3 worldPos = new Vector3(gridPos.x * gridSize, 0, gridPos.y * gridSize);
            GameObject chosenRoom = Rooms[Random.Range(0, Rooms.Length)];
            Instantiate(chosenRoom, worldPos, Quaternion.identity, Parent.transform);
        }
    }
}
