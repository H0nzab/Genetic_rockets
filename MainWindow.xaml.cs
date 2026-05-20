using Genetic_rockets.Core.Entities;
using Genetic_rockets.Core.Simulation;
using Genetic_Rockets.Core.Genetics;
using System;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using System.Windows.Shapes;

namespace Genetic_rockets
{
    public partial class MainWindow : Window
    {
        private World _world;
        private Population _population;

        private int _tick = 0;
        private readonly int _lifespan = 300;

        private readonly List<int> _successHistory = new List<int>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeSimulation();

            CompositionTarget.Rendering += GameLoop;
        }

        private void InitializeSimulation()
        {
            Point target = new Point(400, 50);

            _world = new World(800, 600, target);

            _world.AddObstacle(250, 300, 300, 20);

            Point startPoint = new Point(400, 550);

            double mutation_rate = 0.01;
            _population = new Population(150, _lifespan, 0.5, mutation_rate, startPoint);

            MainRenderer.SimWorld = _world;
            MainRenderer.SimPopulation = _population;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (_tick < _lifespan)
            {
                int currentSuccessCount = 0;

                foreach (var rocket in _population.Rockets)
                {
                    rocket.Update(_tick);

                    if (_world.IsCollision(rocket.Position)) _world.IsCollision(rocket.Position);

                    if (_world.IsCollision(rocket.Position)) rocket.IsDead = true;
                    if (_world.IsAtTarget(rocket.Position)) rocket.HasCompleted = true;

                    if (rocket.HasCompleted) currentSuccessCount++;
                }
                SuccessText.Text = $"V cíli: {currentSuccessCount}";
                _tick++;
            }
            // Vyhodnocení a evoluce
            else
            {
                int finalSuccessCount = _population.Rockets.Count(r => r.HasCompleted);

                _successHistory.Add(finalSuccessCount);

                UpdateGraph();

                _population.Evaluate(_world.Target);
                _population.Selection();

                MainRenderer.SimPopulation = _population;
                _tick = 0;
            }

            GenerationText.Text = $"Generace: {_population.Generation}";
            TickText.Text = $"Životnost: {_tick} / {_lifespan}";

            MainRenderer.InvalidateVisual();
        }

        private void UpdateGraph()
        {
            GraphCanvas.Children.Clear();

            if (_successHistory.Count < 2) return;

            Polyline graphLine = new Polyline
            {
                Stroke = Brushes.LightGreen,
                StrokeThickness = 1.5
            };

            double canvasWidth = 150;
            double canvasHeight = 60;
            double maxRockets = 150;

            double stepX = canvasWidth / Math.Max(1, _successHistory.Count - 1);

            for (int i = 0; i < _successHistory.Count; i++)
            {
                // X roste lineárně s generacemi
                double x = i * stepX;

                // (aktuální_počet / max_možný_počet) * výška_grafu
                double y = canvasHeight - ((_successHistory[i] / maxRockets) * canvasHeight);

                graphLine.Points.Add(new Point(x, y));
            }

            GraphCanvas.Children.Add(graphLine);
        }
    }
}