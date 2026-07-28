using CodeBase.Core.Mathematics;

namespace CodeBase.Core.Factory
{
    public static class FactoryNodeExtensions
    {
        public static bool IsInRange(this FactoryNode center, FactoryNode target, int range)
        {
            Vector2Int rangeVector = Vector2Int.One * range;
            Vector2Int rightTop = center.Position + rangeVector;
            Vector2Int leftBottom = center.Position - rangeVector;

            return rightTop >= target.Position &&
                   leftBottom <= target.Position + target.Size;
        }
    }
}