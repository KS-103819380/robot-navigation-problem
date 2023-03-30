namespace Robot_Navigation_Problem
{
    internal abstract class InformedSearch : SearchAlgorithm
    {
        private readonly PriorityQueue<Node> _priorityQueue;

        public InformedSearch(Environment environment) : base(environment)
        {
            _priorityQueue = new PriorityQueue<Node>();
        }

        protected override void AddNodeToFrontier(Node node)
        {
            Console.WriteLine("Adding node to frontier: " + node.Coordinate + " with priority: " + CalculateHeuristic(node));
            _priorityQueue.Enqueue(node, CalculateHeuristic(node));
        }

        protected override Node GetNodeFromFrontier()
        {
            Node node = _priorityQueue.Dequeue();
            Console.WriteLine("----------------------");
            Console.WriteLine("Dequeueing: " + node.Coordinate + " with priority: " + CalculateHeuristic(node));
            Console.Write("Current frontier: ");
            for (int i = 0; i < _priorityQueue.Count; i++)
            {
                Console.Write(_priorityQueue.UnorderedItems.ElementAt(i).Coordinate + " ");
            }
            Console.WriteLine();
            return node;
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
