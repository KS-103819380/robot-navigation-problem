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

        public static (int x, int y) operator -(Coordinate coordinate1, Coordinate coordinate2)
        {
            int x = coordinate1.x - coordinate2.x;
            int y = coordinate1.y - coordinate2.y;
            return (x, y);
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}
