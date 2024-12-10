using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace GraphEditor
{
    internal class EulerianCycleFinder
    {
        private static List<Edge> visitedEdges = new List<Edge>();
        private static List<Node> eulerianCycle = new List<Node>();
        private static List<List<Node>> eulerianCycles = new List<List<Node>>();


        public static List<List<Node>> FindEulerianCycles(Graph graph)
        {
            eulerianCycles.Clear();
            for (int i = 0; i < graph.Nodes.Count; i++)
            {
                eulerianCycles.Add(FindEulerianCycle(graph, graph.Nodes[i]));
            }
            return eulerianCycles; 
        }

        public static List<Node> FindEulerianCycle(Graph graph, Node startNode)
        {
            if (graph.Edges.Count == 0)
                return null;

            // Создаем копию списка рёбер для безопасной модификации
            var edgesCopy = new List<Edge>(graph.Edges);
            var stack = new Stack<Node>();
            var cycle = new List<Node>();

            // Начинаем с любого узла
            stack.Push(startNode);

            while (stack.Count > 0)
            {
                Node current = stack.Peek();
                
                // Найти инцидентное ребро для текущего узла
                var incidentEdge = edgesCopy.FirstOrDefault(edge => (edge.StartNode == current && edge.isOriented == false) || (edge.EndNode == current));

                if (incidentEdge != null)
                {
                    // Удалить ребро из копии и перейти к следующему узлу
                    edgesCopy.Remove(incidentEdge);
                    var nextNode = incidentEdge.StartNode == current ? incidentEdge.EndNode : incidentEdge.StartNode;
                    stack.Push(nextNode);
                }
                else
                {
                    // Если больше нет инцидентных рёбер, добавляем узел в цикл
                    cycle.Add(current);
                    stack.Pop();
                }
            }

            return cycle;
        }
        
        //Параша
        // public static void FindEulerianCycle(Edge startEdge, Edge previousEdge, Node node, Graph graph)
        // {
        //     if (visitedEdges.Contains(startEdge))
        //     {
        //         if(startEdge == previousEdge)
        //             return;
        //         eulerianCycle.Add(node);
        //         return;
        //     }
        //     visitedEdges.Add(startEdge);
        //     Node nextNode;
        //     if (startEdge.EndNode == node)
        //         nextNode = startEdge.StartNode;
        //     else
        //         nextNode = startEdge.EndNode;
        //
        //     for (int i = 0; i < nextNode.edges.Count; i++)
        //     {
        //         FindEulerianCycle(nextNode.edges[i], startEdge, nextNode, graph);
        //     }
        // }
    }
}
