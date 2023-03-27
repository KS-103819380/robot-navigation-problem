namespace Robot_Navigation_Problem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //check for correct number of arguments
                if (args.Length != 2)
                {
                    Console.WriteLine("Invalid number of arguments");
                    System.Environment.Exit(160); //ERROR_BAD_ARGUMENTS (https://learn.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-)
                }

                //extract the data from the arguments
                string filename = args[0];
                string method = args[1].ToLower();

                //construct environment
                Environment e = FileReader.ConstructEnvironmentFromFileName(filename);

                //create the search algorithm object
                SearchAlgorithm? searchAlgorithm = null;

                //instantiate different types of search algorithm object depending on the method passed
                switch (method)
                {
                    case "dfs":
                        searchAlgorithm = new DepthFirstSearch(e);
                        break;
                    case "bfs":
                        searchAlgorithm = new BreadthFirstSearch(e);
                        break;
                    case "gbfs":
                        searchAlgorithm = new GreedyBestFirstSearch(e);
                        break;
                    case "as":
                        searchAlgorithm = new AStarSearch(e);
                        break;
                    case "cus1":
                    case "ucs":
                        searchAlgorithm = new UniformCostSearch(e);
                        break;
                    case "gui":
                        Gui.Run(e);
                        break;
                    default:
                        Console.WriteLine("Invalid method");
                        System.Environment.Exit(160); //ERROR_BAD_ARGUMENTS (https://learn.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-)
                        break;
                }

                //if gui is not selected, print search result to console
                if (searchAlgorithm == null) return;
                //perform the search
                string path = searchAlgorithm.Search();
                //print arguments passed and number of nodes in the search tree
                Console.WriteLine(filename + " " + args[1] + " " + searchAlgorithm.NumberOfNodes);
                //print the path
                Console.WriteLine(path);
            }
            catch(Exception e) // catch all exceptions thrown by the program (whether it is an expected exception or not)
            {
                Console.WriteLine("Error: " + e.Message + " Please try again.");
                System.Environment.Exit(1); //ERROR_INVALID_FUNCTION (https://learn.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-)
            }
        }
    }
}