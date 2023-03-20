namespace Robot_Navigation_Problem
{
    internal abstract class SearchAlgorithm : ISearchable
    {
        public static readonly string NOT_FOUND = "No solution found.";
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

        public abstract string Search(bool isGui = false);

        /// <summary>
        /// Constructs a path from the start node to the goal node by tracing the parent nodes of the goal node
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns>A list of path that contains element that are either "up", "down", "left", or "right"</returns>
        protected static List<string> ConstructPath(Node currentNode)
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
    }
}
