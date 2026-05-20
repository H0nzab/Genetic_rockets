using Genetic_rockets.Core.Entities;
using Genetic_rockets.Core.Genetics;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Genetic_Rockets.Core.Genetics
{
    public class Population
    {
        public List<Rocket> Rockets { get; private set; }
        public int Generation { get; private set; }

        private readonly int _populationSize;
        private readonly int _lifespan;
        private readonly double _maxForce;
        private readonly double _mutationRate;
        private readonly Point _startPoint;

        private static readonly Random _random = new Random();

        public Population(int size, int lifespan, double maxForce, double mutationRate, Point startPoint)
        {
            _populationSize = size;
            _lifespan = lifespan;
            _maxForce = maxForce;
            _mutationRate = mutationRate;
            _startPoint = startPoint;
            Generation = 1;

            Rockets = new List<Rocket>();

            // Inicializace první generace s náhodnou DNA
            for (int i = 0; i < _populationSize; i++)
            {
                Rockets.Add(new Rocket(_startPoint, new DNA(_lifespan, _maxForce)));
            }
        }

        // Ohodnocení celé populace na konci jejího života
        public void Evaluate(Point target)
        {
            foreach (var rocket in Rockets)
            {
                rocket.CalculateFitness(target);
            }
        }

        // Vytvoření nové generace
        public void Selection()
        {
            var newRockets = new List<Rocket>();

            // fitness populace
            double totalFitness = 0;
            Rocket bestRocket = Rockets[0];

            foreach (var rocket in Rockets)
            {
                totalFitness += rocket.Fitness;
                if (rocket.Fitness > bestRocket.Fitness)
                {
                    bestRocket = rocket;
                }
            }

            newRockets.Add(new Rocket(_startPoint, bestRocket.Dna));

            // křížení
            for (int i = 1; i < _populationSize; i++)
            {
                DNA parentA = SelectParent(totalFitness);
                DNA parentB = SelectParent(totalFitness);

                DNA childDna = parentA.Crossover(parentB);
                childDna.Mutate(_mutationRate);

                newRockets.Add(new Rocket(_startPoint, childDna));
            }

            Rockets = newRockets;
            Generation++;
        }

        private DNA SelectParent(double totalFitness)
        {
            double randomValue = _random.NextDouble() * totalFitness;
            double runningSum = 0;

            foreach (var rocket in Rockets)
            {
                runningSum += rocket.Fitness;

                if (runningSum >= randomValue)
                {
                    return rocket.Dna;
                }
            }

            return Rockets[_random.Next(Rockets.Count)].Dna;
        }
    }
}