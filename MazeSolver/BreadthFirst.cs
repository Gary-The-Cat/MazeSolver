using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MazeSolver
{
    public class BreadthFirst : IMazeSolver
    {
        private Node currentPosition;

        private Point goalPosiiton;

        private List<Node> frontier;

        private List<Point> resultPath;

        private readonly int MazeWidth;
        
        private readonly int MazeHeight;

        public BreadthFirst(int mazeWidth, int mazeHeight)
        {
            MazeWidth = mazeWidth;
            MazeHeight = mazeHeight;
            
            frontier = new List<Node>();
            resultPath = new List<Point>();
        }

        public List<Point> GetResult()
        {
            return resultPath;
        }

        public void Solve(Maze maze)
        {
            currentPosition = new Node
            {
                Position = maze.InitialPosition,
                Parent = null
            };

            // While we are not at the goal position
            while(currentPosition.Position.X != maze.GoalPosition.X && currentPosition.Position.Y != maze.GoalPosition.Y)
            {

            }
        }
    }
}
