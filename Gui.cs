namespace Robot_Navigation_Problem
{
    internal static class Gui
    {
        private static GridWindow? _gridWindow = null;

        public static bool GuiModeEnabled { get; private set; } = false;

        public static int DurationPerIteration
        {
            get => _gridWindow?.DurationPerIteration ?? 500;
        }
        
        public static void Run(Environment e)
        {
            GuiModeEnabled = true;
            if (_gridWindow == null)
                _gridWindow = new GridWindow(e);
            Application.Run(_gridWindow);
        }

        public static void ChangeCellColor(int x, int y, Brush color)
        {
            _gridWindow?.ChangeCellColor(x, y, color);
        }

        public static void ChangeCellColor(Coordinate coordinate, Brush color)
        {
            _gridWindow?.ChangeCellColor(coordinate.x, coordinate.y, color);
        }

        public static void IncrementIteration()
        {
            _gridWindow?.IncrementIteration();
        }

        public static void IncreaseNumberOfNodes(int number = 1)
        {
            _gridWindow?.IncreaseNumberOfNodes(number);
        }
    }

    //this class is used for preventing flickering when repainting the grid
    internal class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            DoubleBuffered = true;
        }
    }

    internal class GridWindow : Form
    {
        private const int GRID_CELLS_DIMENSION = 80;
        private readonly Environment _environment;
        private Rectangle[][] _gridCells;
        private Brush[][] _cellColors;

        //global elements
        private readonly DoubleBufferedPanel _gridPanel = new();
        private readonly FlowLayoutPanel _navbar = new();
        private readonly Button _searchButton = new();
        private readonly TextBox _durationTextBox = new();
        private readonly NumericUpDown _gridHeightNumericUpDown = new();
        private readonly NumericUpDown _gridWidthNumericUpDown = new();
        private readonly Button _changeGridDimensionButton = new();
        private readonly ComboBox _searchAlgorithmDropdown = new();
        private readonly ComboBox _cellBrushDropdown = new();
        private readonly Label _iterationLabel = new();
        private readonly Label _numberOfNodesInTreeLabel = new();

        //state of the search algorithm
        private bool _environmentIsNew = true;
        private int _durationPerIteration = 500;
        private int _numberOfIterations = 0;
        private int _numberOfNodes = 0;

        public int DurationPerIteration
        {
            get => _durationPerIteration;
        }

        public GridWindow(Environment e)
        {
            //initialize members
            _environment = e;
            _gridCells = new Rectangle[_environment.Height][];
            _cellColors = new Brush[_environment.Height][];

            //set up the gridcells
            for (int i = 0; i < _environment.Height; i++)
            {
                _gridCells[i] = new Rectangle[_environment.Width];
                _cellColors[i] = new Brush[_environment.Width];
                for (int j = 0; j < _environment.Width; j++)
                {
                    _gridCells[i][j] = new Rectangle(j * GRID_CELLS_DIMENSION, i * GRID_CELLS_DIMENSION, GRID_CELLS_DIMENSION, GRID_CELLS_DIMENSION);
                }
            }
            
            //set up cell colors
            ResetEnvironmentColor();

            //set up the window
            InitalizeWindow();
            InitializeNavbar();
            InitializeSearchAlgorithmDropdown();
            InitializeCellBrushDropdown();
            InitializeDurationTextBox();
            InitializeGridDimensionNumericUpDown();
            InitializeChangeGridDimensionButton();
            InitializeIterationLabel();
            InitializeNumberOfNodesInTreeLabel();
            InitializeSearchButton();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            _gridPanel.Invalidate();
        }

        private void ResetEnvironmentColor()
        {
            for (int i = 0; i < _environment.Height; i++)
            { 
                for (int j = 0; j < _environment.Width; j++)
                {
                    switch (_environment.GetNode(j, i).Type)
                    {
                        case NodeType.Empty:
                            _cellColors[i][j] = CustomBrush.Empty;
                            break;
                        case NodeType.Goal:
                            _cellColors[i][j] = CustomBrush.Goal;
                            break;
                        case NodeType.Robot:
                            _cellColors[i][j] = CustomBrush.CurrentNode;
                            break;
                        case NodeType.Wall:
                            _cellColors[i][j] = CustomBrush.Wall;
                            break;
                        default:
                            _cellColors[i][j] = CustomBrush.Empty;
                            break;
                    }
                }
            }
            Invalidate();
        }

        private void ResetWindow()
        {
            _environment.Reset();
            ResetEnvironmentColor();
            _environmentIsNew = true;

            _numberOfIterations = 0;
            _iterationLabel.Text = "Iteration: 0";

            _numberOfNodes = 0;
            _numberOfNodesInTreeLabel.Text = "Number of nodes in search tree: 0";

            _searchButton.Text = "Start";
        }

        private void EnableAllInputs()
        {
            SetInputsEnabled(true);
        }

        private void DisableAllInputs()
        {
            SetInputsEnabled(false);
        }

        private void SetInputsEnabled(bool enabled)
        {
            _searchButton.Enabled = enabled;
            _durationTextBox.Enabled = enabled;
            _searchAlgorithmDropdown.Enabled = enabled;
            _cellBrushDropdown.Enabled = enabled;
            _gridWidthNumericUpDown.Enabled = enabled;
            _gridHeightNumericUpDown.Enabled = enabled;
            _changeGridDimensionButton.Enabled = enabled;
        }

        private void InitalizeWindow()
        {
            _gridPanel.Width = _environment.Width * GRID_CELLS_DIMENSION;
            _gridPanel.Height = _environment.Height * GRID_CELLS_DIMENSION;
            _gridPanel.Paint += (sender, e) =>
            {
                Pen pen = new Pen(Color.Black, 3.0f);
                for (int i = 0; i < _environment.Height; i++)
                {
                    for (int j = 0; j < _environment.Width; j++)
                    {
                        e.Graphics.FillRectangle(_cellColors[i][j], _gridCells[i][j]);
                        e.Graphics.DrawRectangle(pen, _gridCells[i][j]);
                        e.Graphics.DrawString($"{_environment.GetNode(j, i).X}, {_environment.GetNode(j, i).Y}", new Font("Arial", 10), Brushes.Black, new PointF(_gridCells[i][j].X + 30, _gridCells[i][j].Y + 30));
                    }
                }
            };
            //add on click listener to grid panel to change node type
            _gridPanel.MouseClick += OnWindowClickEventHandler;

            Text = "Robot Navigation Problem";
            Width = 1000;
            Height = 800;
            AutoScroll = true;

            Controls.Add(_gridPanel);
        }

        private void InitializeNavbar()
        {
            _navbar.Location = new Point(Width, 0);
            _navbar.MinimumSize = new Size(250, 500);
            _navbar.BackColor = Color.LightGray;
            _navbar.Padding = new Padding(10);
            _navbar.Dock = DockStyle.Right;
            _navbar.FlowDirection = FlowDirection.TopDown;

            //adjust window size after adding navbar
            Width += _navbar.Width;
            Height = Math.Max(_navbar?.Height ?? 0, Height);

            Controls.Add(_navbar);
        }

        private void InitializeSearchAlgorithmDropdown()
        {
            Label searchAlgorithmDropdownLabel = new Label
            {
                Text = "Search Algorithm:",
                AutoSize = true
            };
            _searchAlgorithmDropdown.Items.AddRange(new object[] { "dfs", "bfs", "gbfs", "as", "ucs", "jps" });
            _searchAlgorithmDropdown.SelectedIndex = 0;

            _navbar.Controls.Add(searchAlgorithmDropdownLabel);
            _navbar.Controls.Add(_searchAlgorithmDropdown);
        }

        private void InitializeCellBrushDropdown()
        {
            Label cellBrushDropdownLabel = new Label
            {
                Text = "Cell Brush Type: ",
                AutoSize = true
            };
            _cellBrushDropdown.Items.AddRange(new object[] { "wall", "start", "goal", "eraser" });
            _cellBrushDropdown.SelectedIndex = 0;

            _navbar.Controls.Add(cellBrushDropdownLabel);
            _navbar.Controls.Add(_cellBrushDropdown);
        }

        private void InitializeDurationTextBox()
        {

            Label durationTextBoxLabel = new Label
            {
                Text = "Iteration Duration (ms):",
                Margin = new Padding(0, 10, 0, 0),
                AutoSize = true
            };

            _durationTextBox.Text = _durationPerIteration.ToString();

            //onchange listener to prevent user from entering non number characters
            _durationTextBox.KeyPress += (sender, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
                else
                {
                    if(_durationTextBox.Text.Length == 0)
                        _durationPerIteration = 0;
                    else
                        _durationPerIteration = int.Parse(_durationTextBox.Text);
                }
            };

            _navbar.Controls.Add(durationTextBoxLabel);
            _navbar.Controls.Add(_durationTextBox);
        }

        private void InitializeGridDimensionNumericUpDown()
        {
            _gridWidthNumericUpDown.Minimum = 2;
            _gridWidthNumericUpDown.Maximum = int.MaxValue;
            _gridWidthNumericUpDown.Value = _environment.Width;
            Label gridWidthLabel = new Label()
            {
                Text = $"Number of columns ({_gridWidthNumericUpDown.Minimum} - {_gridWidthNumericUpDown.Maximum}):",
                AutoSize = true,
                Margin = new Padding(0, 20, 0, 0)
            };

            _gridHeightNumericUpDown.Minimum = 2;
            _gridHeightNumericUpDown.Maximum = int.MaxValue;
            _gridHeightNumericUpDown.Value = _environment.Height;
            Label gridHeightLabel = new Label()
            {
                Text = $"Number of columns ({_gridHeightNumericUpDown.Minimum} - {_gridHeightNumericUpDown.Maximum}):",
                AutoSize = true
            };

            _navbar.Controls.Add(gridWidthLabel);
            _navbar.Controls.Add(_gridWidthNumericUpDown);
            _navbar.Controls.Add(gridHeightLabel);
            _navbar.Controls.Add(_gridHeightNumericUpDown);
        }

        private void InitializeChangeGridDimensionButton()
        {
            _changeGridDimensionButton.Text = "Change Grid Dimension";
            _changeGridDimensionButton.AutoSize = true;
            _changeGridDimensionButton.BackColor = Color.LightBlue;

            //add onclick listener to the button to change grid dimensions
            _changeGridDimensionButton.Click += (sender, e) =>
            {
                (int width, int height) = ((int)_gridWidthNumericUpDown.Value, (int)_gridHeightNumericUpDown.Value);
                //check if the new grid is shorter/narrower than the old one, if yes, display a warning message that some cells will be deleted
                if (width < _environment.Width || height < _environment.Height)
                {
                    DialogResult dialogResult = MessageBox.Show("Changing the grid to a smaller dimension may result in some cells being deleted. Do you want to continue?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (dialogResult != DialogResult.OK)
                        return;
                }    
                _environment.ChangeDimension(width, height);
                _gridCells = new Rectangle[_environment.Height][];
                _cellColors = new Brush[_environment.Height][];
                for (int i = 0; i < _environment.Height; i++)
                {
                    _gridCells[i] = new Rectangle[_environment.Width];
                    _cellColors[i] = new Brush[_environment.Width];
                    for (int j = 0; j < _environment.Width; j++)
                    {
                        _gridCells[i][j] = new Rectangle(j * GRID_CELLS_DIMENSION, i * GRID_CELLS_DIMENSION, GRID_CELLS_DIMENSION, GRID_CELLS_DIMENSION);
                    }
                }
                ResetEnvironmentColor();
                _gridPanel.Width = _environment.Width * GRID_CELLS_DIMENSION;
                _gridPanel.Height = _environment.Height * GRID_CELLS_DIMENSION;
            };

            _navbar.Controls.Add(_changeGridDimensionButton);
        }

        private void InitializeIterationLabel()
        {
            _iterationLabel.Text = "Iteration: 0";
            _iterationLabel.Margin = new Padding(0, 20, 0, 0);
            _iterationLabel.AutoSize = true;

            _navbar.Controls.Add(_iterationLabel);
        }

        private void InitializeNumberOfNodesInTreeLabel()
        {
            _numberOfNodesInTreeLabel.Text = "Number of nodes in search tree: 0";
            _numberOfNodesInTreeLabel.Margin = new Padding(0, 10, 0, 0);
            _numberOfNodesInTreeLabel.AutoSize = true;

            _navbar.Controls.Add(_numberOfNodesInTreeLabel);
        }

        private void InitializeSearchButton()
        {
            _searchButton.Text = "Search";
            _searchButton.Size = new Size(100, 50);
            _searchButton.BackColor = Color.LightBlue;
            _searchButton.Dock = DockStyle.Bottom;
            _searchButton.Margin = new Padding(0, 20, 0, 0);

            //add onclick listener to the button to start search algorithm
            _searchButton.Click += OnSearchButtonClickEventHandler;            
            _navbar.Controls.Add(_searchButton);
        }
        
        private void OnWindowClickEventHandler(object? sender, MouseEventArgs e)
        {
            //if click outside the grid, dont do anything
            if (e.X < 0 || e.X > _environment.Width * GRID_CELLS_DIMENSION || e.Y < 0 || e.Y > _environment.Height * GRID_CELLS_DIMENSION)
                return;
            //do not allow user to change start/goal cell while searching/environment is not new (search algorithm has been run)
            if (!_environmentIsNew)
            {
                MessageBox.Show("Please reset the environment before changing the cell types.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //get coordinate of cell on clicked position
            (int x, int y) = (e.X / GRID_CELLS_DIMENSION, e.Y / GRID_CELLS_DIMENSION);
            NodeType nodeTypeToChangeTo;
            switch (_cellBrushDropdown?.SelectedItem?.ToString())
            {
                case "wall":
                    nodeTypeToChangeTo = NodeType.Wall;
                    break;
                case "start":
                    nodeTypeToChangeTo = NodeType.Robot;
                    break;
                case "goal":
                    nodeTypeToChangeTo = NodeType.Goal;
                    break;
                case "eraser":
                    nodeTypeToChangeTo = NodeType.Empty;
                    break;
                default:
                    MessageBox.Show("Invalid cell brush", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }
            try
            {
                _environment.ChangeCellType(x, y, nodeTypeToChangeTo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ResetEnvironmentColor();
        }

        private void OnSearchButtonClickEventHandler(object? sender, EventArgs e)
        {
            //disable input elements to prevent changing input while searching
            DisableAllInputs();
            //temporary disable changing cell type until search is done
            _gridPanel.MouseClick -= OnWindowClickEventHandler;

            //if environment is new (search algorithm hasnt been run), run the search algorithm, else reset the environment
            if (_environmentIsNew)
            {
                _searchButton.Text = "Finding...";
                _environmentIsNew = false;
                string path = "";
                _durationPerIteration = _durationTextBox.Text == "" ? 0 : int.Parse(_durationTextBox.Text);
                switch (_searchAlgorithmDropdown?.SelectedItem?.ToString())
                {
                    case "dfs":
                        DepthFirstSearch dfs = new DepthFirstSearch(_environment);
                        path = dfs.Search();
                        break;
                    case "bfs":
                        BreadthFirstSearch bfs = new BreadthFirstSearch(_environment);
                        path = bfs.Search();
                        break;
                    case "gbfs":
                        GreedyBestFirstSearch gbfs = new GreedyBestFirstSearch(_environment);
                        path = gbfs.Search();
                        break;
                    case "as":
                        AStarSearch astar = new AStarSearch(_environment);
                        path = astar.Search();
                        break;
                    case "ucs":
                        UniformCostSearch ucs = new UniformCostSearch(_environment);
                        path = ucs.Search();
                        break;
                    case "jps":
                        JumpPointSearch jps = new JumpPointSearch(_environment);
                        path = jps.Search();
                        break;
                    default:
                        MessageBox.Show("Invalid algorithm", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        EnableAllInputs();
                        _searchButton.Text = "Start";
                        return;
                }
                _searchButton.Text = "Reset";
                MessageBox.Show(path, path == SearchAlgorithm.NOT_FOUND ? "No path found" : "Path found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                ResetWindow();

            EnableAllInputs();
            _gridPanel.MouseClick += OnWindowClickEventHandler;
       
        }
        
        public void ChangeCellColor(int x, int y, Brush color)
        {
            _cellColors[y][x] = color;
            Invalidate();
        }

        public void IncrementIteration()
        {
            _numberOfIterations++;
            _iterationLabel.Text = $"Iteration: {_numberOfIterations}";
        }

        public void IncreaseNumberOfNodes(int numberOfNodes)
        {
            _numberOfNodes += numberOfNodes;
            _numberOfNodesInTreeLabel.Text = $"Number of nodes in search tree: {_numberOfNodes}";
        }
    }
}
