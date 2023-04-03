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
            int minDistance = allGoalNodes.Select(goalNode => Math.Abs(goalNode.X - node.X) + Math.Abs(goalNode.Y - node.Y)).Min();
            return minDistance + node.Cost;
        }

        protected override bool ShouldAddNodeToTree(Node neighbor)
        {
            return !neighbor.Visited;
        }

        protected override void AddNodeToTree(Node neighbor, Node currentNode)
        {
            //we only want to update the parent if the updated cost is lower (i.e., it is a more efficient path)
            if (neighbor.Cost > currentNode.Cost + 1)
            {
                neighbor.Cost = currentNode.Cost + 1;
                neighbor.Parent = currentNode;
            }
            neighbor.InTree = true;
            AddNodeToFrontier(neighbor);
            AddNodeCount();
        }
    }
}
