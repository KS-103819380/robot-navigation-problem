namespace Robot_Navigation_Problem
{
    internal class DepthFirstSearch : SearchAlgorithm
    {
        private readonly Stack<Node> _stack;

        public DepthFirstSearch(Environment environment) : base(environment)
        {
            _stack = new Stack<Node>();
        }

        protected override bool ShouldAddNodeToTree(Node neighbor)
        {
            return !neighbor.Visited;
        }

        protected override void AddNodeToFrontier(Node node)
        {
            _stack.Push(node);
        }

        protected override Node GetNodeFromFrontier()
        {
            return _stack.Pop();
        }

        protected override bool CheckIfPathNotFound()
        {
            return _stack.Count == 0;
        }

        protected override IEnumerable<Node> GetFrontier()
        {
            return _stack;
        }

        //override the base virtual method to get the reverse order of neighbors since we want to push the node to explored first last
        protected override List<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = _environment.GetNeighbors(node);
            neighbors.Reverse();
            return neighbors;
        }
    }
}
