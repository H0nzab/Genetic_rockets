using Genetic_rockets.Core.Genetics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Genetic_rockets.Core.Entities
{
    class Rocket
    {
        // Fyzikální vlastnosti
        public Point Position { get; private set; }
        public Vector Velocity { get; private set; }
        public Vector Acceleration { get; private set; }

        public DNA Dna { get; private set; }

        // Stavy rakety
        public double Fitness { get; private set; }
        public bool IsDead { get; set; }
        public bool HasCompleted { get; set; }

        public Rocket(Point startPosition, DNA dna)
        {
            Position = startPosition;
            Velocity = new Vector(0, 0);
            Acceleration = new Vector(0, 0);
            Dna = dna;

            IsDead = false;
            HasCompleted = false;
        }

        // F = m * a
        public void ApplyForce(Vector force)
        {
            Acceleration += force;
        }

        // Posun rakety
        public void Update(int tick)
        {
            if (IsDead || HasCompleted) return;

            // Přečteme instrukci z DNA pro aktuální krok a aplikujeme ji
            if (tick < Dna.Genes.Length)
            {
                ApplyForce(Dna.Genes[tick]);
            }

            Velocity += Acceleration;
            Position += Velocity; //Point + Vector = Point

            Acceleration = new Vector(0, 0);
        }

        // Výpočet fitness
        public void CalculateFitness(Point target)
        {
            // Vzdálenost od cíle
            double distance = (target - Position).Length;

            if (distance < 1) distance = 1;

            // Čím blíž, tím líp
            Fitness = 1.0 / Math.Pow(distance, 2);

            // Reward
            if (HasCompleted)
            {
                Fitness *= 10.0;
            }
            // Penalizace
            if (IsDead)
            {
                Fitness *= 0.1;
            }
        }
    }
}