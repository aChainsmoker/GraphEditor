namespace GraphEditor
{
    internal class PathFinder
    {
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