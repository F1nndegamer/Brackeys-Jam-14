using UnityEngine;
using UnityEngine.UIElements;

public class RoomSpawner : MonoBehaviour
{
    [Header("Room Prefabs")]
    public GameObject[] Rooms;
    public GameObject Parent;
    void Awake()
    {
        Rooms = Resources.LoadAll<GameObject>("Chamber");
        Debug.Log("Rooms loaded: " + Rooms.Length);
    }

    public void SpawnRoom()
    {
        foreach(Transform child in Parent.transform)
        {
            Destroy(child.gameObject);
        }

        PosMid[] posMids = FindObjectsByType<PosMid>(FindObjectsSortMode.InstanceID);
        for (int i = 0; i < posMids.Length; i++)
        {
            if (posMids[i] == null)
            {
                Debug.LogError("No PosMid component found!");
                return;
            }

            GameObject[] possibleRooms = Rooms;
            GameObject parent = Parent;
            
            if (possibleRooms == null || possibleRooms.Length == 0)
            {
                Debug.LogWarning("No prefabs assigned for this position.");
                return;
            }

            GameObject chosenRoom = possibleRooms[Random.Range(0, possibleRooms.Length)];

            Instantiate(chosenRoom, posMids[i].position, Quaternion.identity, parent.transform);
        }
    }
}
