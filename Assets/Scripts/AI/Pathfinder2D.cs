using UnityEngine;
using System.Collections.Generic;

public class Pathfinder2D : MonoBehaviour
{
    public LayerMask Occluders;      
    public float PathBuffer = 0.20f; 
    
    Grid2D grid;

    void Awake() => grid = FindFirstObjectByType<Grid2D>();

    public bool TryFindPath(Vector2 start, Vector2 target, out List<Vector2> waypoints)
    {
        waypoints = null;
        if (grid == null) return false;

        var startNode = grid.NodeFromWorldPoint(start);
        var targetNode = grid.NodeFromWorldPoint(target);
        if (!targetNode.walkable) targetNode = ClosestWalkable(targetNode);
        if (!startNode.walkable || targetNode == null) return false;

        var open = new List<Grid2D.Node>();
        var closed = new HashSet<Grid2D.Node>();

        startNode.gCost = 0;
        startNode.hCost = Heuristic(startNode, targetNode);
        startNode.parent = null;
        open.Add(startNode);

        while (open.Count > 0)
        {
            var current = open[0];
            for (int i = 1; i < open.Count; i++)
                if (open[i].fCost < current.fCost || (open[i].fCost == current.fCost && open[i].hCost < current.hCost))
                    current = open[i];

            open.Remove(current);
            closed.Add(current);

            if (current == targetNode) { waypoints = Retrace(startNode, targetNode); return true; }

            foreach (var nb in grid.GetNeighbours(current))
            {
                if (closed.Contains(nb)) continue;
                int newCost = current.gCost + StepCost(current, nb);
                if (!open.Contains(nb) || newCost < nb.gCost)
                {
                    nb.gCost = newCost;
                    nb.hCost = Heuristic(nb, targetNode);
                    nb.parent = current;
                    if (!open.Contains(nb)) open.Add(nb);
                }
            }
        }
        return false;
    }

    int StepCost(Grid2D.Node a, Grid2D.Node b) => (a.gridX == b.gridX || a.gridY == b.gridY) ? 10 : 14;

    int Heuristic(Grid2D.Node a, Grid2D.Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        int diag = Mathf.Min(dx, dy);
        int straight = Mathf.Abs(dx - dy);
        return 14 * diag + 10 * straight;
    }

    List<Vector2> Retrace(Grid2D.Node start, Grid2D.Node end)
    {
        var path = new List<Vector2>();
        var c = end;
        while (c != start && c != null) { path.Add(c.worldPosition); c = c.parent; }
        path.Reverse();
        return Simplify(path);
    }

    List<Vector2> Simplify(List<Vector2> path)
    {
        if (path == null || path.Count <= 2) return path;
        var result = new List<Vector2>();
        Vector2 current = path[0];
        result.Add(current);

        int i = 1;
        while (i < path.Count)
        {
            int j = i;
            while (j + 1 < path.Count && HasClearPathBuffered(current, path[j + 1])) j++;
            current = path[j];
            result.Add(current);
            i = j + 1;
        }
        return result;
    }

    Grid2D.Node ClosestWalkable(Grid2D.Node from)
    {
        var q = new Queue<Grid2D.Node>();
        var seen = new HashSet<Grid2D.Node>();
        q.Enqueue(from); seen.Add(from);
        int layers = 0;
        while (q.Count > 0 && layers++ < 8)
        {
            int count = q.Count;
            while (count-- > 0)
            {
                var n = q.Dequeue();
                foreach (var nb in grid.GetNeighbours(n))
                {
                    if (nb.walkable) return nb;
                    if (seen.Add(nb)) q.Enqueue(nb);
                }
            }
        }
        return from;
    }
    bool HasClearPathBuffered(Vector2 from, Vector2 to)
    {
        Vector2 dir = to - from; float dist = dir.magnitude;
        if (dist < 1e-4f) return true;
        return !Physics2D.CircleCast(from, PathBuffer, dir.normalized, dist, Occluders);
    }
}
