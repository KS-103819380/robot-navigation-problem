namespace Robot_Navigation_Problem
{
    internal static class CustomBrush
    {
        public static Brush CurrentNode { get => Brushes.Red; }
        public static Brush Visited { get => Brushes.Blue; }
        public static Brush ToBeExplored { get => Brushes.MediumPurple; }
        public static Brush Wall { get => Brushes.SlateGray; }
        public static Brush Goal { get => Brushes.LightGreen; }
        public static Brush Empty { get => Brushes.White; }
        public static Brush Path { get => Brushes.Yellow; }
        public static Brush StartNode { get => Brushes.Orange; }
    }
}
