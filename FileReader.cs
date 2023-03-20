namespace Robot_Navigation_Problem
{
    internal static class FileReader
    {
        public static Environment ConstructEnvironmentFromFileName(string filename)
        {
            //this text array will hold all the lines of the file (each line is an element of the array)
            string[] text = Array.Empty<string>();

            try
            {
                text = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
            }
            catch(Exception e)
            {
                Console.WriteLine("Error reading " + filename + ": " + e.Message);
                System.Environment.Exit(2); //ERROR_FILE_NOT_FOUND (https://learn.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-)
            }

            //extract the metadata of the environment as string
            string gridSizeString = text[0];
            string robotLocString = text[1];
            string[] goalLocString = text[2].Replace(" ", "").Split("|");
            string[] wallLocString = text.Skip(3).ToArray();

            //convert the metadata of the environment to the correct data type
            (int, int) gridSize = StringToTuple(gridSizeString);
            Coordinate robotLoc = new Coordinate(StringToTuple(robotLocString));
            List<Coordinate> goalLocs = goalLocString.Select(locString => new Coordinate(StringToTuple(locString))).ToList();
            List<Coordinate> wallLocs = ConstructWalls(wallLocString);

            //return the environment constructed from the metadata
            return new Environment(gridSize.Item2, gridSize.Item1, robotLoc, goalLocs, wallLocs);
        }

        private static (int, int) StringToTuple(string s)
        {
            //remove first and last char of string
            string bracketsRemoved = s.Substring(1, s.Length - 2);
            //split the string in comma
            string[] valuesAsString = bracketsRemoved.Split(',');
            //parse the string to an int tuple and return it
            return (int.Parse(valuesAsString[0]), int.Parse(valuesAsString[1]));
        }

        private static List<Coordinate> ConstructWalls(string[] wallLocString)
        {
            //this list will hold all the walls of the environment, each element of the list is a wall cell
            List<Coordinate> wallLocs = new List<Coordinate>();
            //remove brackets
            wallLocString = wallLocString.Select(str => str.Substring(1, str.Length - 2)).ToArray();
            //split into 4 numbers
            (int, int, int, int)[] walls = wallLocString.Select(str => str.Split(',')).Select(str => (int.Parse(str[0]), int.Parse(str[1]), int.Parse(str[2]), int.Parse(str[3]))).ToArray();
            //construct walls
            foreach ((int, int, int, int) wall in walls)
            {
                int x = wall.Item1;
                int y = wall.Item2;
                int width = wall.Item3;
                int height = wall.Item4;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        wallLocs.Add(new Coordinate(x + i, y + j));
                    }
                }
            }
            return wallLocs;
        }
    }
}
