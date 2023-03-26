namespace Robot_Navigation_Problem
{
    internal class Node
    {
        private NodeType _type;
        private bool _visited = false;
        private bool _inTree = false;
        private Node? _parent = null;
        private int _cost = 0;
        private readonly Coordinate _coordinate;

        public NodeType Type 
        {
            get => _type;
            set => _type = value;
        }

        public bool Visited
        {
            get => _visited;
            set => _visited = value ? value : !value; //can only set visited to true, since we cannot unvisit a node
        }

        public bool InTree
        {
            get => _inTree;
            set => _inTree = value ? value : !value; //can only set inTree to true, since we dont want to remove a node from the tree
        }

        public Node? Parent
        {
            get => _parent;
            set => _parent = value;
        }

        public int Cost
        {
            get => _cost;
            set => _cost = value;
        }

        public Coordinate Coordinate => _coordinate;

        public Node(NodeType type, Coordinate coordinate)
        {
            _type = type;
            _coordinate = coordinate;
        }

        public Node(NodeType type, int x, int y)
        {
            _type = type;
            _coordinate = new Coordinate(x, y);
        }

        public static (int x, int y) operator-(Node node1, Node node2)
        {
            int x = node2.Coordinate.x - node1.Coordinate.x;
            int y = node2.Coordinate.y - node1.Coordinate.y;
            return (x, y);
        }

        public void Reset()
        {
            _visited = false;
            _inTree = false;
            _parent = null;
            _cost = 0;
        }
    }
}
