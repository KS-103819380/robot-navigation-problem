﻿namespace Robot_Navigation_Problem
{
    internal class Environment
    {
        private readonly List<List<Node>> _grid;
        private int _width;
        private int _height;
        private Node _robotNode;
        private readonly List<Node> _goalNodes;

        public int Width => _width;
        public int Height => _height;
        public Node RobotNode { get => _robotNode; set => _robotNode = value; }
        public List<Node> GoalNodes => _goalNodes;

        public Environment(int width, int height)
        {
            _width = width;
            _height = height;
            _robotNode = new Node(NodeType.Robot, 0, 0);
            _goalNodes = new List<Node>();
            _grid = new List<List<Node>>();
            for (int i = 0; i < height; i++)
            {
                _grid.Add(new List<Node>());
                for (int j = 0; j < width; j++)
                {
                    _grid[i].Add(new Node(NodeType.Empty, j, i));
                }
            }
        }
        
        public Environment(int width, int height, Coordinate robotLoc, List<Coordinate> goalLoc, List<Coordinate> wallLocs)
        {
            //initialize width and height of the environment
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and height must be greater than 0");
            _width = width;
            _height = height;

            //initialize grid
            _grid = new List<List<Node>>();
            //fill grid with empty nodes
            for (int i = 0; i < _height; i++)
            {
                List<Node> row = new List<Node>();
                for (int j = 0; j < _width; j++)
                {
                    row.Add(new Node(NodeType.Empty, j, i));
                }
                _grid.Add(row);
            }
            //set robot location
            _grid[robotLoc.y][robotLoc.x].Type = NodeType.Robot;
            _robotNode = _grid[robotLoc.y][robotLoc.x];
            //set goal locations
            _goalNodes = new List<Node>();
            foreach (Coordinate goal in goalLoc)
            {
                _grid[goal.y][goal.x].Type = NodeType.Goal;
                _goalNodes.Add(_grid[goal.y][goal.x]);
            }
            //set wall locations
            foreach (Coordinate wall in wallLocs)
            {
                _grid[wall.y][wall.x].Type = NodeType.Wall;
            } 
        }

        public override string ToString()
        {
            string output = "";
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    output += _grid[i][j].Type.ToString()[0];
                }
                output += "\n";
            }
            return output;
        }

        public Node GetRobotNode()
        {
            return _robotNode;
        }

        public List<Node> GetGoalNodes()
        {
            return _goalNodes;
        }

        /// <summary>
        /// Gets all neighbors of a node that are not walls
        /// </summary>
        /// <param name="node"></param>
        /// <returns>A list of node in up, left, down, right order, as specified in the assignment</returns>
        public List<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>();
            //get neighbors above
            if (node.Y - 1 >= 0 && _grid[node.Y - 1][node.X].Type != NodeType.Wall)
                neighbors.Add(_grid[node.Y - 1][node.X]);
            //get neighbors to the left
            if (node.X - 1 >= 0 && _grid[node.Y][node.X - 1].Type != NodeType.Wall)
                neighbors.Add(_grid[node.Y][node.X - 1]);
            //get neighbors below
            if (node.Y + 1 < _height && _grid[node.Y + 1][node.X].Type != NodeType.Wall)
                neighbors.Add(_grid[node.Y + 1][node.X]);
            //get neighbors to the right
            if (node.X + 1 < _width && _grid[node.Y][node.X + 1].Type != NodeType.Wall)
                neighbors.Add(_grid[node.Y][node.X + 1]);
            return neighbors;
        }

        public Node GetNode(int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                throw new Exception($"Cannot get node type at {x}, {y}");
            return _grid[y][x];
        }

        public void Reset()
        {
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    _grid[i][j].Reset();
                }
            }
        }

        public void ChangeCellType(int x, int y, NodeType type)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                throw new Exception($"Cannot change node type at {x}, {y}");
            Node nodeToBeChanged = _grid[y][x];
            Node currentRobotNode = GetRobotNode();
            //environment must have at least 1 goal
            if (nodeToBeChanged.Type == NodeType.Goal && type != NodeType.Goal && GetGoalNodes().Count == 1)
                throw new Exception("Environment must have at least 1 goal");
            switch (type)
            {
                case NodeType.Robot:
                    currentRobotNode.Type = nodeToBeChanged.Type;
                    nodeToBeChanged.Type = NodeType.Robot;
                    _robotNode = nodeToBeChanged;
                    break;
                case NodeType.Goal:
                    //if the node is currently a robot, swap the robot and goal
                    if (nodeToBeChanged.Type == NodeType.Robot)
                    {
                        currentRobotNode.Type = NodeType.Goal;
                        _goalNodes.Add(currentRobotNode);
                        nodeToBeChanged.Type = NodeType.Robot;
                        _robotNode = nodeToBeChanged;
                    }
                    else
                    {
                        nodeToBeChanged.Type = NodeType.Goal;
                        _goalNodes.Add(nodeToBeChanged);
                    }
                    break;
                case NodeType.Wall:
                    //if the node is currently a robot, swap the robot and wall
                    if (nodeToBeChanged.Type == NodeType.Robot)
                    {
                        currentRobotNode.Type = NodeType.Wall;
                        nodeToBeChanged.Type = NodeType.Robot;
                        _robotNode = nodeToBeChanged;
                    }
                    //if the node is currently a goal, remove it from the list of goals
                    else if (nodeToBeChanged.Type == NodeType.Goal)
                    {
                        //if there is only 1 goal, throw an exception
                        if (GetGoalNodes().Count == 1)
                            throw new Exception("Environment must have at least 1 goal");
                        _goalNodes.Remove(nodeToBeChanged);
                        nodeToBeChanged.Type = NodeType.Wall;
                    }
                    else
                    {
                        nodeToBeChanged.Type = NodeType.Wall;
                    }
                    break;
                case NodeType.Empty:
                    if (nodeToBeChanged.Type == NodeType.Robot)
                        throw new Exception("Environment must have 1 robot. Please change the robot's location before removing it.");
                    if (nodeToBeChanged.Type == NodeType.Goal)
                        _goalNodes.Remove(nodeToBeChanged);
                    nodeToBeChanged.Type = NodeType.Empty;
                    break;
                default:
                    throw new Exception("Invalid node type");
            }
        }

        public void ChangeDimension(int newWidth, int newHeight)
        {
            if (newWidth <= 1 || newHeight <= 1)
                throw new ArgumentException("Width and height must be at least 2.");
            if (newWidth == _width && newHeight == _height)
                return;
            if (newWidth < _width)
            {
                for (int i = 0; i < _height; i++)
                {
                    _grid[i].RemoveRange(newWidth, _width - newWidth);
                }
            }
            else if (newWidth > _width)
            {
                for (int i = 0; i < _height; i++)
                {
                    for (int j = _width; j < newWidth; j++)
                    {
                        _grid[i].Add(new Node(NodeType.Empty, j, i));
                    }
                }
            }
            if (newHeight < _height)
            {
                _grid.RemoveRange(newHeight, _height - newHeight);
            }
            else if (newHeight > _height)
            {
                for (int i = _height; i < newHeight; i++)
                {
                    List<Node> newRow = new List<Node>();
                    for (int j = 0; j < newWidth; j++)
                    {
                        newRow.Add(new Node(NodeType.Empty, j, i));
                    }
                    _grid.Add(newRow);
                }
            }

            _width = newWidth;
            _height = newHeight;

            try
            {
                GetRobotNode();
            }
            catch (Exception)
            {
                Node topLeftNode = GetNode(0, 0);
                topLeftNode.Type = NodeType.Robot;
            }

            //if there is no goal, add one
            int goalCount = GetGoalNodes().Count;
            if (goalCount > 0) return;
            //pick a random node to be a goal, if it is a robot node, random again
            while (true)
            {
                Random random = new Random();
                int randomX = random.Next(0, _width);
                int randomY = random.Next(0, _height);

                Node randomNode = GetNode(randomX, randomY);
                if (randomNode.Type != NodeType.Robot)
                {
                    randomNode.Type = NodeType.Goal;
                    break;
                }
            }
        }
    }
}
