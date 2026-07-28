using System.Collections.Concurrent;
using CodeBase.Core.Mathematics;

namespace CodeBase.Core.Factory.Grid
{
    public class GridMap
    {
        public ConcurrentDictionary<Vector2Int, FactoryNode> FactoryNodes { get; private set; } = new();

        public bool TryAddNode(FactoryNode node)
        {
            Vector2Int topRight = node.Position + node.Size;
            
            for(int x = node.Position.X; x < topRight.X; x++)
            for(int y = node.Position.Y; y < topRight.Y; y++)
            {
                if (FactoryNodes.TryGetValue(new Vector2Int(x, y), out _))
                    return false;
            }
            
            for(int x = node.Position.X; x < topRight.X; x++)
            for(int y = node.Position.Y; y < topRight.Y; y++)
                FactoryNodes.TryAdd(new Vector2Int(x, y), node);
            
            return true;
        }
    }
}