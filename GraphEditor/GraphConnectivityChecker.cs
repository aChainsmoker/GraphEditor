using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GraphEditor
{
    internal class GraphConnectivityChecker
    {
        
        public static bool CheckGraphConnectivity(Graph graph)
        {
            for (int i = 0; i < graph.Nodes.Count; i++)
            {
                if(CheckNodeConnectivity(graph.Nodes[i], graph.Nodes) == false)
                {graph.ResetNodesChecks();return false;}
                graph.ResetNodesChecks();
            }
            return true;
        }

        public static void FixGraphConnectivity(Graph graph)
        {
            if(CheckGraphConnectivity(graph) == true)
                return;
            
            for (int i = 0; i < graph.Nodes.Count; i++)
            {
                FixNodeConnectivity(graph.Nodes[i], graph.Nodes, graph);
                graph.ResetNodesChecks();
            }
        }
        public static bool CheckNodeConnectivity(Node startNode, List<Node> nodes)
        {
            if (startNode.isChecked == true)
                return false;

            startNode.isChecked = true;
            for(int i =0; i<startNode.edges.Count; i++)
            {
                if (startNode.edges[i].isOriented == false)
                    CheckNodeConnectivity(startNode.edges[i].StartNode, nodes);
                CheckNodeConnectivity(startNode.edges[i].EndNode, nodes);
            }

            for (int i =0;i < nodes.Count; i++)
            {
                if (nodes[i].isChecked == false)
                {
                    return false;
                }
            }
            return true;
        }
        
        public static void FixNodeConnectivity(Node startNode, List<Node> nodes, Graph graph)
        {
            if (startNode.isChecked == true)
                return;

            startNode.isChecked = true;
            for(int i =0; i<startNode.edges.Count; i++)
            {
                if (startNode.edges[i].isOriented == false)
                    CheckNodeConnectivity(startNode.edges[i].StartNode, nodes);
                CheckNodeConnectivity(startNode.edges[i].EndNode, nodes);
            }

            for (int i =0;i < nodes.Count; i++)
            {
                if (nodes[i].isChecked == false)
                {
                    Edge edge = new Edge
                    {
                        StartNode = startNode,
                        EndNode = nodes[i],
                        isOriented = false
                    };
                    Point startPoint = startNode.ellipse.TranslatePoint(
                        new Point(startNode.ellipse.ActualWidth / 2, startNode.ellipse.ActualHeight / 2),
                        (UIElement)startNode.Parent);
                    edge.polyline.Points.Add(startPoint);
                    edge.polyline.Points.Add(new Point());
                    graph.AddingEdge?.Invoke(edge);
                    return; //Если захочу, чтобы все отсоеденённые компоненты связности привязывалисьь к одному ребру, надо будет сделать, так чтобы цикл методы FixGraphConnectivity откатывался назад на 1, в случае, если было добавлено ребро
                }
            }
        }

    }
}
