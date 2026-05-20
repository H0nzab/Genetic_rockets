namespace Genetic_rockets.Core
{
    public static class Config
    {
        // === Základní parametry simulace ===
        public static int PopulationSize = 500;
        public static int Lifespan = 300;
        public static double MaxForce = 0.5;
        public static double MutationRate = 0.01;

        // === Nastavení fitness ===
        public static double TargetBonus = 5.0;
        public static double CrashPenalty = 0.2;
    }
}