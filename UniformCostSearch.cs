namespace Robot_Navigation_Problem
{
    internal class UniformCostSearch : SearchAlgorithm
    {
        private readonly PriorityQueue<Node, int> _priorityQueue;

        public UniformCostSearch(Environment environment) : base(environment)
        {
            _priorityQueue = new PriorityQueue<Node, int>();
        }

        public override void AddNodeToFrontier(Node node)
        {
            _priorityQueue.Enqueue(node, node.Cost);
        }

        public override IEnumerable<Node> GetFrontier()
        {
            return _priorityQueue.UnorderedItems.Select(element => element.Element);
        }

        public override Node GetNodeFromFrontier()
        {
            return _priorityQueue.Dequeue();
        }

        public override bool CheckIfPathNotFound()
        {
            return _priorityQueue.Count == 0;
        }

        public override bool ShouldAddNodeToTree(Node node)
        {
            return !node.Visited;
        }
    }
}
