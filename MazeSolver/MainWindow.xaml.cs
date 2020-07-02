using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MazeSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const int MazeWidth = 187 + 1;
        private const int MazeHeight = 100 + 1;

        private Grid[,] maze;
        private List<Button> maze1D;
        private Stopwatch Stopwatch;
        private Timer timer;
        public event PropertyChangedEventHandler PropertyChanged;
        private int currentPosition = 0;
        private List<Point> mazePath;
        private MazeGenerator mazeGenerator;
        private List<Point> solution;
        private bool finalPath = false;
        private bool drawMaze = true;
        private int currentDrawMazePosition = 0;

        public MainWindow()
        {
            InitializeComponent();
            maze = new Grid[MazeWidth,MazeHeight];
            maze1D = new List<Button>();

            //Initialise timer
            Stopwatch = new Stopwatch();

            mazeGenerator = new MazeGenerator(MazeWidth, MazeHeight);
            
            var verticalStackPanel = new StackPanel();
            verticalStackPanel.Orientation = Orientation.Vertical;

            for (int y = 0; y < MazeHeight; y++)
            {
                var horizontalStackPanel = new StackPanel();
                horizontalStackPanel.Orientation = Orientation.Horizontal;

                for (int x = 0; x < MazeWidth; x++)
                {
                    var button = new Grid();
                    button.Width = 8;
                    button.Height = 8;
                    button.PreviewMouseDown += Button_Click;
                    button.Background = new SolidColorBrush(Colors.Black);
                    horizontalStackPanel.Children.Add(button);
                    maze[x, y] = button;
                }

                verticalStackPanel.Children.Add(horizontalStackPanel);
            }

            MainGrid.Children.Add(verticalStackPanel);
            
            mazeGenerator.generate();
            
            timer = new Timer(new TimerCallback((s) =>
            {
                try
                {
                    Dispatcher.Invoke(() => FillPathSquare());
                }
                catch
                {

                }
            }), null, 100, 1);
        }

        private void FillPathSquare()
        {
                if (drawMaze)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        if (currentDrawMazePosition < mazeGenerator.mapGeneration.Count())
                        {
                            var cell = mazeGenerator.mapGeneration[currentDrawMazePosition];
                            int x = cell.row;
                            int y = cell.col;

                            maze[x, y].Background = new SolidColorBrush(Colors.White);

                            currentDrawMazePosition++;
                        }
                        else
                        {
                            drawMaze = false;
                            currentDrawMazePosition = 0;
                            SetStartEndPosition();
                            Solve(null, null);
                        }
                    }
                }
                else
                {
                for (int i = 0; i < 100; i++)
                {
                    if (mazePath == null || currentPosition >= mazePath.Count)
                    {
                        if (finalPath)
                        {
                            finalPath = false;
                            Regenerate(null, null);
                        }
                        return;
                    }

                    var point = mazePath[currentPosition];
                    HSLColor color = new HSLColor((point.X / MazeWidth) * 255, 255, finalPath ? 90 : 190);
                    maze[(int)point.X, (int)point.Y].Background = new SolidColorBrush(color);
                    currentPosition++;

                    if (currentPosition >= mazePath.Count && !finalPath)
                    {
                        currentPosition = 0;
                        mazePath = solution;
                        finalPath = true;
                    }
                }
            }
        }

        private void SetStartEndPosition()
        {
            Random random = new Random();

            int x = 1;
            int y = 1;

            while((maze[x,y].Background as SolidColorBrush).Color == Colors.Black)
            {
                x = random.Next(MazeWidth);
                y = random.Next(MazeHeight);
            }

            maze[x, y].Background = new SolidColorBrush(Colors.Green);

            x = MazeWidth - 2;
            y = MazeHeight- 2;

            while ((maze[x, y].Background as SolidColorBrush).Color == Colors.Black)
            {
                x = random.Next(MazeWidth);
                y = random.Next(MazeHeight);
            }

            maze[x, y].Background = new SolidColorBrush(Colors.Blue);
        }

        private void Button_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if(((SolidColorBrush)(sender as Grid).Background).Color == Colors.Blue)
                {
                    (sender as Grid).Background = new SolidColorBrush(Colors.Green);
                }
                else if (((SolidColorBrush)(sender as Grid).Background).Color == Colors.Green)
                {
                    (sender as Grid).Background = new SolidColorBrush(Colors.LightGray);
                }
                else
                {
                    (sender as Grid).Background = new SolidColorBrush(Colors.Blue);
                }
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                (sender as Grid).Background = new SolidColorBrush(Colors.Black);
            }
            
        }

                private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Create a file to write to.
            string puzzle = "";

            for (int x = MazeWidth - 1; x >= 0; x--)
            {
                for (int y = MazeHeight - 1; y >= 0; y--)
                {
                    if ((maze[x, y].Background as SolidColorBrush).Color == Colors.Black)
                    {
                        puzzle = puzzle + 1 + " ";
                    }
                    else
                    {
                        puzzle = puzzle + 0 + " ";
                    }
                }
                puzzle = puzzle + "\n";
            }
            File.WriteAllText(Environment.CurrentDirectory + "/scenario.maze", puzzle);
        }

        private void Solve(object sender, RoutedEventArgs e)
        {
            Point startPos = GetStartPosition();
            Point endPos = GetEndPosition();

            Maze mazePuzzle = new Maze(mazeGenerator.GetMazeList(), startPos, endPos);

            AStar solver = new AStar(MazeWidth, MazeHeight);
            solver.Solve(mazePuzzle);
            var path = solver.GetExploredPaths();
            solution = solver.GetResult();
            mazePath = path;
            
            currentPosition = 0;
            finalPath = false;
        }

        private void ColourPath(List<Point> path)
        {
            foreach (var point in path)
            {
                maze[(int)point.X, (int)point.Y].Background = new SolidColorBrush(Colors.Purple);
                Console.WriteLine(point.X + " " + point.Y);
            }
        }

        private void StartPathAnimation()
        {
            
        }

        private Point GetEndPosition()
        {
            for (int i = 0; i < MazeWidth; i++)
            {
                for (int j = 0; j < MazeHeight; j++)
                {
                    if ((maze[i, j].Background as SolidColorBrush).Color == Colors.Blue)
                    {
                        return new Point(i, j);
                    }
                }
            }
            
            return new Point(1, 1);
        }

        private Point GetStartPosition()
        {
            for (int i = 0; i < MazeWidth; i++)
            {
                for (int j = 0; j < MazeHeight; j++)
                {
                    if ((maze[i, j].Background as SolidColorBrush).Color == Colors.Green)
                    {
                        return new Point(i, j);
                    }
                }
            }

            return new Point(-1, -1);
        }

        private List<List<bool>> GetMaze(string filePath)
        {
            List<List<bool>> maze = new List<List<bool>>();

            string readText = File.ReadAllText(filePath);

            string[] columns = readText.Split('\n');

            for (int x = 0; x < columns.Length - 1; x++)
            {
                string[] row = columns[x].Split(' ');

                maze.Add(new List<bool>());

                for (int i = 0; i < columns.Length - 1; i++)
                {
                    string element = row[i];
                    if (element.Equals("0"))
                    {
                        maze[x].Add(false);
                    }
                    else
                    {
                        maze[x].Add(true);
                    }
                }
            }
            Console.WriteLine(maze);
            return maze;

        }

        // Import the maze
        private void Import(object sender, RoutedEventArgs e)
        {
            var mazeFromFile = GetMaze(Environment.CurrentDirectory + "/scenario.maze");
            for (int x = 0; x < MazeWidth; x++)
            {
                for (int y = 0; y < MazeHeight; y++)
                {
                    if (mazeFromFile[x][y])
                    {
                        maze[x, y].Background = new SolidColorBrush(new Color());
                    }
                    else
                    {
                        maze[x, y].Background = new SolidColorBrush(Colors.Beige);
                    }
                }
            }
        }

        private void FirePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        private void Regenerate(object sender, RoutedEventArgs e)
        {
            mazeGenerator = new MazeGenerator(MazeWidth, MazeHeight);
            mazeGenerator.generate();
            
            var mazeMap = mazeGenerator.GetMaze();

            for (int y = 0; y < MazeHeight; y++)
            {
                for (int x = 0; x < MazeWidth; x++)
                {
                    maze[x, y].Background = new SolidColorBrush(Colors.Black);
                }
            }

            currentDrawMazePosition = 0;
            drawMaze = true;
        }
     }
}
