using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CodeBase.Core.Factory.Grid;

namespace CodeBase.Core.Factory.Conveyor
{
    public abstract class ConveyorBase : ReceiverNode
    {
        public ObservableCollection<ItemOnBelt> Items { get; } = new();
        
        protected readonly Dictionary<ItemOnBelt, ItemStep> LastSteps = new();
        protected readonly float Speed;
        protected readonly byte QueueCapacity;
        protected readonly byte HalfCapacity;
        protected readonly float ItemSize;

        public ConveyorBase(GridMap gridMap, NodePositioning nodePositioning, float speed, byte queueCapacity) : base(
            gridMap,
            nodePositioning
        )
        {
            if (queueCapacity % 2 != 1 || queueCapacity < 3)
                throw new Exception("Capacity must be odd and equal or more than 3");
            
            QueueCapacity = queueCapacity;
            HalfCapacity = (byte)(queueCapacity / 2);
            Speed = speed;
            ItemSize = 1f / (queueCapacity - 1);
        }
    }
}