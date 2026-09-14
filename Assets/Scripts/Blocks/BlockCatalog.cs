using System;
using System.Collections.Generic;
using UnityEngine;

namespace CosmicBlock.Blocks
{
    public static class BlockCatalog
    {
        public static IReadOnlyList<BlockShape> Shapes { get; } = Array.AsReadOnly(new[]
        {
            new BlockShape("single", new Vector2Int(0, 0)),
            new BlockShape("horizontal_2", new Vector2Int(0, 0), new Vector2Int(1, 0)),
            new BlockShape("horizontal_3", new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0)),
            new BlockShape("vertical_2", new Vector2Int(0, 0), new Vector2Int(0, 1)),
            new BlockShape("vertical_3", new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2)),
            new BlockShape("square_2", new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1)),
            new BlockShape("l_small", new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1)),
            new BlockShape("reverse_l_small", new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1))
        });
    }

    public sealed class BlockGenerator
    {
        private readonly System.Random random;
        public BlockGenerator(int? seed = null)
        { random = seed.HasValue ? new System.Random(seed.Value) : new System.Random(); }
        public BlockShape Next() => BlockCatalog.Shapes[random.Next(BlockCatalog.Shapes.Count)];
    }
}
