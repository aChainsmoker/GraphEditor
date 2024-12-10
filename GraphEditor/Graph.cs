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
    }
}
