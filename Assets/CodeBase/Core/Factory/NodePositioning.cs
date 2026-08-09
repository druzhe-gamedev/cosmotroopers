using CodeBase.Core.Mathematics;

namespace CodeBase.Core.Factory
{
    public struct NodePositioning
    {
        public Vector2Int Position { get; }
        public Vector2Int Size { get; }
        public NodeRotation NodeRotation { get; }

        public NodePositioning(Vector2Int position, Vector2Int size, NodeRotation nodeRotation)
        {
            Position = position;
            Size = size;
            NodeRotation = nodeRotation;
        }
    }
}