using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace CosmicBlock.Blocks
{
    // Offsets use the board convention: x right, y down from bounding-box top-left.
    public sealed class BlockShape
    {
        public string Id { get; }
        public IReadOnlyList<Vector2Int> Cells { get; }
        public int Width { get; }
        public int Height { get; }

        public BlockShape(string id, params Vector2Int[] offsets)
        {
            if (string.IsNullOrEmpty(id) || offsets == null || offsets.Length == 0)
                throw new ArgumentException("A shape needs an id and at least one cell.");
            var copy = (Vector2Int[])offsets.Clone();
            var unique = new HashSet<Vector2Int>();
            int minX = int.MaxValue, minY = int.MaxValue, maxX = 0, maxY = 0;
            foreach (var cell in copy)
            {
                if (cell.x < 0 || cell.y < 0 || !unique.Add(cell))
                    throw new ArgumentException("Shape offsets must be non-negative and unique.");
                minX = Math.Min(minX, cell.x); minY = Math.Min(minY, cell.y);
                maxX = Math.Max(maxX, cell.x); maxY = Math.Max(maxY, cell.y);
            }
            if (minX != 0 || minY != 0) throw new ArgumentException("Shape bounds must start at (0,0).");
            Id = id; Width = maxX + 1; Height = maxY + 1;
            Cells = new ReadOnlyCollection<Vector2Int>(copy);
        }
    }
}
