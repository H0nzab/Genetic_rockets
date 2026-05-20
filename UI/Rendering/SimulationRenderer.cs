using Genetic_rockets.Core.Genetics;
using Genetic_rockets.Core.Simulation;
using Genetic_Rockets.Core.Genetics;
using System.Windows;
using System.Windows.Media;

namespace Genetic_rockets.UI.Rendering
{
    public class SimulationRenderer : FrameworkElement
    {
        public World SimWorld { get; set; }
        public Population SimPopulation { get; set; }

        private readonly SolidColorBrush _rocketBrush = Brushes.White;
        private readonly SolidColorBrush _deadRocketBrush = Brushes.Red;
        private readonly SolidColorBrush _completedRocketBrush = Brushes.LightGreen;
        private readonly SolidColorBrush _obstacleBrush = Brushes.DarkGray;
        private readonly SolidColorBrush _targetBrush = Brushes.Gold;
        private readonly Pen _rocketOutline = new Pen(Brushes.Black, 1);

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (SimWorld == null || SimPopulation == null) return;

            // obdélníky
            foreach (var obstacle in SimWorld.Obstacles)
            {
                dc.DrawRectangle(_obstacleBrush, null, obstacle.Bounds);
            }

            // cíl
            dc.DrawEllipse(_targetBrush, null, SimWorld.Target, 15, 15);

            // rakety
            foreach (var rocket in SimPopulation.Rockets)
            {
                SolidColorBrush currentBrush = _rocketBrush;

                // Barva podle stavu rakety
                if (rocket.HasCompleted) currentBrush = _completedRocketBrush;
                else if (rocket.IsDead) currentBrush = _deadRocketBrush;

                dc.DrawEllipse(currentBrush, _rocketOutline, rocket.Position, 4, 4);
            }
        }
    }
}