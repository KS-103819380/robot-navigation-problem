namespace Robot_Navigation_Problem
{
    internal interface ISearchable
    {
        string Search(bool isGui);

        void AddNodeToFrontier(Node node);

        bool CheckIfPathNotFound();

        Node GetNodeFromFrontier();

        bool ShouldAddNodeToTree(Node node);

        IEnumerable<Node> GetFrontier();
    }
}
