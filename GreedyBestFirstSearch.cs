namespace Robot_Navigation_Problem
{
    internal class GreedyBestFirstSearch : SearchAlgorithm
    {
        private readonly PriorityQueue<Node, int> _priorityQueue;

        public GreedyBestFirstSearch(Environment environment) : base(environment)
        {
            _priorityQueue = new PriorityQueue<Node, int>();
        }

        public override string Search(bool isGui = false)
        {
            Node startNode = _environment.GetRobotNode();
            _priorityQueue.Enqueue(startNode, CalculateHeuristic(startNode));
            AddNodeCount();
            if (isGui) Gui.IncreaseNumberOfNodes();

            while (_priorityQueue.Count > 0)
            {
                Node currentNode = _priorityQueue.Dequeue();
                if (isGui)
                {
                    Gui.IncrementIteration();
                    //set explored nodes to blue
                    for (int i = 0; i < _environment.Height; i++)
                    {
                        for (int j = 0; j < _environment.Width; j++)
                        {
                            Node node = _environment.GetNode(j, i);
                            if (node.Visited)
                                Gui.ChangeCellColor(node.Coordinate, CustomBrush.Visited);
                        }
                    }
                    //set nodes to be explored to purple
                    foreach (var (Element, Priority) in _priorityQueue.UnorderedItems)
                    {
                        Gui.ChangeCellColor(Element.Coordinate, CustomBrush.ToBeExplored);
                    }
                    //color the current exploring path to yellow
                    List<string> path = ConstructPath(currentNode);
                    (int x, int y) directionOffset = (0, 0);
                    foreach (string direction in path)
                    {
                        switch (direction)
                        {
                            case "up":
                                directionOffset.y--;
                                break;
                            case "down":
                                directionOffset.y++;
                                break;
                            case "left":
                                directionOffset.x--;
                                break;
                            case "right":
                                directionOffset.x++;
                                break;
                        }
                        Gui.ChangeCellColor(startNode.Coordinate.x + directionOffset.x, startNode.Coordinate.y + directionOffset.y, CustomBrush.Path);
                    }
                    //set the start node to orange
                    Gui.ChangeCellColor(startNode.Coordinate, CustomBrush.StartNode);
                    //set the current node to red
                    Gui.ChangeCellColor(currentNode.Coordinate, CustomBrush.CurrentNode);
                    Application.DoEvents();
                    Thread.Sleep(Gui.DurationPerIteration);
                }
                currentNode.Visited = true;

                if(currentNode.Type == NodeType.Goal)
                {
                    return string.Join("; ", ConstructPath(currentNode));
                }
                else
                {
                    //get all neighbors of current node
                    List<Node> neighbors = _environment.GetNeighbors(currentNode);
                    foreach (Node neighbor in neighbors)
                    {
                        //push neighbor into the queue if it is already in tree
                        if (!neighbor.Visited && !neighbor.InTree)
                        {
                            _priorityQueue.Enqueue(neighbor, CalculateHeuristic(neighbor));
                            neighbor.InTree = true;
                            neighbor.Parent = currentNode;
                            AddNodeCount();
                        }
                    }
                }
            }

            return NOT_FOUND;
        }

        private int CalculateHeuristic(Node node)
        {
            List<Node> allGoalNodes = _environment.GetGoalNodes();
            //calculate the smallest manhattan distance between all node and the nearest goal node
            int minDistance = allGoalNodes.Select(goalNode => Math.Abs(goalNode.Coordinate.x - node.Coordinate.x) + Math.Abs(goalNode.Coordinate.y - node.Coordinate.y)).Min();
            return minDistance;
        }
    }
}
