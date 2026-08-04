using System.Collections.Concurrent;
using CodeBase.Core.Mathematics;

namespace CodeBase.Core.Factory.Grid
{
    public class GridMap
    {
        public ConcurrentDictionary<Vector2Int, FactoryNode> FactoryNodes { get; } = new();

        public bool CanSetup(Vector2Int position, Vector2Int size)
        {
            Vector2Int topRight = position + size;
            
            for(int x = position.X; x < topRight.X; x++)
            for(int y = position.Y; y < topRight.Y; y++)
            {
                if (FactoryNodes.TryGetValue(new Vector2Int(x, y), out _))
                    return false;
            }

            return true;
        }
        
        public bool TryAddNode(FactoryNode node)
        {
            if (!CanSetup(node.Position, node.Size))
                return false;
            
            Vector2Int topRight = node.Position + node.Size;
            
            for(int x = node.Position.X; x < topRight.X; x++)
            for(int y = node.Position.Y; y < topRight.Y; y++)
                FactoryNodes.TryAdd(new Vector2Int(x, y), node);
            
            return true;
        }
    }
}