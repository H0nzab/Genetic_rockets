using System.Windows;

namespace Genetic_rockets.Core.Entities
{
    public class Obstacle
    {
        public Rect Bounds { get; private set; }

        public Obstacle(double x, double y, double width, double height)
        {
            Bounds = new Rect(x, y, width, height);
        }

        public bool Contains(Point point)
        {
            return Bounds.Contains(point);
        }
    }
}