namespace Robot_Navigation_Problem
{
    internal class JumpPointSearch : InformedSearch
    {
        public JumpPointSearch(Environment environment) : base(environment)
        {
        }

        public override string Search()
        {
            Node startNode = _environment.GetRobotNode();
            startNode.Cost = 0;
            AddNodeToFrontier(startNode);
            AddNodeCount();

            if (Gui.GuiModeEnabled)
                Gui.IncreaseNumberOfNodes();

            while (!CheckIfPathNotFound())
            {
                Node currentNode = GetNodeFromFrontier();
                currentNode.Visited = true;

                if (Gui.GuiModeEnabled)
                    ColorGuiGrid(startNode, currentNode, GetFrontier());

                if (currentNode.Type == NodeType.Goal)
                    return string.Join("; ", ConstructPath(currentNode)) + ";";

                List<Node> successors = GetSuccessors(currentNode);
                foreach (Node successor in successors)
                {
                    _priorityQueue.Enqueue(successor, CalculateHeuristic(successor));
                    Gui.IncreaseNumberOfNodes();
                }
            }

            return NOT_FOUND;
        }

        private List<Node> GetSuccessors(Node currentNode)
        {
            List<Node> opened = new List<Node>();
            List<Node> neighbors = GetNeighbors(currentNode);

            foreach (Node neighbor in neighbors)
            {
                Node? jumpNode = Jump(neighbor, currentNode);

                //don't add a node we have already gotten to quicker
                if (jumpNode == null || jumpNode.Visited)
                    continue;

                int manhattanDistance = Math.Abs(jumpNode.X - currentNode.X) + Math.Abs(jumpNode.Y - currentNode.Y);
                int newCost = currentNode.Cost + manhattanDistance;

                if (!jumpNode.Visited || newCost < jumpNode.Cost)
                {
                    jumpNode.Cost = newCost;
                    jumpNode.Parent = currentNode;
                    opened.Add(jumpNode);

                    if (!jumpNode.Visited)
                    {
                        jumpNode.Visited = true;
                        opened.Add(jumpNode);
                    }
                }
            }
            return opened;
        }

        private Node? Jump(Node neighbor, Node currentNode)
        {
            if (neighbor == null || neighbor.Type == NodeType.Wall)
                return null;

            if (neighbor.Type == NodeType.Goal)
                return neighbor;

            int dx = neighbor.X - currentNode.X;
            int dy = neighbor.Y - currentNode.Y;

            //check for forced neighbors
            if (dx != 0)
            {
                if ((IsWalkable(neighbor.X, neighbor.Y + 1) && !IsWalkable(neighbor.X - dx, neighbor.Y + 1)) ||
                    (IsWalkable(neighbor.X, neighbor.Y - 1) && !IsWalkable(neighbor.X - dx, neighbor.Y - 1)))
                    return neighbor;
            }
            else if (dy != 0)
            {
                if ((IsWalkable(neighbor.X + 1, neighbor.Y) && !IsWalkable(neighbor.X + 1, neighbor.Y - dy)) ||
                    (IsWalkable(neighbor.X - 1, neighbor.Y) && !IsWalkable(neighbor.X - 1, neighbor.Y - dy)))
                    return neighbor;
                try
                {
                    if (Jump(_environment.GetNode(neighbor.X + 1, neighbor.Y), neighbor) != null ||
                        Jump(_environment.GetNode(neighbor.X - 1, neighbor.Y), neighbor) != null)
                        return neighbor;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            try
            {
                return Jump(_environment.GetNode(neighbor.X + dx, neighbor.Y + dy), neighbor);
            }
            catch
            {
                return null;
            }
        }

        protected override List<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>();
            Node? parent = node.Parent;

            if (parent != null)
            {
                int x = node.X;
                int y = node.Y;

                int dx = (x - parent.X) / Math.Max(Math.Abs(x - parent.X), 1);
                int dy = (y - parent.Y) / Math.Max(Math.Abs(y - parent.Y), 1);

                if (dx != 0)
                {
                    if (IsWalkable(x + dx, y))
                        neighbors.Add(_environment.GetNode(x + dx, y));
                    if (IsWalkable(x, y + 1))
                        neighbors.Add(_environment.GetNode(x, y + 1));
                    if (IsWalkable(x, y - 1))
                        neighbors.Add(_environment.GetNode(x, y - 1));
                }
                else if (dy != 0)
                {
                    if (IsWalkable(x, y + dy))
                        neighbors.Add(_environment.GetNode(x, y + dy));
                    if (IsWalkable(x + 1, y))
                        neighbors.Add(_environment.GetNode(x + 1, y));
                    if (IsWalkable(x - 1, y))
                        neighbors.Add(_environment.GetNode(x - 1, y));
                }
            }
            else
            {
                List<Node> allNeighbors = _environment.GetNeighbors(node);
                foreach (Node neighbor in allNeighbors)
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private bool IsWalkable(int x, int y)
        {
            if (x < 0 || x >= _environment.Width || y < 0 || y >= _environment.Height)
                return false;
            else if (_environment.GetNode(x, y).Type == NodeType.Wall)
                return false;
            else
                return true;
        }

        protected override int CalculateHeuristic(Node node)
        {
            List<Node> allGoalNodes = _environment.GetGoalNodes();
            //calculate the smallest manhattan distance between all node and the nearest goal node
            int distanceToNearestGoalNode = allGoalNodes.Select(goalNode => Math.Abs(goalNode.X - node.X) + Math.Abs(goalNode.Y - node.Y)).Min();
            return distanceToNearestGoalNode + node.Cost;
        }

        protected override bool ShouldAddNodeToTree(Node neighbor)
        {
            return !neighbor.Visited;
        }

        protected override List<string> ConstructPath(Node currentNode)
        {
            List<string> path = new List<string>();
            while (currentNode.Parent != null)
            {
                (int x, int y) = currentNode.Coordinate - currentNode.Parent.Coordinate;
                if (x < 0)
                {
                    for (int i = 0; i < Math.Abs(x); i++)
                        path.Add("left");
                }    
                else if (x > 0)
                {
                    for (int i = 0; i < Math.Abs(x); i++)
                        path.Add("right");
                }
                else if (y < 0)
                {
                    for (int i = 0; i < Math.Abs(y); i++)
                        path.Add("up");
                }
                else if (y > 0)
                {
                    for (int i = 0; i < Math.Abs(y); i++)
                        path.Add("down");
                }
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }
    }
}
