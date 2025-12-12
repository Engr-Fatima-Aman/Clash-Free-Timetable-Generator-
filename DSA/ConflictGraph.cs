using System;
using System.Collections.Generic;
using System.Linq;

namespace dsa_project.DSA
{
    public class Node
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public NodeType Type { get; set; }
        public List<int> ConflictingNodeIds { get; set; } = new List<int>();
        public int AssignedColor { get; set; } = -1;
    }

    public enum NodeType
    {
        Course,
        Teacher,
        Class,
        Room
    }

    public class ConflictGraph
    {
        private Dictionary<int, Node> nodes;
        private Dictionary<int, List<int>> adjacencyList;
        private int edgeCount;

        public ConflictGraph()
        {
            nodes = new Dictionary<int, Node>();
            adjacencyList = new Dictionary<int, List<int>>();
            edgeCount = 0;
        }

        public void AddNode(int id, string name, NodeType type)
        {
            if (!nodes.ContainsKey(id))
            {
                nodes[id] = new Node
                {
                    Id = id,
                    Name = name,
                    Type = type,
                    ConflictingNodeIds = new List<int>()
                };
                adjacencyList[id] = new List<int>();
            }
        }

        public void AddConflict(int nodeId1, int nodeId2)
        {
            if (nodes.ContainsKey(nodeId1) && nodes.ContainsKey(nodeId2) && nodeId1 != nodeId2)
            {
                if (!adjacencyList[nodeId1].Contains(nodeId2))
                {
                    adjacencyList[nodeId1].Add(nodeId2);
                    nodes[nodeId1].ConflictingNodeIds.Add(nodeId2);
                    edgeCount++;
                }

                if (!adjacencyList[nodeId2].Contains(nodeId1))
                {
                    adjacencyList[nodeId2].Add(nodeId1);
                    nodes[nodeId2].ConflictingNodeIds.Add(nodeId1);
                    edgeCount++;
                }
            }
        }

        public List<int> GetConflictingNodes(int nodeId)
        {
            return adjacencyList.ContainsKey(nodeId) ? new List<int>(adjacencyList[nodeId]) : new List<int>();
        }

        public Node GetNode(int nodeId)
        {
            return nodes.ContainsKey(nodeId) ? nodes[nodeId] : null;
        }

        public Dictionary<int, Node> GetAllNodes() => new Dictionary<int, Node>(nodes);
        public Dictionary<int, List<int>> GetAdjacencyList() => new Dictionary<int, List<int>>(adjacencyList);
        public int GetNodeCount() => nodes.Count;
        public int GetEdgeCount() => edgeCount;
        public bool HasNode(int nodeId) => nodes.ContainsKey(nodeId);

        public int GetNodeDegree(int nodeId)
        {
            return nodes.ContainsKey(nodeId) ? adjacencyList[nodeId].Count : 0;
        }

        public void Clear()
        {
            nodes.Clear();
            adjacencyList.Clear();
            edgeCount = 0;
        }
    }
}