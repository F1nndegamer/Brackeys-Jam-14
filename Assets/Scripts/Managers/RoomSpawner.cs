using UnityEngine;
using UnityEngine.UIElements;

public class RoomSpawner : MonoBehaviour
{
    [Header("Room Prefabs")]
    public GameObject[] topRooms;
    public GameObject[] middleRooms;
    public GameObject[] bottomRooms;
    public GameObject TopParent;
    public GameObject MiddleParent;
    public GameObject BottomParent;
    void Awake()
    {
        topRooms = Resources.LoadAll<GameObject>("Chamber/Attic");
        middleRooms = Resources.LoadAll<GameObject>("Chamber/Loft");
        bottomRooms = Resources.LoadAll<GameObject>("Chamber/Upper");

        Debug.Log("Top rooms loaded: " + topRooms.Length);
        Debug.Log("Middle rooms loaded: " + middleRooms.Length);
        Debug.Log("Bottom rooms loaded: " + bottomRooms.Length);

    }

    public void SpawnRoom()
    {
        PosMid[] posMids = FindObjectsByType<PosMid>(FindObjectsSortMode.InstanceID);
        for (int i = 0; i < posMids.Length; i++)
        {
            if (posMids[i] == null)
            {
                Debug.LogError("No PosMid component found!");
                return;
            }

            GameObject[] possibleRooms = null;
            GameObject parent = null;
            switch (posMids[i].positionIndex)
            {
                case PosMid.Positionindex.Top:
                    possibleRooms = topRooms;
                    parent = TopParent;
                    break;
                case PosMid.Positionindex.Middle:
                    possibleRooms = middleRooms;
                    parent = MiddleParent;
                    break;
                case PosMid.Positionindex.Bottom:
                    possibleRooms = bottomRooms;
                    parent = BottomParent;
                    break;
            }

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
