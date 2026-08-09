using CodeBase.Core.Factory.Grid;
using CodeBase.Core.Mathematics;

namespace CodeBase.Core.Factory
{
    public abstract class ReceiverNode : FactoryNode
    {
        public abstract bool TryAccept(ItemTransfer transfer);

        protected ReceiverNode(GridMap gridMap, NodePositioning nodePositioning) : base(gridMap, nodePositioning) { }
    }
    
    public abstract class FactoryNode
    {
        public NodeName NodeName => NodeName.Create(this);
        public Vector2Int Position { get; protected set; }
        public Vector2Int Size { get; protected set; }
        public NodeRotation NodeRotation { get; protected set; }
        public Vector2Int LookDirection => NodeRotation.ToVector();
        public GridMap GridMap { get; }

        protected FactoryNode(GridMap gridMap, NodePositioning nodePositioning) => (Position, Size, NodeRotation, GridMap) =
            (nodePositioning.Position, nodePositioning.Size, nodePositioning.NodeRotation, gridMap);
        
        public abstract void Tick(float deltaTime); 
    }
}