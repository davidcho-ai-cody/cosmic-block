using System;
namespace CosmicBlock.Core
{
    public static class ScoreRules
    {
        public const int PointsPerPlacedCell = 10;
        public const int PointsPerLine = 100;
        public const int ComboBonusStep = 50;
        public static int AddPlacement(int current, int placedCells, int clearedLines, int combo)
        {
            long bonus = clearedLines > 0 ? Math.Max(0L, (long)combo - 1) * ComboBonusStep : 0;
            long total = current + (long)placedCells * PointsPerPlacedCell + (long)clearedLines * PointsPerLine + bonus;
            return (int)Math.Min(int.MaxValue, total);
        }
    }
}
