using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MazeSolver
{
    public class Maze
    {

        public List<List<bool>> MazePuzzle;

        public Point InitialPosition;

        public Point GoalPosition;

        public Maze(List<List<bool>> maze, Point initialPosition, Point goalPosition)
        {
            MazePuzzle = maze;
            InitialPosition = initialPosition;
            GoalPosition = goalPosition;
        }

        public double[,] GetPuzzle(int mazeWidth, int mazeHeight)
        {
            double[,] HeuristicMap = new double[mazeWidth, mazeHeight];

            for(int x = 0; x < mazeWidth; x++)
            {
                for(int y = 0; y < mazeHeight; y++)
                {
                    HeuristicMap[x, y] = Math.Sqrt(((x - GoalPosition.X) * (x - GoalPosition.X)) + ((y - GoalPosition.Y) * (y - GoalPosition.Y)));
                }
            }

            return HeuristicMap;
        }
    }
}
