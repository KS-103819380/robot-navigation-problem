namespace Robot_Navigation_Problem
{    
    internal class BreadthFirstSearch : SearchAlgorithm
    {
        private readonly Queue<Node> _queue;

        public BreadthFirstSearch(Environment environment) : base(environment)
        {
            _queue = new Queue<Node>();
        }

        protected override void AddNodeToFrontier(Node node)
        {
            _queue.Enqueue(node);
        }

        protected override Node GetNodeFromFrontier()
        {
            return _queue.Dequeue();
        }

        protected override bool CheckIfPathNotFound()
        {
            return _queue.Count == 0;
        }

        protected override bool ShouldAddNodeToTree(Node neighbor)
        {
            return !neighbor.Visited && !neighbor.InTree;
        }

        protected override IEnumerable<Node> GetFrontier()
        {
            return _queue;
        }
    }
}
