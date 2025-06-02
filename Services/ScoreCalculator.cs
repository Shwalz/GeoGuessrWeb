namespace GeoGuessrWeb.Services
{
    public static class ScoreCalculator
    {
        public static int CalculateScore(double distance)
        {
            if (distance < 1) return 5000;
            if (distance < 10) return 4500;
            if (distance < 100) return 4000;
            if (distance < 500) return 3000;
            if (distance < 1000) return 2000;
            if (distance < 5000) return 1000;
            return 0;
        }
    }
}
