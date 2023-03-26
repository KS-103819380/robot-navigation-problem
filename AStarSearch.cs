namespace Robot_Navigation_Problem
{    
    internal class AStarSearch : InformedSearch
    {
        public AStarSearch(Environment environment) : base(environment)
        {
        }

        protected override int CalculateHeuristic(Node node)
        {
            List<Node> allGoalNodes = _environment.GetGoalNodes();
            //calculate the smallest manhattan distance between all node and the nearest goal node
            int minDistance = allGoalNodes.Select(goalNode => Math.Abs(goalNode.Coordinate.x - node.Coordinate.x) + Math.Abs(goalNode.Coordinate.y - node.Coordinate.y)).Min();
            return minDistance + node.Cost;
        }

    }
}
