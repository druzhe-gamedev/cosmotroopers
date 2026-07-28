using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using CodeBase.Core.Factory.Grid;
using CodeBase.Core.Mathematics;

namespace CodeBase.Core.Factory.Conveyor
{
    class ConveyorQueue
    {
        private byte _queue;

        public ConveyorQueue(byte queue)
        {
            _queue = (byte)(queue > 7 ? 7 : queue);
        }
        
        public bool IsOccupied(byte n) => (_queue & (byte)(1 << n)) == 1;
        
        public bool TryOccupy(byte n)
        {
            if (IsOccupied(n))
                return false;

            _queue |= (byte)(1 << n);
            return true;
        }

        public void Off(byte n) => _queue &= (byte)~(1 << n);
    }
    
    public class ConveyorSegment : ReceiverNode
    {
        public ObservableCollection<ItemOnBelt> Items;
        
        private readonly ConveyorQueue _mainQueue;
        private readonly ConveyorQueue _additionalQueue;
        
        private readonly byte[] _lastSteps;
        private readonly float _speed;
        private readonly byte _queueCapacity;
        private readonly byte _halfCapacity;
        private const float _itemSize = 0.2f;
        private readonly GridMap _gridMap;

        public ConveyorSegment(byte queueCapacity, float speed, Vector2Int position, Vector2Int size,
                               NodeRotation nodeRotation, GridMap gridMap
        )
        {
            if (queueCapacity % 2 != 1 || queueCapacity < 3)
                throw new Exception("Capacity must be odd and more than 3");
            
            _queueCapacity = queueCapacity;
            _halfCapacity = (byte)(queueCapacity / 2);
            _speed = speed;
            _lastSteps = new byte[_queueCapacity];
            
            _mainQueue = new ConveyorQueue(queueCapacity);
            _additionalQueue = new ConveyorQueue(queueCapacity);

            Position = position;
            Size = size;
            NodeRotation = nodeRotation;
            _gridMap = gridMap;
        }

        public override bool TryAccept(ItemTransfer transfer)
        {
            if (!this.IsInRange(transfer.Emitter, 1))
                return false;

            Vector2Int emitterCenter = transfer.Emitter.Position + transfer.Emitter.Size / 2;
            Vector2Int difference = Position - emitterCenter;

            difference = difference.X > difference.Y
                ? new Vector2Int(Math.Sign(difference.X), 0)
                : new Vector2Int(0, Math.Sign(difference.Y));
            
            int dotProduct = (int)difference.Dot(LookDirection);
            bool isTargetBelow = Position.Y > emitterCenter.Y;

            // front
            if (dotProduct == -1) return false;

            return dotProduct switch
            {
                // back
                1 => AddToQueue(true),
                // left or right
                0 => AddToQueue(false),
                _ => false
            };

            bool AddToQueue(bool isBack)
            {
                ConveyorQueue queue = isBack ? _mainQueue : _additionalQueue;
                // set step of current item to 0 if it's coming from {0; 0.5} or {0.5; 0}
                // if position is {0.5; 1}, step = _queueCapacity -1
                float xClamp = isBack ? 0f : 0.5f;
                float yClamp = isBack ? 0.5f : isTargetBelow ? 1f : 0;
                byte targetStep = (byte)(isTargetBelow ? _queueCapacity - 1 : 0);
                
                if (queue.IsOccupied(targetStep)) return false;

                _lastSteps[Items.Count] = targetStep;
                Items.Add(transfer.Item);
                transfer.Item.X.SetClamped(xClamp);
                transfer.Item.Y.SetClamped(yClamp);
                return true;
            }
        }

        public override void Tick(float deltaTime)
        {
            float translation = _speed * deltaTime;

            for (byte i = 0; i < Items.Count; i++)
            {
                ItemOnBelt item = Items[i];
                byte lastStep = _lastSteps[i];
                byte nextStep = (byte)(lastStep < _halfCapacity ? lastStep + 1 : lastStep - 1);
                float currentY = item.Y.Current.Value;
                
                if (currentY == 0.5f)
                {
                    float target = (lastStep + 1) * _itemSize;
                    if (item.X.Current.Value >= 1)
                    {
                        item.X.SetClamped(1);
                        TryTransfer(item, i);
                        continue;
                    }
                    
                    // move to the target
                    if(item.X.Current.Value < target)
                        item.X.AddClamped(translation);
                    else
                    {
                        item.X.SetClamped(target);
                            
                        if(nextStep == _queueCapacity || _mainQueue.IsOccupied(nextStep))
                            continue;
                        
                        // reach target, increment step
                        _lastSteps[i] = nextStep;
                        _mainQueue.Off(lastStep);
                        _mainQueue.TryOccupy(nextStep);
                    }
                }
                else
                {
                    // direction
                    int sign = Math.Sign(_halfCapacity - lastStep);
                    
                    // clamp to center
                    if(sign == 1 && currentY > 0.5f ||
                       sign == -1 && currentY < 0.5f)
                    {
                        item.Y.SetClamped(0.5f);
                        continue;
                    }
                    // (sign + 1) / 2 eliminates all values except 1 (because of indexing from 0)
                    float target = (lastStep + (sign + 1) / 2) * _itemSize;
                    translation *= sign;
                    
                    if(sign == 1 && currentY < target ||
                       sign == -1 && currentY > target)
                        item.Y.AddClamped(translation);
                    else
                    {
                        item.Y.SetClamped(target);
                        
                        bool isMigrating = nextStep == _halfCapacity;
                        bool isNextOccupied = (isMigrating &&
                                               _mainQueue.IsOccupied(_halfCapacity)) ||
                                               _additionalQueue.IsOccupied(nextStep);
                        if (isNextOccupied)
                            continue;

                        _lastSteps[i] = nextStep;
                        _additionalQueue.Off(lastStep);
                        ConveyorQueue pickedQueue = isMigrating ? _mainQueue : _additionalQueue;
                        pickedQueue.TryOccupy(nextStep);
                    }
                }
            }
        }

        private bool TryTransfer(ItemOnBelt item, byte n)
        {
            Vector2Int targetPosition = Position + LookDirection;

            if (!_gridMap.FactoryNodes.TryGetValue(targetPosition, out FactoryNode node) ||
                node is not ReceiverNode receiverNode) return false;
            
            if (!receiverNode.TryAccept(new ItemTransfer(item, this))) return false;
            Items.RemoveAt(n);
            _mainQueue.Off(n);
            return true;

        }
    }
}