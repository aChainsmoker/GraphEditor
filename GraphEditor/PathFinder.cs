using System.Windows.Documents;

namespace GraphEditor
{
    internal class PathFinder
    {
        public static List<Dictionary<Node, Edge>> FindAllPaths(Graph graph, Node startNode, Node endNode)
        {
            var allPaths = new List<List<Node>>();
            var allEdgePaths = new List<List<Edge>>();
            var currentPath = new List<Node>();
            var currendEdgePath = new List<Edge>();
            var visited = new HashSet<Node>();

            // Рекурсивный поиск путей
            FindPathsRecursive(startNode, endNode, null, visited, currentPath, currendEdgePath, allPaths, allEdgePaths);

            return FormPath(allPaths, allEdgePaths);
        }
        
        private static void FindPathsRecursive(Node currentNode, Node endNode, Edge currentEdge, HashSet<Node> visited, List<Node> currentPath, List<Edge> currentEdgePath, List<List<Node>> allPaths, List<List<Edge>> allEdgePaths)
        {
            // Добавляем текущую вершину в путь и отмечаем как посещённую
            currentPath.Add(currentNode);
            currentEdgePath.Add(currentEdge);
            visited.Add(currentNode);

            // Если достигли конечной вершины, сохраняем текущий путь
            if (currentNode == endNode)
            {
                allPaths.Add(new List<Node>(currentPath));
                allEdgePaths.Add(new List<Edge>(currentEdgePath));
            }
            else
            {
                // Обходим все соседние вершины
                foreach (var edge in currentNode.GetListOfAvailableEdges())
                {
                    Node nextNode = edge.GetSecondNode(currentNode);
                    if (!visited.Contains(nextNode))
                    {
                        FindPathsRecursive(nextNode, endNode, edge, visited, currentPath, currentEdgePath, allPaths, allEdgePaths);
                    }
                }
            }

            // Возвращаемся назад: удаляем текущую вершину из пути и снимаем отметку о посещении
            currentPath.RemoveAt(currentPath.Count - 1);
            currentEdgePath.RemoveAt(currentEdgePath.Count - 1);
            visited.Remove(currentNode);
        }
        
        public static Dictionary<Node,Edge> FindShortestWay(Graph graph, Node startNode, Node endNode)
        {
            PriorityQueue<Node, int> priorityQueue = new PriorityQueue<Node, int>();
            priorityQueue.Enqueue(startNode, 0); //Временное решение
            Dictionary<Node, Node> came_from = new Dictionary<Node, Node>();
            Dictionary<Node,Edge> cameUsingEdge = new Dictionary<Node,Edge>();
            Dictionary<Node, int> cost_so_far = new Dictionary<Node, int>();
            came_from.Add(startNode, null);
            cameUsingEdge.Add(startNode, null);
            cost_so_far.Add(startNode, 0);

            while (priorityQueue.Count > 0)
            {
                Node currentNode = priorityQueue.Dequeue();

                if (currentNode == endNode)
                {
                    return FormPath(came_from, endNode, cameUsingEdge);
                }
                
                
                foreach (var nextEdge in currentNode.GetListOfAvailableEdges())
                {
                    int newCost = cost_so_far[currentNode] + nextEdge.Weight;
                    if (cost_so_far.ContainsKey(nextEdge.GetSecondNode(currentNode)) == false || newCost < cost_so_far[nextEdge.GetSecondNode(currentNode)])
                    {
                        cost_so_far[nextEdge.GetSecondNode(currentNode)] = newCost;
                        int priority = newCost;
                        priorityQueue.Enqueue(nextEdge.GetSecondNode(currentNode), priority);
                        came_from[nextEdge.GetSecondNode(currentNode)] = currentNode;
                        cameUsingEdge[nextEdge.GetSecondNode(currentNode)] = nextEdge;
                    }
                }
            }

            return null;
        }

        private static Dictionary<Node, Edge> FormPath(Dictionary<Node, Node> cameFrom, Node endNode, Dictionary<Node, Edge> cameUsingEdge)
        {
            Dictionary<Node, Edge> path = new Dictionary<Node, Edge>();
            Node previousNode = endNode;
            while (previousNode != null)
            {
                path[previousNode] = cameUsingEdge[previousNode];
                previousNode = cameFrom[previousNode];
            }
            return path;
        }

        private static List<Dictionary<Node, Edge>> FormPath(List<List<Node>> allPaths, List<List<Edge>> allEdgePaths)
        {
            List<Dictionary<Node, Edge>> paths = new List<Dictionary<Node, Edge>>();
            for (int i = 0; i < allPaths.Count; i++)
            {
                Dictionary<Node, Edge> path = new Dictionary<Node, Edge>();
                for (int j = 0; j < allPaths[i].Count; j++)
                {
                    path.Add(allPaths[i][j], allEdgePaths[i][j]);
                }
                paths.Add(path);
            }
            return paths;
        }

        public static int AssessThePath(Dictionary<Node, Edge> path)
        {
            int psthCost = 0;
            
            if (path == null)
                return -1;
            foreach (var Node in path)
            {
                if(Node.Value != null)
                    psthCost += Node.Value.Weight;
            }
            return psthCost;
        }
    }
}