namespace Robot_Navigation_Problem
{
    internal abstract class InformedSearch : SearchAlgorithm
    {
        private readonly PriorityQueue<Node, int> _priorityQueue;

        public InformedSearch(Environment environment) : base(environment)
        {
            _priorityQueue = new PriorityQueue<Node, int>();
        }

        public override void AddNodeToFrontier(Node node)
        {
            _priorityQueue.Enqueue(node, CalculateHeuristic(node));
        }

        public override Node GetNodeFromFrontier()
        {
            return _priorityQueue.Dequeue();
        }

        public override bool NotFoundCondition()
        {
            return _priorityQueue.Count == 0;
        }

        public override bool ShouldAddNodeToTree(Node node)
        {
            return !node.Visited;
        }

        public override IEnumerable<Node> GetFrontier()
        {
            return _priorityQueue.UnorderedItems.Select(element => element.Element);
        }

        protected abstract int CalculateHeuristic(Node node);
    }
}
