using System.Collections.Generic;
using UnityEngine;

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
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    private int[,] roomMap;

    private List<RectInt> rooms = new List<RectInt>();
    public GameObject[] Rooms;
    public GameObject Parent;
    public int roomCount = 5;
    public int gridSize = 10;
    public int Difficulty = 1;

    void Awake()
    {
        Rooms = Resources.LoadAll<GameObject>("Chamber");
    }
    void Start()
    {
        GenerateRooms();
    }
    public void GenerateRooms()
    {
        GenerateRoomsTouching();
        FillRoomMap();
        InstantiateFloors();
        InstantiateWallsWithDoors();
    }
    void GenerateRoomsTouching()
    {
        rooms.Clear();
        int attempts = 0;
        RectInt first = CreateRandomRoomPosition();
        rooms.Add(first);

        while (rooms.Count < maxRooms && attempts < placeAttempts)
        {
            attempts++;

            RectInt newRoom = CreateRandomRoom();
            bool placed = false;
            for (int tryAttach = 0; tryAttach < 30 && !placed; tryAttach++)
            {
                RectInt existing = rooms[Random.Range(0, rooms.Count)];
                int side = Random.Range(0, 4);

                RectInt candidate = SetRoomTouching(existing, newRoom, side);
                if (candidate.xMin < 0 || candidate.yMin < 0 || candidate.xMax > gridWidth || candidate.yMax > gridHeight)
                    continue;

                if (!OverlapsAny(candidate))
                {
                    rooms.Add(candidate);
                    placed = true;
                }
            }

            if (!placed)
            {
                RectInt fallback = CreateRandomRoomPosition();
                if (!OverlapsAny(fallback))
                {
                    rooms.Add(fallback);
                }
            }
        }
    }

    RectInt CreateRandomRoom()
    {
        List<GameObject> possibleRooms = new List<GameObject>(Rooms);
        foreach (GameObject room in possibleRooms)
        {
            if (room.GetComponent<RoomHold>().Difficultymin < Difficulty || room.GetComponent<RoomHold>().Difficultymax > Difficulty)
            {
                possibleRooms.Remove(room);
            }
            else
            {
                Debug.LogError("Room size exceeds grid size!");
            }
        }
        int i = Random.Range(0, possibleRooms.Count);
        GameObject chosenRoom = possibleRooms[i];
        return new RectInt(0, 0, chosenRoom.GetComponent<RoomHold>().size.x, chosenRoom.GetComponent<RoomHold>().size.y);
    }

    RectInt CreateRandomRoomPosition()
    {
        RectInt r = CreateRandomRoom();
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
        foreach (RectInt r in rooms)
        {
            if (candidate.Overlaps(r))
                return true;
        }
        return false;
    }

    void FillRoomMap()
    {
        roomMap = new int[gridWidth, gridHeight];
        for (int i = 0; i < rooms.Count; i++)
        {
            RectInt r = rooms[i];
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
        Transform parent = new GameObject("Floors").transform;
        parent.parent = transform;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (roomMap[x, y] > 0)
                {
                    Vector3 pos = new Vector3(x * cellSize, y * cellSize, 0f);
                    Instantiate(floorPrefab, pos, Quaternion.identity, parent);
                }
            }
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
                    if (other == 0)
                    {
                        // border with empty space -> will place vertical wall at (x + 0.5, y)
                    }
                    else if (other != id)
                    {
                        var key = MakePairKey(id, other);
                        if (!sharedBorders.ContainsKey(key)) sharedBorders[key] = new List<(int, int, int)>();
                        sharedBorders[key].Add((x, y, 0)); // dir 0 vertical
                    }
                }
                else
                {
                    // grid edge (treat as border)
                }

                if (y + 1 < gridHeight)
                {
                    int other = roomMap[x, y + 1];
                    if (other == 0)
                    {
                        // border with empty space -> will place horizontal wall at (x, y + 0.5)
                    }
                    else if (other != id)
                    {
                        var key = MakePairKey(id, other);
                        if (!sharedBorders.ContainsKey(key)) sharedBorders[key] = new List<(int, int, int)>();
                        sharedBorders[key].Add((x, y, 1)); // dir 1 horizontal
                    }
                }
                else
                {
                    // grid edge
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
                    return;
            }

            string key = $"{wallPos.x:F3}_{wallPos.y:F3}_{(horizontal ? "H" : "V")}";
            if (placed.Contains(key)) return;
            placed.Add(key);

            GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, parentTransform);
            if (horizontal)
            {
                wall.transform.localScale = new Vector3(1f * cellSize, 0.1f * cellSize, 1f);
            }
            else
            {
                wall.transform.localScale = new Vector3(0.1f * cellSize, 1f * cellSize, 1f);
            }
        }
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
}
