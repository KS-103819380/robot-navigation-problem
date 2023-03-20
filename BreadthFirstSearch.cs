namespace Robot_Navigation_Problem
{    
    internal class BreadthFirstSearch : SearchAlgorithm
    {
        private readonly Queue<Node> _queue;

        public BreadthFirstSearch(Environment environment) : base(environment)
        {
            _queue = new Queue<Node>();
        }

        public override string Search(bool isGui = false)
        {
            Node startNode = _environment.GetRobotNode();
            _queue.Enqueue(startNode);
            AddNodeCount();
            if (isGui) Gui.IncreaseNumberOfNodes();

            //keep looping until there are no more nodes to visit, which means that no path exists
            while (_queue.Count > 0)
            {
                Node currentNode = _queue.Dequeue();
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
                    foreach (Node node in _queue)
                    {
                        Gui.ChangeCellColor(node.Coordinate, CustomBrush.ToBeExplored);
                    }
                    //color the current exploring path to yellow
                    List<string> path = ConstructPath(currentNode);
                    (int x, int y) directionOffset = (0, 0);
                    foreach (string direction in path)
                    {
                        Gui.ChangeCellColor(startNode.Coordinate.x + directionOffset.x, startNode.Coordinate.y + directionOffset.y, CustomBrush.Path);
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
                    }
                    //set start node to orange
                    Gui.ChangeCellColor(startNode.Coordinate, CustomBrush.StartNode);
                    //set the current node to red
                    Gui.ChangeCellColor(currentNode.Coordinate, CustomBrush.CurrentNode);
                    Application.DoEvents();
                    Thread.Sleep(Gui.DurationPerIteration);
                }
                if (currentNode.Type == NodeType.Goal)
                {
                    //if the current node is the goal node, then we have found a path
                    return string.Join("; ", ConstructPath(currentNode)) + ";";
                }
                else
                {
                    //mark current node as visited
                    currentNode.Visited = true;
                    //get all neighbors of current node
                    List<Node> neighbours = _environment.GetNeighbors(currentNode);
                    //push all unvisited and not-in-tree neighbors onto the queue
                    foreach (Node neighbour in neighbours)
                    {
                        if (!neighbour.Visited && !neighbour.InTree)
                        {
                            neighbour.Parent = currentNode;
                            _queue.Enqueue(neighbour);
                            neighbour.InTree = true;
                            AddNodeCount();
                            if (isGui) Gui.IncreaseNumberOfNodes();
                        }
                    }
                }
            }

            return NOT_FOUND;
        }
    }
}
