using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MazeSolver
{
    class Node
    {
        public Point Position { get; set; }

        public bool Obstacle { get; set; } = false;

        public double HeuristicCost = 0;

        public double PathCost = 0;

        public double TotalCost => HeuristicCost + PathCost;

        public Node Parent;

        public Node()
        {

        }

        public Node(Point position, double pathCost, double heuristicCost, Node parent)
        {
            Position = position;
            PathCost = pathCost;
            HeuristicCost = heuristicCost;
            Parent = parent;
        }

        public Node LazyClone()
        {
            return new Node
            {
                Parent = this.Parent,
                Position = this.Position
            };
        }


    }
}
