using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEditor
{
    internal class Graph
    {
        private List<Node> nodes = new List<Node>();
        private List<Edge> edges = new List<Edge>();

        public List<Node> Nodes { get { return nodes; } }
        public List<Edge> Edges { get { return edges; } }
        public Func<Edge, Edge> AddingEdge;

        public void RemoveEdge(Edge edge)
        {
            edges.Remove(edge);
        }

        public void AddEdge(Edge edge)
        {
            edges.Add(edge);
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
        }

        public void RemoveNode(Node node)
        {
            nodes.Remove(node); 
        }

        public void ResetNodesChecks()
        {
            for(int i =0; i < nodes.Count; i++)
                nodes[i].isChecked = false;
        }

        public Edge FindEdgeBetweenNodes(Node node1, Node node2)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i].isOriented == false)
                {
                    if ((edges[i].StartNode == node1 && edges[i].EndNode == node2) ||
                        (edges[i].StartNode == node2 && edges[i].EndNode == node1))
                        return edges[i];
                }
                else
                {
                    if (edges[i].StartNode == node1 && edges[i].EndNode == node2)
                        return edges[i];  
                }
            }
            return null;
        }

        public List<Edge> FindAvailableEdges(Node node)
        {
            List<Edge> availableEdges = new List<Edge>();
            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i].isOriented == false)
                {
                    if (edges[i].StartNode == node || edges[i].EndNode == node)
                        availableEdges.Add(edges[i]);
                }
                else
                {
                    if (edges[i].StartNode == node)
                        availableEdges.Add(edges[i]);  
                }
            }
            return availableEdges;
        }
    }
}
