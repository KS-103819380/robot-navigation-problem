namespace Robot_Navigation_Problem
{
    internal class UniformCostSearch : SearchAlgorithm
    {
        private readonly PriorityQueue<Node> _priorityQueue;

        public UniformCostSearch(Environment environment) : base(environment)
        {
            _priorityQueue = new PriorityQueue<Node>();
        }

        protected override void AddNodeToFrontier(Node node)
        {
            _priorityQueue.Enqueue(node, node.Cost);
        }

        protected override IEnumerable<Node> GetFrontier()
        {
            return _priorityQueue.UnorderedItems;
        }

        protected override Node GetNodeFromFrontier()
        {
            return _priorityQueue.Dequeue();
        }

        protected override bool CheckIfPathNotFound()
        {
            return _priorityQueue.Count == 0;
        }

        protected override bool ShouldAddNodeToTree(Node neighbor)
        {
            return !neighbor.Visited;
        }
    }
}
