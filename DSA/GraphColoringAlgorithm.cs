using System;
using System.Collections.Generic;
using System.Linq;

namespace dsa_project.DSA
{
    // Greedy Graph Coloring Algorithm - O(V^2 + E)
    // Used for finding minimum time slots needed before backtracking
    public class GraphColoringAlgorithm
    {
        private ConflictGraph graph;
        private Dictionary<int, int> nodeColors;
        private int chromaticNumber;

        public GraphColoringAlgorithm(ConflictGraph conflictGraph)
        {
            graph = conflictGraph;
            nodeColors = new Dictionary<int, int>();
            chromaticNumber = 0;
        }

        // Greedy coloring algorithm - O(V^2 + E)
        public Dictionary<int, int> ColorGraph(int maxColors)
        {
            var nodes = graph.GetAllNodes();

            if (nodes.Count == 0)
                return nodeColors;

            // Initialize all nodes as uncolored (-1)
            foreach (var node in nodes.Values)
            {
                nodeColors[node.Id] = -1;
            }

            // Assign first color to first node
            nodeColors[nodes.Values.First().Id] = 0;
            chromaticNumber = 1;

            bool[] available = new bool[maxColors];

            // Assign colors to remaining nodes
            foreach (var node in nodes.Values.Skip(1))
            {
                // Reset available colors for this node
                // (Web projects usually run on newer .NET, so Array.Fill is better/faster)
                Array.Fill(available, true);

                // Mark colors of conflicting nodes as unavailable
                foreach (int conflictNodeId in graph.GetConflictingNodes(node.Id))
                {
                    if (nodeColors.ContainsKey(conflictNodeId) && nodeColors[conflictNodeId] != -1)
                    {
                        // Check bounds to be safe
                        if (nodeColors[conflictNodeId] < available.Length)
                        {
                            available[nodeColors[conflictNodeId]] = false;
                        }
                    }
                }

                // Find first available color
                int color = 0;
                for (int i = 0; i < maxColors; i++)
                {
                    if (available[i])
                    {
                        color = i;
                        break;
                    }
                }

                nodeColors[node.Id] = color;
                chromaticNumber = Math.Max(chromaticNumber, color + 1);
            }

            return nodeColors;
        }
    }
}