using System.Collections.Generic;
using UnityEngine;

namespace CosmicBlock.Board
{
    public sealed class LineClearResult
    {
        public static readonly LineClearResult Empty = new LineClearResult(new List<int>(), new List<int>(), new List<Vector2Int>());
        public IReadOnlyList<int> ClearedRows { get; }
        public IReadOnlyList<int> ClearedColumns { get; }
        public IReadOnlyList<Vector2Int> UniqueClearedCells { get; }
        public int LineCount => ClearedRows.Count + ClearedColumns.Count;
        public LineClearResult(List<int> rows, List<int> columns, List<Vector2Int> cells)
        {
            ClearedRows = rows.ToArray();
            ClearedColumns = columns.ToArray();
            UniqueClearedCells = cells.ToArray();
        }
    }
}
