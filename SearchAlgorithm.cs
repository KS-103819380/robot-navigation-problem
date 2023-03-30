namespace Robot_Navigation_Problem
{
    internal abstract class SearchAlgorithm : ISearchable
    {
        public const string NOT_FOUND = "No solution found.";
        protected readonly Environment _environment;
        private int _numberOfNodes;

        public int NumberOfNodes
        {
            get => _numberOfNodes;
        }

        protected SearchAlgorithm(Environment environment)
        {
            _environment = environment;
            _numberOfNodes = 0;
        }

        public string Search(bool isGui = false)
        {
            Node startNode = _environment.GetRobotNode();
            startNode.Cost = 0;
            AddNodeToFrontier(startNode);
            AddNodeCount();

            if (isGui)
                Gui.IncreaseNumberOfNodes();

            while (!CheckIfPathNotFound())
            {
                Node currentNode = GetNodeFromFrontier();

                if (currentNode.Visited)
                    continue;

                if (isGui)
                    ColorGuiGrid(startNode, currentNode, GetFrontier());

                if (currentNode.Type == NodeType.Goal)
                    return string.Join("; ", ConstructPath(currentNode)) + ";";

                currentNode.Visited = true;
                List<Node> neighbors = GetNeighbors(currentNode);
                foreach (Node neighbor in neighbors)
                {
                    if (ShouldAddNodeToTree(neighbor))
                    {
                        AddNodeToTree(neighbor, currentNode);
                        if (isGui) Gui.IncreaseNumberOfNodes();
                    }
                }

            }

            return NOT_FOUND;
        }

        protected abstract void AddNodeToFrontier(Node node);

        protected abstract bool CheckIfPathNotFound();

        protected abstract Node GetNodeFromFrontier();

        protected abstract bool ShouldAddNodeToTree(Node neighbor);

        protected abstract IEnumerable<Node> GetFrontier();

        protected virtual List<Node> GetNeighbors(Node node)
        {
            return _environment.GetNeighbors(node);
        }

        protected virtual void AddNodeToTree(Node neighbor, Node currentNode)
        {
            neighbor.Cost = currentNode.Cost + 1;
            neighbor.Parent = currentNode;
            neighbor.InTree = true;
            AddNodeToFrontier(neighbor);
            AddNodeCount();
        }

        /// <summary>
        /// Constructs a path from the start node to the goal node by tracing the parent nodes of the goal node
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns>A list of path that contains element that are either "up", "down", "left", or "right"</returns>
        private static List<string> ConstructPath(Node currentNode)
        {
            List<string> path = new List<string>();

            while (currentNode.Parent != null)
            {
                (int, int) difference = currentNode.Parent - currentNode;
                switch (difference)
                {
                    case (1, 0):
                        path.Add("right");
                        break;
                    case (-1, 0):
                        path.Add("left");
                        break;
                    case (0, 1):
                        path.Add("down");
                        break;
                    case (0, -1):
                        path.Add("up");
                        break;
                    default:
                        throw new Exception("Parent and child nodes are not adjacent");
                }
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }

        protected void AddNodeCount(int count = 1)
        {
            if (count < 0)
                throw new Exception("Count cannot be negative");
            _numberOfNodes += count;
        }

        /// <summary>
        /// Colors the GUI grid based on the current node and the frontier, runs on every iteration
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="currentNode"></param>
        /// <param name="frontier"></param>
        private void ColorGuiGrid(Node startNode, Node currentNode, IEnumerable<Node> frontier)
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

            //set frontier to purple
            foreach (Node node in frontier)
            {
                if (node.Type != NodeType.Goal)
                    Gui.ChangeCellColor(node.Coordinate, CustomBrush.ToBeExplored);
            }

            //set current exploring path to yellow
            List<string> path = ConstructPath(currentNode);
            (int xOffset, int yOffset) = (0, 0);
            foreach (string direction in path)
            {
                Gui.ChangeCellColor(startNode.Coordinate.x + xOffset, startNode.Coordinate.y + yOffset, CustomBrush.Path);
                switch (direction)
                {
                    case "up":
                        yOffset--;
                        break;
                    case "down":
                        yOffset++;
                        break;
                    case "left":
                        xOffset--;
                        break;
                    case "right":
                        xOffset++;
                        break;
                }
            }

            //set start node to orange
            Gui.ChangeCellColor(startNode.Coordinate, CustomBrush.StartNode);

            //set current node to red
            Gui.ChangeCellColor(currentNode.Coordinate, CustomBrush.CurrentNode);

            Application.DoEvents();
            Thread.Sleep(Gui.DurationPerIteration);
        }
    }
}
