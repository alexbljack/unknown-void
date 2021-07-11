using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar
{
    const int MOVE_STRAIGHT_COST = 10;
    const int MOVE_DIAGONAL_COST = 14;
    
    Grid _grid;
    
    List<Node> _openList;
    List<Node> _closedList;
    
    public AStar(Vector3 position, Vector2 size, float step)
    {
        var xCount = (int) (size.x / step);
        var yCount = (int) (size.y / step);
        
        _grid = new Grid(position, xCount, yCount, step);
    }

    public void DebugDraw()
    {
        _grid.DrawGrid();
    }

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        if (!_grid.InGridBounds(start) || !_grid.InGridBounds(end))
        {
            Debug.LogWarning($"Outside grid ${start} or ${end}!");
            return new List<Vector3>();
        }
        
        Node startNode = _grid.GetNodeFromWorld(start);
        Node endNode = _grid.GetNodeFromWorld(end);

        _openList = new List<Node> { startNode };
        _closedList = new List<Node>();
        
        for (var x = 0; x < _grid.Width; x++)
        {
            for (var y = 0; y < _grid.Height; y++)
            {
                Node node = _grid.GetNode(x, y);
                node.GCost = int.MaxValue;
                node.CameFromNode = null;
            }
        }

        startNode.GCost = 0;
        startNode.HCost = CalcDistance(startNode, endNode);

        while (_openList.Count > 0)
        {
            Node node = GetLowestFCostNode(_openList);
            if (node == endNode)
            {
                var pathNodes = from pathNode in CalculatedPath(node) select _grid.GetNodeWorld(pathNode.x, pathNode.y);
                return pathNodes.ToList();
            }
            
            _openList.Remove(node);
            _closedList.Add(node);

            foreach (Node neighbour in GetNeighbours(node))
            {
                if (_closedList.Contains(neighbour)) continue;

                int tentativeGCost = node.GCost + CalcDistance(node, neighbour);
                if (tentativeGCost < neighbour.GCost)
                {
                    neighbour.CameFromNode = node;
                    neighbour.GCost = tentativeGCost;
                    neighbour.HCost = CalcDistance(neighbour, endNode);

                    if (!_openList.Contains(neighbour))
                    {
                        _openList.Add(neighbour);
                    }
                }
            }
        }

        return null;
    }

    List<Node> CalculatedPath(Node endNode)
    {
        var path = new List<Node> {endNode};
        Node current = endNode;
        
        while (current.CameFromNode != null)
        {
            path.Add(current.CameFromNode);
            current = current.CameFromNode;
        }
        
        path.Reverse();
        return path;
    }

    int CalcDistance(Node a, Node b)
    {
        int xDist = Mathf.Abs(a.x - b.x);
        int yDist = Mathf.Abs(a.y - b.y);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDist, yDist) + MOVE_STRAIGHT_COST * Mathf.Abs(xDist - yDist);
    }

    Node GetLowestFCostNode(List<Node> nodes)
    {
        Node lowestFCostNode = nodes[0];
        
        for (int i = 1; i < nodes.Count; i++)
        {
            if (nodes[i].FCost < lowestFCostNode.FCost)
            {
                lowestFCostNode = nodes[i];
            }
        }

        return lowestFCostNode;
    }

    List<Node> GetNeighbours(Node node)
    {
        var neighbours = new List<Node>();
        int x = node.x;
        int y = node.y;
        
        var indices = new List<Vector2Int>
        {
            new Vector2Int(x-1, y),
            new Vector2Int(x-1, y-1),
            new Vector2Int(x-1, y+1),
            new Vector2Int(x+1, y),
            new Vector2Int(x+1, y-1),
            new Vector2Int(x+1, y+1),
            new Vector2Int(x, y+1),
            new Vector2Int(x, y-1)
        };

        foreach (Vector2Int index in indices)
        {
            if (index.x > 0 && index.x < _grid.Width && index.y > 0 && index.y < _grid.Height)
            {
                neighbours.Add(_grid.GetNode(index.x, index.y));
            }
        }
        
        return neighbours;
    }
}

public class Grid
{
    Node[,] nodes;
    
    public readonly int Width;
    public readonly int Height;
    public readonly Vector3 Position;
    public readonly float WorldStep;
    
    public Grid(Vector3 worldPosition, int xCount, int yCount, float step)
    {
        Width = xCount;
        Height = yCount;
        Position = worldPosition;
        WorldStep = step;

        nodes = new Node[Width, Height];
        
        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < yCount; j++)
            {
                nodes[i, j] = new Node(i, j);
            }
        }
        
        Debug.Log($"{Width} x {Height} / {WorldStep}");
    }

    public Node GetNode(int x, int y)
    {
        return nodes[x, y];
    }

    public Vector3 GetNodeWorld(int x, int y)
    {
        return Position + new Vector3(x * WorldStep, y * WorldStep, 0);;
    }

    public Node GetNodeFromWorld(Vector3 point)
    {
        Node node = null;
        float distance = float.MaxValue;
        
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Vector3 nodePos = GetNodeWorld(i, j);
                float toNode = Vector3.Distance(point, nodePos);
                
                if (toNode < distance)
                {
                    distance = toNode;
                    node = GetNode(i, j);
                }
            }
        }

        return node;
    }

    public bool InGridBounds(Vector3 point)
    {
        var sizeX = Width * WorldStep;
        var sizeY = Height * WorldStep;
        return (point.x >= Position.x && point.x <= Position.x + sizeX && point.y >= Position.y && point.y <= Position.y + sizeY);
    }

    public void DrawGrid()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Vector3 center = GetNodeWorld(i, j);
                Color color = Color.green;
                Debug.DrawLine(center + (Vector3.left) * 0.1f, center + (Vector3.right) * 0.1f, color);
                Debug.DrawLine(center + (Vector3.up) * 0.1f, center + (Vector3.down) * 0.1f, color);
            }
        }
    }
}

public class Node
{
    public int x;
    public int y;

    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;
    
    public Node CameFromNode;
    
    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    public override string ToString()
    {
        return $"Node [{x}, {y}]";
    }
}