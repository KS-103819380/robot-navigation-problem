namespace Robot_Navigation_Problem
{
    internal abstract class InformedSearch : SearchAlgorithm
    {
        protected readonly PriorityQueue<Node> _priorityQueue;

        public InformedSearch(Environment environment) : base(environment)
        {
            _priorityQueue = new PriorityQueue<Node>();
        }

        protected override void AddNodeToFrontier(Node node)
        {
            _priorityQueue.Enqueue(node, CalculateHeuristic(node));
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
            return !neighbor.InTree && !neighbor.Visited;
        }

        protected override IEnumerable<Node> GetFrontier()
        {
            return _priorityQueue.UnorderedItems;
        }

        protected abstract int CalculateHeuristic(Node node);
    }
}
