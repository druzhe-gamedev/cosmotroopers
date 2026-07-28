using System;
using CodeBase.Core.Mathematics;

namespace CodeBase.Core.Factory.Connection
{
    public record Connection(Port Emitter, Port Receiver) : ConnectionBase 
    {
        public bool TryPropagate(ItemOnBelt item)
        {
            if (Emitter.CanPropagate(item)) 
                return Emitter.TryPropagate(item);

            return false;
        }
    }

    public record ZeroConnection : ConnectionBase;

    public abstract record ConnectionBase;
    
    public class Port
    {
        public FactoryNode Owner { get; private set; }
        public Vector2Int Position { get; private set; }
        public Predicate<ItemOnBelt> Filter { get; set; } = _ => true;
        
        public virtual bool CanPropagate(ItemOnBelt item) => Filter(item);
        public virtual bool TryPropagate(ItemOnBelt item) => true;
    }
}