using System.Collections.Generic;
using System.Windows;
using Genetic_rockets.Core.Entities;

namespace Genetic_rockets.Core.Simulation
{
    public class World
    {
        public Point Target { get; private set; }
        public List<Obstacle> Obstacles { get; private set; }

        public double Width { get; private set; }
        public double Height { get; private set; }

        public World(double width, double height, Point target)
        {
            Width = width;
            Height = height;
            Target = target;
            Obstacles = new List<Obstacle>();
        }

        public void AddObstacle(double x, double y, double width, double height)
        {
            Obstacles.Add(new Obstacle(x, y, width, height));
        }

        // Kontrola kolize rakety
        public bool IsCollision(Point position)
        {
            if (position.X < 0 || position.X > Width || position.Y < 0 || position.Y > Height)
            {
                return true;
            }

            foreach (var obstacle in Obstacles)
            {
                if (obstacle.Contains(position))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsAtTarget(Point position, double targetRadius = 10)
        {
            return (Target - position).Length <= targetRadius;
        }
    }
}