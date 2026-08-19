using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;

namespace RescuAR.App.Services.Routing;

public class AStarPathfinder
{
    public class Node
    {
        public Coordinate Coordinate { get; set; }
        public List<Edge> Edges { get; set; } = new();
        public double G { get; set; } = double.MaxValue;
        public double F { get; set; } = double.MaxValue;
        public Node? Parent { get; set; }

        public Node(Coordinate coord)
        {
            Coordinate = coord;
        }
    }

    public class Edge
    {
        public Node Target { get; set; }
        public double Weight { get; set; }
        public Edge(Node target, double weight)
        {
            Target = target;
            Weight = weight;
        }
    }

    private Dictionary<Coordinate, Node> _graph = new();

    public void BuildGraph(IEnumerable<Geometry> geometries)
    {
        _graph.Clear();

        foreach (var geom in geometries)
        {
            if (geom is LineString lineString)
            {
                AddLineString(lineString);
            }
            else if (geom is MultiLineString multiLineString)
            {
                foreach (LineString ls in multiLineString.Geometries)
                {
                    AddLineString(ls);
                }
            }
        }
    }

    private void AddLineString(LineString ls)
    {
        for (int i = 0; i < ls.Coordinates.Length - 1; i++)
        {
            var p1 = ls.Coordinates[i];
            var p2 = ls.Coordinates[i + 1];

            if (!_graph.TryGetValue(p1, out var node1))
            {
                node1 = new Node(p1);
                _graph[p1] = node1;
            }

            if (!_graph.TryGetValue(p2, out var node2))
            {
                node2 = new Node(p2);
                _graph[p2] = node2;
            }

            double dist = p1.Distance(p2);
            node1.Edges.Add(new Edge(node2, dist));
            node2.Edges.Add(new Edge(node1, dist)); // undirected graph
        }
    }

    public List<Coordinate> FindPath(Coordinate start, Coordinate goal)
    {
        var startNode = GetNearestNode(start);
        var goalNode = GetNearestNode(goal);

        if (startNode == null || goalNode == null) return new List<Coordinate>();

        foreach (var node in _graph.Values)
        {
            node.G = double.MaxValue;
            node.F = double.MaxValue;
            node.Parent = null;
        }

        startNode.G = 0;
        startNode.F = Heuristic(startNode.Coordinate, goalNode.Coordinate);

        var openSet = new HashSet<Node> { startNode };
        var closedSet = new HashSet<Node>();

        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(n => n.F).First();

            if (current == goalNode)
            {
                return ReconstructPath(current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (var edge in current.Edges)
            {
                if (closedSet.Contains(edge.Target)) continue;

                double tentativeG = current.G + edge.Weight;

                if (tentativeG < edge.Target.G)
                {
                    edge.Target.Parent = current;
                    edge.Target.G = tentativeG;
                    edge.Target.F = tentativeG + Heuristic(edge.Target.Coordinate, goalNode.Coordinate);

                    if (!openSet.Contains(edge.Target))
                    {
                        openSet.Add(edge.Target);
                    }
                }
            }
        }

        return new List<Coordinate>(); // No path found
    }

    private Node? GetNearestNode(Coordinate c)
    {
        Node? nearest = null;
        double minDist = double.MaxValue;
        foreach (var node in _graph.Values)
        {
            double d = node.Coordinate.Distance(c);
            if (d < minDist)
            {
                minDist = d;
                nearest = node;
            }
        }
        return nearest;
    }

    private double Heuristic(Coordinate a, Coordinate b)
    {
        return a.Distance(b);
    }

    private List<Coordinate> ReconstructPath(Node current)
    {
        var path = new List<Coordinate>();
        while (current != null)
        {
            path.Add(current.Coordinate);
            current = current.Parent;
        }
        path.Reverse();
        return path;
    }
}
