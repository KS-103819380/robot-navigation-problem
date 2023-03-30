namespace Robot_Navigation_Problem
{
    internal readonly struct Coordinate
    {
        public readonly int x;
        public readonly int y;

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Coordinate((int, int) coordinate)
        {
            x = coordinate.Item1;
            y = coordinate.Item2;
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}
