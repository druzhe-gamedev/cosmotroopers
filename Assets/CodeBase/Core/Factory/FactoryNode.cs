using CodeBase.Core.Mathematics;

namespace CodeBase.Core.Factory
{
    public abstract class ReceiverNode : FactoryNode
    {
        public abstract bool TryAccept(ItemTransfer transfer);

        protected ReceiverNode(Vector2Int position, Vector2Int size) : base(position, size) { }
    }
    
    public abstract class FactoryNode
    {
        public NodeName NodeName => NodeName.Create(this);
        public Vector2Int Position { get; protected set; }
        public Vector2Int Size { get; protected set; }
        public NodeRotation NodeRotation { get; protected set; }
        public Vector2Int LookDirection => NodeRotation.ToVector();

        protected FactoryNode(Vector2Int position, Vector2Int size) => (Position, Size) = (position, size);
        public abstract void Tick(float deltaTime); 
    }
}