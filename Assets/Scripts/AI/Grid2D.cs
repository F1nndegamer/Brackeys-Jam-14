using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics.Geometry;

public class Grid2D : MonoBehaviour
{
    public LayerMask Occluders;
    public GameObject[] guardians;
    public Vector2 WorldSize = new Vector2(50, 50);
    public float NodeRadius = 0.25f;
    public bool DrawGizmos = true;
    public float Clearance = 0.2f;

    Node[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;
    Vector2 bottomLeft;
    Stack<GameObject> _waypoints;


    public void CreateGrid()
    {
        nodeDiameter = NodeRadius * 2f;
        gridSizeX = Mathf.Max(1, Mathf.RoundToInt(WorldSize.x / nodeDiameter));
        gridSizeY = Mathf.Max(1, Mathf.RoundToInt(WorldSize.y / nodeDiameter));
        grid = new Node[gridSizeX, gridSizeY];
        bottomLeft = (Vector2)transform.position - WorldSize * 0.5f;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 p = bottomLeft + new Vector2(x + 0.5f, y + 0.5f) * nodeDiameter;
                bool walkable = !Physics2D.OverlapCircle( p,NodeRadius * 0.95f + Clearance, Occluders);
                grid[x, y] = new Node(walkable, p, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector2 worldPos)
    {
        float px = Mathf.Clamp01((worldPos.x - bottomLeft.x) / WorldSize.x);
        float py = Mathf.Clamp01((worldPos.y - bottomLeft.y) / WorldSize.y);
        int x = Mathf.Clamp(Mathf.RoundToInt((gridSizeX - 1) * px), 0, gridSizeX - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt((gridSizeY - 1) * py), 0, gridSizeY - 1);
        return grid[x, y];
    }

    public IEnumerable<Node> GetNeighbours(Node n)
    {
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int x = n.gridX + dx, y = n.gridY + dy;
                if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY) continue;
                // köþe kesmeyi engelle:
                if (dx != 0 && dy != 0)
                {
                    if (!grid[n.gridX + dx, n.gridY].walkable) continue;
                    if (!grid[n.gridX, n.gridY + dy].walkable) continue;
                }
                var nb = grid[x, y];
                if (nb.walkable) yield return nb;
            }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(WorldSize.x, WorldSize.y, 0));
        if (!DrawGizmos || grid == null) return;
        float s = (NodeRadius * 2f) * 0.9f;
        foreach (var n in grid)
        {
            Gizmos.color = n.walkable ? new Color(1, 1, 1, 0.05f) : new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(n.worldPosition, new Vector3(s, s, 0));
        }
    }
    void FindWaypoints()
    {
        GameObject[] g = GameObject.FindGameObjectsWithTag("Waypoint");
        _waypoints = new Stack<GameObject>(g);
    }
    void CreateMoveableSecurity()
    {
        if (_waypoints == null || _waypoints.Count == 0)
            return;

        if (guardians == null || guardians.Length == 0)
        {
            Debug.LogWarning("Guard prefabs are not defined.");
            return;
        }

        while (_waypoints.Count > 0)
        {
            int prefabIndex = Random.Range(0, guardians.Length);
            GameObject prefab = guardians[prefabIndex];

            Transform a = _waypoints.Pop().transform;

            Transform b = _waypoints.Count > 0 ? _waypoints.Pop().transform : null;

            Vector3 pos = a != null ? a.position : Vector3.zero;
            Quaternion rot = a != null ? a.rotation : Quaternion.identity;

            GameObject spawned = Instantiate(prefab, pos, rot);

            if (spawned.TryGetComponent<SecurityGuard>(out var guard))
            {
                if (b != null)
                {
                    guard.SetWaypoint(new Transform[] { a, b });
                }
                else
                {
                    guard.SetWaypoint(new Transform[] { a });
                }
            }
            else if (spawned.TryGetComponent<Dog>(out var dog))
            {
                if (b != null)
                {
                    dog.SetWaypoint(new Transform[] { a, b });
                }
                else
                {
                    dog.SetWaypoint(new Transform[] { a });
                }
            }
            else
            {
                Debug.LogWarning($"'{prefab.name}' There is no SecurityGuard in the prefab.");
            }
        }
    }
    public class Node
    {
        public bool walkable;
        public Vector2 worldPosition;
        public int gridX, gridY;
        public int gCost, hCost;
        public Node parent;
        public int fCost => gCost + hCost;
        public Node(bool w, Vector2 pos, int x, int y) { walkable = w; worldPosition = pos; gridX = x; gridY = y; }
    }

    public IEnumerator RebuildNextFrame(Grid2D g)
    {
        yield return null;              
        yield return new WaitForFixedUpdate(); 
        g.CreateGrid();
        yield return null;                 
        yield return new WaitForFixedUpdate();
        FindWaypoints();
        yield return null;
        yield return new WaitForFixedUpdate();
        CreateMoveableSecurity();
    }
}
