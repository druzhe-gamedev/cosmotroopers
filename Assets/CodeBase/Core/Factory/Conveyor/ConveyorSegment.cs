using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CodeBase.Core.Factory.Grid;
using CodeBase.Core.Mathematics;
using UnityEngine;
using Vector2Int = CodeBase.Core.Mathematics.Vector2Int;

namespace CodeBase.Core.Factory.Conveyor
{
    public class ConveyorQueue
    {
        private byte _queue;

        public bool IsOccupied(byte n) => (_queue & (byte)(1 << n)) != 0;
        
        public bool TryOccupy(byte n)
        {
            if (IsOccupied(n))
                return false;

            _queue |= (byte)(1 << n);
            return true;
        }

        public void Off(byte n) => _queue &= (byte)~(1 << n);

        public override string ToString() => int.Parse(Convert.ToString(_queue, 2)).ToString("00000");
    }
    
    public class ConveyorSegment : ReceiverNode
    {
        public ObservableCollection<ItemOnBelt> Items { get; } = new();
        
        private readonly ConveyorQueue _mainQueue;
        private readonly ConveyorQueue _additionalQueue;

        public ConveyorQueue MainQueue => _mainQueue;
        public ConveyorQueue AdditionalQueue => _additionalQueue;

        private readonly Dictionary<ItemOnBelt, byte> _lastSteps = new();
        private readonly float _speed;
        private readonly byte _queueCapacity;
        private readonly byte _halfCapacity;
        private readonly float _itemSize;
        private readonly GridMap _gridMap;

        public ConveyorSegment(byte queueCapacity, float speed, NodeRotation nodeRotation, GridMap gridMap, 
                               Vector2Int position, Vector2Int size
        ) : base(position, size)
        {
            if (queueCapacity % 2 != 1 || queueCapacity < 3)
                throw new Exception("Capacity must be odd and equal or more than 3");
            
            _queueCapacity = queueCapacity;
            _halfCapacity = (byte)(queueCapacity / 2);
            _speed = speed;
            _itemSize = 1f / queueCapacity;
            
            _mainQueue = new ConveyorQueue();
            _additionalQueue = new ConveyorQueue();

            NodeRotation = nodeRotation;
            _gridMap = gridMap;
        }

        public override bool TryAccept(ItemTransfer transfer)
        {
            if (!this.IsInRange(transfer.Emitter, 1))
                return false;

            Vector2Int emitterCenter = transfer.Emitter.Position + transfer.Emitter.Size / 2;
            Vector2Int difference = Position - emitterCenter;
            
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
                
                if (!queue.TryOccupy(targetStep)) return false;

                _lastSteps[transfer.Item] = targetStep;
                Items.Add(transfer.Item);
                transfer.Item.X.SetClamped(xClamp);
                transfer.Item.Y.SetClamped(yClamp);
                return true;
            }
        }

        public override void Tick(float deltaTime)
        {
            for (byte i = 0; i < Items.Count; i++)
            {
                ItemOnBelt item = Items[i];
                byte lastStep = _lastSteps[item]; 
                byte nextStep = (byte)(lastStep + 1);
                float currentY = item.Y.Current.Value;
                float translation = _speed * deltaTime;

                if (currentY > 0.5f)
                    nextStep = (byte)(lastStep - 1);
                
                if (Mathf.Approximately(currentY, 0.5f))
                {
                    float target = (lastStep + 1) * _itemSize;
                    if (item.X.IsMax)
                    {
                        TryTransfer(item, i, lastStep);
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
                        _lastSteps[item] = nextStep;
                        _mainQueue.Off(lastStep);
                        _mainQueue.TryOccupy(nextStep);
                    }
                }
                else
                {
                    // direction
                    int sign = Math.Sign(_halfCapacity - lastStep);
                    
                    // (sign + 1) / 2 eliminates all values except 1 (because of indexing from 0)
                    // ReSharper disable once PossibleLossOfFraction
                    float target = (lastStep + (sign + 1) / 2) * _itemSize;
                    
                    if (sign == 1 && currentY < target ||
                        sign == -1 && currentY > target)
                        item.Y.AddClamped(translation * sign);
                    else
                    {
                        item.Y.SetClamped(target);
                        
                        bool isMigrating = nextStep == _halfCapacity;
                        bool isNextOccupied = (isMigrating &&
                                               _mainQueue.IsOccupied(_halfCapacity)) ||
                                               _additionalQueue.IsOccupied(nextStep);
                        if (isNextOccupied)
                            continue;

                        if(isMigrating)
                            item.Y.SetClamped(0.5f);
                        
                        _lastSteps[item] = nextStep;
                        _additionalQueue.Off(lastStep);
                        ConveyorQueue pickedQueue = isMigrating ? _mainQueue : _additionalQueue;
                        pickedQueue.TryOccupy(nextStep);
                    }
                }
            }
        }

        private bool TryTransfer(ItemOnBelt item, byte n, byte step)
        {
            Vector2Int targetPosition = Position + LookDirection;

            if (!_gridMap.FactoryNodes.TryGetValue(targetPosition, out FactoryNode node) ||
                node is not ReceiverNode receiverNode) return false;
            
            if (!receiverNode.TryAccept(new ItemTransfer(item, this))) return false;
            Items.RemoveAt(n);
            _lastSteps.Remove(item);
            _mainQueue.Off(step);
            return true;

        }
    }
}