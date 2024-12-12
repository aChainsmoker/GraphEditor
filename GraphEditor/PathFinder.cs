namespace GraphEditor
{
    internal class PathFinder
    {
        public static List<List<Node>> FindAllPaths(Graph graph, Node startNode, Node endNode)
        {
            var allPaths = new List<List<Node>>();
            var currentPath = new List<Node>();
            var visited = new HashSet<Node>();

            // Рекурсивный поиск путей
            FindPathsRecursive(startNode, endNode, visited, currentPath, allPaths);

            return allPaths;
        }
        
        private static void FindPathsRecursive(Node currentNode, Node endNode, HashSet<Node> visited, List<Node> currentPath, List<List<Node>> allPaths)
        {
            // Добавляем текущую вершину в путь и отмечаем как посещённую
            currentPath.Add(currentNode);
            visited.Add(currentNode);

            // Если достигли конечной вершины, сохраняем текущий путь
            if (currentNode == endNode)
            {
                allPaths.Add(new List<Node>(currentPath));
            }
            else
            {
                // Обходим все соседние вершины
                foreach (var edge in currentNode.GetListOfAvailableEdges())
                {
                    Node nextNode = edge.GetSecondNode(currentNode);
                    if (!visited.Contains(nextNode))
                    {
                        FindPathsRecursive(nextNode, endNode, visited, currentPath, allPaths);
                    }
                }
            }

            // Возвращаемся назад: удаляем текущую вершину из пути и снимаем отметку о посещении
            currentPath.RemoveAt(currentPath.Count - 1);
            visited.Remove(currentNode);
        }
        
        public static List<Node> FindShortestWay(Graph graph, Node startNode, Node endNode)
        {
            PriorityQueue<Node, int> priorityQueue = new PriorityQueue<Node, int>();
            priorityQueue.Enqueue(startNode, 0); //Временное решение
            Dictionary<Node, Node> came_from = new Dictionary<Node, Node>();
            Dictionary<Node, int> cost_so_far = new Dictionary<Node, int>();
            came_from.Add(startNode, null);
            cost_so_far.Add(startNode, 0);

            while (priorityQueue.Count > 0)
            {
                Node currentNode = priorityQueue.Dequeue();

                if (currentNode == endNode)
                {
                    return FormPath(came_from, endNode);
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
                    }
                }
            }

            return null;
        }

        private static List<Node> FormPath(Dictionary<Node, Node> cameFrom, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node previousNode = endNode;
            while (previousNode != null)
            {
                path.Insert(0, previousNode);
                previousNode = cameFrom[previousNode];
            }
            return path;
        }
    }
}