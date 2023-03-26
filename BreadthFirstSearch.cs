namespace Robot_Navigation_Problem
{    
    internal class BreadthFirstSearch : SearchAlgorithm
    {
        private readonly Queue<Node> _queue;

        public BreadthFirstSearch(Environment environment) : base(environment)
        {
            _queue = new Queue<Node>();
        }

        public override void AddNodeToFrontier(Node node)
        {
            _queue.Enqueue(node);
        }

        public override Node GetNodeFromFrontier()
        {
            return _queue.Dequeue();
        }

        public override bool NotFoundCondition()
        {
            return _queue.Count == 0;
        }

        public override bool ShouldAddNodeToTree(Node node)
        {
            return !node.Visited && !node.InTree;
        }

        public override IEnumerable<Node> GetFrontier()
        {
            return _queue;
        }
    }
}
