namespace Robot_Navigation_Problem
{
    internal class DepthFirstSearch : SearchAlgorithm
    {
        private readonly Stack<Node> _stack;

        public DepthFirstSearch(Environment environment) : base(environment)
        {
            _stack = new Stack<Node>();
        }

        public override string Search(bool isGui = false)
        {
            Node startNode = _environment.GetRobotNode();
            _stack.Push(startNode);
            AddNodeCount();
            if (isGui) Gui.IncreaseNumberOfNodes();

            //keep looping until there are no more nodes to visit, which means that no path exists
            while (_stack.Count > 0)
            {
                Node currentNode = _stack.Pop();
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
                    foreach (Node node in _stack)
                    {
                        Gui.ChangeCellColor(node.Coordinate, CustomBrush.ToBeExplored);
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
                //mark current node as visited
                currentNode.Visited = true;
                if (currentNode.Type == NodeType.Goal)
                {
                    //if the current node is the goal node, then we have found a path
                    return string.Join("; ", ConstructPath(currentNode)) + ";";
                }
                else
                {
                    //get all neighbors of current node
                    List<Node> neighbours = _environment.GetNeighbors(currentNode);
                    //reverse the order of the neighbours since the GetNeighbours method returns in order, and we want to push from the reverse order so that the last element is the first to be visited
                    neighbours.Reverse();
                    //push all unvisited neighbors onto the stack
                    foreach (Node neighbour in neighbours)
                    {
                        if (!neighbour.Visited)
                        {
                            _stack.Push(neighbour);
                            neighbour.InTree = true;
                            neighbour.Parent = currentNode;
                            AddNodeCount();
                            Gui.IncreaseNumberOfNodes();
                        }
                    }
                }
            }

            return NOT_FOUND;
        }
    }
}
