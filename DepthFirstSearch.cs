namespace Robot_Navigation_Problem
{
    internal class DepthFirstSearch : SearchAlgorithm
    {
        private readonly Stack<Node> _stack;

        public DepthFirstSearch(Environment environment) : base(environment)
        {
            _stack = new Stack<Node>();
        }

        public override bool ShouldAddNodeToTree(Node node)
        {
            return !node.Visited;
        }

        public override void AddNodeToFrontier(Node node)
        {
            _stack.Push(node);
        }

        public override Node GetNodeFromFrontier()
        {
            return _stack.Pop();
        }

        public override bool CheckIfPathNotFound()
        {
            return _stack.Count == 0;
        }

        public override IEnumerable<Node> GetFrontier()
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
