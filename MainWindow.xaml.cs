using Genetic_rockets.Core;
using Genetic_rockets.Core.Entities;
using Genetic_rockets.Core.Simulation;
using Genetic_Rockets.Core;
using Genetic_Rockets.Core.Genetics;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace Genetic_rockets
{
    public partial class MainWindow : Window
    {
        private World _world;
        private Population _population;

        private int _tick = 0;

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

            _population = new Population(
            Config.PopulationSize,
            Config.Lifespan,
            Config.MaxForce,
            Config.MutationRate,
            startPoint
        );

            MainRenderer.SimWorld = _world;
            MainRenderer.SimPopulation = _population;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (_tick < Config.Lifespan)
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
            TickText.Text = $"Životnost: {_tick} / {Config.Lifespan}";

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
            double maxRockets = Config.PopulationSize;

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

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Textové soubory (*.txt)|*.txt|Všechny soubory (*.*)|*.*",
                FileName = $"SmartRockets_Export_{DateTime.Now:yyyyMMdd_HHmm}.txt",
                Title = "Uložit statistiky simulace"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    // === HLAVIČKA ===
                    sb.AppendLine("==================================================");
                    sb.AppendLine($"   MĚŘENÍ SIMULACE SMART ROCKETS - {DateTime.Now}");
                    sb.AppendLine("==================================================");
                    sb.AppendLine();

                    // === NASTAVENÍ CONFIGU ===
                    sb.AppendLine("--- AKTUÁLNÍ KONFIGURACE (HYPERPARAMETRY) ---");
                    sb.AppendLine($"Velikost populace:  {Config.PopulationSize}");
                    sb.AppendLine($"Životnost rakety:   {Config.Lifespan} ticků");
                    sb.AppendLine($"Maximální síla:     {Config.MaxForce}");
                    sb.AppendLine($"Míra mutace:        {Config.MutationRate * 100} %");
                    sb.AppendLine($"Bonus za cíl:       {Config.TargetBonus}x");
                    sb.AppendLine($"Penalizace za smrt: {Config.CrashPenalty}x");
                    sb.AppendLine();

                    // === GLOBÁLNÍ STATISTIKY ===
                    sb.AppendLine("--- SOUHRNNÉ VÝSLEDKY ---");
                    sb.AppendLine($"Celkový počet generací: {_successHistory.Count}");

                    if (_successHistory.Count > 0)
                    {
                        int maxSuccess = _successHistory.Max();
                        double avgSuccess = _successHistory.Average();

                        sb.AppendLine($"Nejlepší výsledek:      {maxSuccess} raket v cíli ({Math.Round((maxSuccess / (double)Config.PopulationSize) * 100, 1)} %)");
                        sb.AppendLine($"Průměrně v cíli:        {Math.Round(avgSuccess, 2)} raket na generaci");
                    }
                    sb.AppendLine();

                    // === DETAILNÍ LOG PO GENERACÍCH ===
                    sb.AppendLine("--- HISTORIE GENERACÍ ---");
                    sb.AppendLine("Generace\tPočet raket v cíli");

                    for (int i = 0; i < _successHistory.Count; i++)
                    {
                        sb.AppendLine($"{i + 1}\t{_successHistory[i]}");
                    }

                    File.WriteAllText(saveFileDialog.FileName, sb.ToString());

                    MessageBox.Show("Data byla úspěšně exportována!", "Export dokončen", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Při ukládání souboru došlo k chybě:\n{ex.Message}", "Chyba exportu", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}