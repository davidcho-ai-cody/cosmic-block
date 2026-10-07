using System;
namespace CosmicBlock.Core
{
    public static class ScoreRules
    {
        public const int PointsPerPlacedCell = 10;
        public const int PointsPerLine = 100;
        public const int ComboBonusStep = 50;
        public static int LinePoints(int index) => (int)Math.Min(int.MaxValue, PointsPerLine + Math.Max(0L,(long)index-1)*ComboBonusStep);
        public static int AddLine(int current,int index) => (int)Math.Min(int.MaxValue,(long)current+LinePoints(index));
        public static int AddPlacement(int current, int placedCells, int clearedLines, int combo)
        {
            // combo is retained for source compatibility; placement streak no longer adds points.
            long lines = Math.Max(0L, clearedLines);
            long lineTotal = lines >= 10000 ? int.MaxValue : lines * PointsPerLine + lines * (lines - 1) / 2 * ComboBonusStep;
            long total = current + (long)placedCells * PointsPerPlacedCell + lineTotal;
            return (int)Math.Min(int.MaxValue, total);
        }
    }
}
