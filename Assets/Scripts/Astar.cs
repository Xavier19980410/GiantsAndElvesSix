using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    private class Node : System.IComparable<Node>
    {
        public Vector2Int Position; // 网格坐标
        public Vector3 RealPos;     // 实际坐标
        public Node Parent;         // 父节点，用于追溯路径
        public float G;             // 起点到当前节点的成本
        public float H;             // 当前节点到目标节点的预估成本
        public float F => G + H;

        public Node(Vector2Int position, Node parent, float g, float h)
        {
            Position = position;
            RealPos = HexagonManager.Instance[Position.x, Position.y].GetPos();
            Parent = parent;
            G = g;
            H = h;
        }

        public int CompareTo(Node other)
        {
            int compare = F.CompareTo(other.F);
            if (compare == 0)
            {
                compare = RealPos.x.CompareTo(other.RealPos.x);
                if (compare == 0)
                    compare = RealPos.y.CompareTo(other.RealPos.y);
                if (compare == 0)
                    compare = RealPos.z.CompareTo(other.RealPos.z);
            }
            return compare;
        }
    }

    private SortedSet<Node> openList = new SortedSet<Node>();
    private HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();
    private HexagonManager hexagonManager;

    public Astar()
    {
        hexagonManager = HexagonManager.Instance;
    }

    public List<Vector3> FindPath(Vector2Int start, Vector2Int target)
    {
        openList.Clear();
        closedList.Clear();

        Node startNode = new Node(start, null, 0, GetHeuristic(start, target));
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node current = GetLowestFNode();

            if (current.Position == target)
                return ReconstructPath(current);

            openList.Remove(current);
            closedList.Add(current.Position);

            foreach (Vector2Int neighbor in GetNeighbors(current.Position))
            {
                if (closedList.Contains(neighbor))
                    continue;

                float tentativeG = current.G + GetMovementCost(current.Position, neighbor);

                Node neighborNode = FindNodeInOpenList(neighbor);

                if (neighborNode == null)
                {
                    neighborNode = new Node(neighbor, current, tentativeG, GetHeuristic(neighbor, target));
                    openList.Add(neighborNode);
                }
                else if (tentativeG < neighborNode.G)
                {
                    openList.Remove(neighborNode);
                    neighborNode.Parent = current;
                    neighborNode.G = tentativeG;
                    openList.Add(neighborNode);
                }
            }
        }
        return new List<Vector3>();
    }

    private Node GetLowestFNode()
    {
        return openList.Min;
    }

    private Node FindNodeInOpenList(Vector2Int position)
    {
        foreach (var node in openList)
        {
            if (node.Position == position)
                return node;
        }
        return null;
    }

    private List<Vector3> ReconstructPath(Node current)
    {
        List<Vector3> path = new List<Vector3>();
        while (current != null)
        {
            path.Add(hexagonManager[current.Position.x, current.Position.y].GetPos());
            current = current.Parent;
        }
        path.Reverse();
        return path;
    }

    private float GetHeuristic(Vector2Int from, Vector2Int to)
    {
        var fromPos = hexagonManager[from.x, from.y].GetPos();
        var toPos = hexagonManager[to.x, to.y].GetPos();
        return Mathf.Abs(fromPos.x - toPos.x) + Mathf.Abs(fromPos.y - toPos.y) + Mathf.Abs(fromPos.z - toPos.z);
    }

    private float GetMovementCost(Vector2Int from, Vector2Int to)
    {
        return 1f;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int position)
    {
        Vector2Int[] directions = new Vector2Int[6]
        {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            position.y % 2 == 0 ? new Vector2Int(1, 1) : new Vector2Int(-1, 1),
            position.y % 2 == 0 ? new Vector2Int(1, -1) : new Vector2Int(-1, -1),
        };

        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbor = position + direction;

            // 检查邻居是否在地图范围内
            if (hexagonManager[neighbor.x, neighbor.y] != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}