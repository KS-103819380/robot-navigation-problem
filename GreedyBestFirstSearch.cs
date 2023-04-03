namespace Robot_Navigation_Problem
{
    internal class GreedyBestFirstSearch : InformedSearch
    {
        public GreedyBestFirstSearch(Environment environment) : base(environment)
        {
        }

        protected override int CalculateHeuristic(Node node)
        {
            List<Node> allGoalNodes = _environment.GetGoalNodes();
            //calculate the smallest manhattan distance between all node and the nearest goal node
            int distanceToNearestGoalNode = allGoalNodes.Select(goalNode => Math.Abs(goalNode.X - node.X) + Math.Abs(goalNode.Y - node.Y)).Min();
            return distanceToNearestGoalNode;
        }
    }
}
