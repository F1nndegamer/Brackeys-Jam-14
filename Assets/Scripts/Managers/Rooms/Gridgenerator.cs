using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class GridRoomGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 20;
    public int gridHeight = 20;
    public float cellSize = 1f;

    [Header("Room Settings")]
    public int minRoomSize = 3;
    public int maxRoomSize = 6;
    public int maxRooms = 6;
    public int placeAttempts = 200;

    [Header("Prefabs")]
    public GameObject wallPrefab;
    private int[,] roomMap;
    public GameObject[] Rooms;
    public int roomCount = 5;
    public int gridSize = 10;
    public int Difficulty = 1;
    public int collectibleCount = 10;
    private List<RoomInstance> rooms = new List<RoomInstance>();
    public List<GameObject> CollectiblesPrefabs;
    public GameObject BiscuitPrefab;
    public static GridRoomGenerator Instance;

    void Awake()
    {
        Instance = this;
        Rooms = Resources.LoadAll<GameObject>("Chamber");
    }

    void Start()
    {
        GenerateRooms();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateRooms();
        }
    }
    public void GenerateRooms()
    {
        ClearExisting();
        GenerateRoomsTouching();
        FillRoomMap();
        InstantiateFloors();
        InstantiateWallsWithDoors();
        InstantiateCollectibles();
    }
    void ClearExisting()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        rooms.Clear();
    }
    void GenerateRoomsTouching()
    {
        int attempts = 0;
        RoomInstance first = CreateRandomRoomInstance();
        rooms.Add(first);

        while (rooms.Count < maxRooms && attempts < placeAttempts)
        {
            attempts++;

            RoomInstance newRoomInstance = CreateRandomRoomInstance();
            RectInt newRoom = newRoomInstance.rect;
            bool placed = false;
            for (int tryAttach = 0; tryAttach < 30 && !placed; tryAttach++)
            {
                RoomInstance existingInstance = rooms[Random.Range(0, rooms.Count)];
                RectInt existing = existingInstance.rect;
                int side = Random.Range(0, 4);

                RectInt candidate = SetRoomTouching(existing, newRoom, side);
                if (candidate.xMin < 0 || candidate.yMin < 0 || candidate.xMax > gridWidth || candidate.yMax > gridHeight)
                    continue;

                if (!OverlapsAny(candidate))
                {
                    rooms.Add(new RoomInstance(candidate, newRoomInstance.prefab));
                    placed = true;
                }
            }

            if (!placed)
            {
                RectInt fallbackRect = CreateRandomRoomPosition();
                if (!OverlapsAny(fallbackRect))
                {
                    rooms.Add(new RoomInstance(fallbackRect, newRoomInstance.prefab));
                }
            }
        }
    }

    RoomInstance CreateRandomRoomInstance()
    {
        List<GameObject> possibleRooms = new List<GameObject>();
        foreach (GameObject room in Rooms)
        {
            var data = room.GetComponent<RoomHold>();
            if (data.Difficultymin <= Difficulty && data.Difficultymax >= Difficulty)
            {
                possibleRooms.Add(room);
            }
        }

        GameObject chosenRoom = possibleRooms[Random.Range(0, possibleRooms.Count)];
        var size = chosenRoom.GetComponent<RoomHold>().size;

        RectInt r = new RectInt(0, 0, size.x, size.y);
        return new RoomInstance(r, chosenRoom);
    }

    RectInt CreateRandomRoomPosition()
    {
        RectInt r = CreateRandomRoomInstance().rect;
        int x = Random.Range(0, Mathf.Max(1, gridWidth - r.width + 1));
        int y = Random.Range(0, Mathf.Max(1, gridHeight - r.height + 1));
        return new RectInt(x, y, r.width, r.height);
    }

    RectInt SetRoomTouching(RectInt existing, RectInt roomTemplate, int side)
    {
        RectInt r = roomTemplate;
        switch (side)
        {
            case 0:
                r.x = existing.xMax;
                r.y = Random.Range(existing.yMin - r.height + 1, existing.yMax);
                break;
            case 1:
                r.x = existing.xMin - r.width;
                r.y = Random.Range(existing.yMin - r.height + 1, existing.yMax);
                break;
            case 2:
                r.y = existing.yMax;
                r.x = Random.Range(existing.xMin - r.width + 1, existing.xMax);
                break;
            case 3:
                r.y = existing.yMin - r.height;
                r.x = Random.Range(existing.xMin - r.width + 1, existing.xMax);
                break;
        }

        r.x = Mathf.Clamp(r.x, 0, gridWidth - r.width);
        r.y = Mathf.Clamp(r.y, 0, gridHeight - r.height);
        return r;
    }

    bool OverlapsAny(RectInt candidate)
    {
        foreach (RoomInstance r in rooms)
        {
            if (candidate.Overlaps(r.rect))
                return true;
        }
        return false;
    }

    void FillRoomMap()
    {
        roomMap = new int[gridWidth, gridHeight];
        for (int i = 0; i < rooms.Count; i++)
        {
            RectInt r = rooms[i].rect;
            int id = i + 1;
            for (int x = r.xMin; x < r.xMax; x++)
            {
                for (int y = r.yMin; y < r.yMax; y++)
                {
                    roomMap[x, y] = id;
                }
            }
        }
    }

    void InstantiateFloors()
    {
        Transform parent = new GameObject("Rooms").transform;
        parent.parent = transform;

        foreach (RoomInstance rInstance in rooms)
        {
            RectInt r = rInstance.rect;
            Vector3 pos = new Vector3((r.xMin + r.width / 2f) * cellSize, (r.yMin + r.height / 2f) * cellSize, 0f);

            GameObject roomObj = Instantiate(rInstance.prefab, pos, Quaternion.identity, parent);

            RoomHold data = roomObj.GetComponent<RoomHold>();
            if (data != null)
            {
                roomObj.transform.localScale = new Vector3(r.width * cellSize, r.height * cellSize, 1f);
            }
        }
    }
    public void InstantiateCollectibles(bool isBiscuit = false)
    {
        if (CollectiblesPrefabs.Count == 0 || rooms.Count == 0) return;
        int Collectibles = collectibleCount;
        int attempts = 0;
        int placed = 0;
        AllCollectibles.Instance.totalCollectibles = Collectibles;
        LayerMask obstacleMask = LayerMask.GetMask("Furnature", "Wall", "Occluders");
        if (isBiscuit)
        {
            Collectibles = 1;
        }
        while (placed < Collectibles && attempts < Collectibles * 20)
        {
            attempts++;
            RoomInstance r = rooms[Random.Range(0, rooms.Count)];
            RectInt rect = r.rect;
            int x = Random.Range(rect.xMin, rect.xMax);
            int y = Random.Range(rect.yMin, rect.yMax);
            Vector3 pos = new Vector3(x * cellSize + cellSize / 2f, y * cellSize + cellSize / 2f, -0.1f);

            float margin = 0.1f * cellSize;
            Vector2 checkSize = new Vector2(cellSize - margin * 2, cellSize - margin * 2);
            Collider2D hit = Physics2D.OverlapBox(pos, checkSize, 0f, obstacleMask);
            if (hit != null) continue;
            if (isBiscuit)
            {
                Instantiate(BiscuitPrefab, pos, Quaternion.identity, transform);
                placed++;
                break;
            }
            GameObject prefab = CollectiblesPrefabs[Random.Range(0, CollectiblesPrefabs.Count)];
            Instantiate(prefab, pos, Quaternion.identity, transform);

            placed++;
        }
    }




    void InstantiateWallsWithDoors()
    {
        Transform parent = new GameObject("Walls").transform;
        parent.parent = transform;
        var sharedBorders = new Dictionary<(int a, int b), List<(int x, int y, int dir)>>(new PairComparer());

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int id = roomMap[x, y];
                if (id == 0) continue;

                if (x + 1 < gridWidth)
                {
                    int other = roomMap[x + 1, y];
                    if (other != 0 && other != id)
                    {
                        var key = MakePairKey(id, other);
                        if (!sharedBorders.ContainsKey(key)) sharedBorders[key] = new List<(int, int, int)>();
                        sharedBorders[key].Add((x, y, 0));
                    }
                }

                if (y + 1 < gridHeight)
                {
                    int other = roomMap[x, y + 1];
                    if (other != 0 && other != id)
                    {
                        var key = MakePairKey(id, other);
                        if (!sharedBorders.ContainsKey(key)) sharedBorders[key] = new List<(int, int, int)>();
                        sharedBorders[key].Add((x, y, 1));
                    }
                }
            }
        }

        var doorSet = new HashSet<(int x, int y, int dir)>();

        foreach (var kv in sharedBorders)
        {
            var list = kv.Value;
            if (list.Count > 0)
            {
                var pick = list[Random.Range(0, list.Count)];
                doorSet.Add(pick);
            }
        }

        var placedWalls = new HashSet<string>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (roomMap[x, y] == 0) continue;

                CheckAndPlaceWall(x, y, x - 1, y, parent, placedWalls);
                CheckAndPlaceWall(x, y, x + 1, y, parent, placedWalls);
                CheckAndPlaceWall(x, y, x, y - 1, parent, placedWalls);
                CheckAndPlaceWall(x, y, x, y + 1, parent, placedWalls);
            }
        }

        void CheckAndPlaceWall(int cx, int cy, int nx, int ny, Transform parentTransform, HashSet<string> placed)
        {
            bool neighborInBounds = nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridHeight;
            int myId = roomMap[cx, cy];
            int otherId = neighborInBounds ? roomMap[nx, ny] : 0;

            bool isBorder = (!neighborInBounds) || (otherId == 0) || (otherId != myId);
            if (!isBorder) return;

            Vector3 wallPos = Vector3.zero;
            bool horizontal = false;
            (int wx, int wy, int dir) borderKey = (0, 0, 0);

            if (nx == cx + 1 && ny == cy)
            {
                wallPos = new Vector3((cx + 0.5f) * cellSize, cy * cellSize, 0f);
                horizontal = false;
                borderKey = (cx, cy, 0);
            }
            else if (nx == cx - 1 && ny == cy)
            {
                wallPos = new Vector3((cx - 0.5f) * cellSize, cy * cellSize, 0f);
                horizontal = false;
                borderKey = (nx, ny, 0);
            }
            else if (ny == cy + 1 && nx == cx)
            {
                wallPos = new Vector3(cx * cellSize, (cy + 0.5f) * cellSize, 0f);
                horizontal = true;
                borderKey = (cx, cy, 1);
            }
            else if (ny == cy - 1 && nx == cx)
            {
                wallPos = new Vector3(cx * cellSize, (cy - 0.5f) * cellSize, 0f);
                horizontal = true;
                borderKey = (nx, ny, 1);
            }
            else
            {
                return;
            }

            if (neighborInBounds && otherId != 0 && otherId != myId)
            {
                if (doorSet.Contains(borderKey))
                {
                    ClearFurnitureAtDoor(wallPos, horizontal);
                    return;
                }
            }

            string key = $"{wallPos.x:F3}_{wallPos.y:F3}_{(horizontal ? "H" : "V")}";
            if (placed.Contains(key)) return;
            placed.Add(key);

            GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, parentTransform);
            if (!horizontal)
                wall.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            else
                wall.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        void ClearFurnitureAtDoor(Vector3 doorPos, bool horizontal)
        {
            doorPos += new Vector3(0.5f, 0.5f, -1f);
            float width = horizontal ? cellSize : 2f * cellSize;
            float height = horizontal ? 2f * cellSize : cellSize;
            Vector2 size = new Vector2(width, height);
            DrawDebugBox(doorPos, size, Color.red, 1f);
            Collider2D[] hits = Physics2D.OverlapBoxAll(doorPos, new Vector2(width, height), 0f);
            foreach (Collider2D c in hits)
            {
                Debug.Log(c.gameObject.name);
                if (c.CompareTag("Furniture"))
                {
                    Destroy(c.gameObject);
                }
            }
        }

        //hardcode fix:
        parent.transform.position += new Vector3(0.5f, 0.5f, -1f);
    }
    void DrawDebugBox(Vector3 center, Vector2 size, Color color, float duration = 0f)
    {
        Vector3 halfSize = (Vector3)size * 0.5f;

        Vector3 topLeft = center + new Vector3(-halfSize.x, halfSize.y);
        Vector3 topRight = center + new Vector3(halfSize.x, halfSize.y);
        Vector3 bottomLeft = center + new Vector3(-halfSize.x, -halfSize.y);
        Vector3 bottomRight = center + new Vector3(halfSize.x, -halfSize.y);

        Debug.DrawLine(topLeft, topRight, color, duration);
        Debug.DrawLine(topRight, bottomRight, color, duration);
        Debug.DrawLine(bottomRight, bottomLeft, color, duration);
        Debug.DrawLine(bottomLeft, topLeft, color, duration);
    }
    (int a, int b) MakePairKey(int a, int b)
    {
        if (a < b) return (a, b);
        return (b, a);
    }
    class PairComparer : IEqualityComparer<(int a, int b)>
    {
        public bool Equals((int a, int b) x, (int a, int b) y) => x.a == y.a && x.b == y.b;
        public int GetHashCode((int a, int b) obj) => obj.a * 73856093 ^ obj.b * 19349663;
    }

    public class RoomInstance
    {
        public RectInt rect;
        public GameObject prefab;

        public RoomInstance(RectInt r, GameObject p)
        {
            rect = r;
            prefab = p;
        }
    }
}
