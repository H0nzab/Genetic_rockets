using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Genetic_rockets.Core.Genetics
{
    public class DNA
    {
        // Samotné geny
        // Vektor je vestavěný
        public Vector[] Genes { get; private set; }

        private static readonly Random _random = new Random();

        private readonly double _maxForce;

        public DNA(int lifespan, double maxForce)
        {
            _maxForce = maxForce;
            Genes = new Vector[lifespan];

            for (int i = 0; i < lifespan; i++)
            {
                Genes[i] = GenerateRandomVector();
            }
        }

        private DNA(Vector[] newGenes, double maxForce)
        {
            Genes = newGenes;
            _maxForce = maxForce;
        }

        private Vector GenerateRandomVector()
        {
            double angle = _random.NextDouble() * Math.PI * 2;
            return new Vector(Math.Cos(angle) * _maxForce, Math.Sin(angle) * _maxForce);
        }

        // Křížení DNA
        public DNA Crossover(DNA partner)
        {
            Vector[] childGenes = new Vector[Genes.Length];

            // Náhodně zvolíme bod zlomu
            //int midpoint = _random.Next(Genes.Length);

            for (int i = 0; i < Genes.Length; i++)
            {
                if (_random.NextDouble() < 0.5)
                {
                    childGenes[i] = Genes[i];
                }
                else
                {
                    childGenes[i] = partner.Genes[i];
                }
            }

            return new DNA(childGenes, _maxForce);
        }

        public void Mutate(double mutationRate)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                if (_random.NextDouble() < mutationRate)
                {
                    Genes[i] = GenerateRandomVector();
                }
            }
        }
    }
}