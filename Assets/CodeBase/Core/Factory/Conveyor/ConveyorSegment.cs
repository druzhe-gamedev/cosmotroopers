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
    
    public class ConveyorSegment : ConveyorBase
    {
        private readonly ConveyorQueue _mainQueue;
        private readonly ConveyorQueue _additionalQueue;

        public ConveyorSegment(byte queueCapacity, float speed, GridMap gridMap, NodePositioning nodePositioning) 
            : base(gridMap, nodePositioning, speed, queueCapacity)
        {
            _mainQueue = new ConveyorQueue();
            _additionalQueue = new ConveyorQueue();
        }

        public override bool TryAccept(ItemTransfer transfer)
        {
            if (!this.IsInRange(transfer.Emitter, 1))
                return false;

            Vector2Int emitterCenter = transfer.Emitter.Position + transfer.Emitter.Size / 2;
            Vector2Int difference = Position - emitterCenter;
            
            Vector2Int right = new (LookDirection.Y, -LookDirection.X);
            float side = difference.Dot(right);
            bool isRight = side < 1;
            int dotProduct = (int)difference.Dot(LookDirection);

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
                float yClamp = isBack ? 0.5f : isRight ? 1 : 0;
                byte targetStep = (byte)(!isBack && isRight ? QueueCapacity - 1 : 0);
                byte nextStep = (byte)(targetStep + 1);
                if (!isBack && isRight)
                    nextStep = (byte)(targetStep - 1);
                
                if (queue.IsOccupied(nextStep)) return false;

                queue.TryOccupy(targetStep);
                LastSteps[transfer.Item] = new(targetStep, isBack);
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
                ItemStep lastQueueStep = LastSteps[item];
                byte lastStep = lastQueueStep.Step;
                float translation = Speed * deltaTime;
                
                if (lastQueueStep.IsMainQueue)
                {
                    float target = lastStep * ItemSize;
                    byte nextStep = (byte)(lastStep + 1);
                    
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
                            
                        if(lastStep == QueueCapacity || _mainQueue.IsOccupied(nextStep))
                            continue;
                        
                        // reach target, increment step
                        LastSteps[item] = new ItemStep(nextStep, true);
                        _mainQueue.Off(lastStep);
                        _mainQueue.TryOccupy(nextStep);
                    }
                }
                else
                {
                    float currentY = item.Y.Current.Value;
                    int sign = Math.Sign(0.5f - currentY);
                    float target = lastStep * ItemSize;
                    
                    if ((sign == 1 && currentY < target) ||
                        (sign == -1 && currentY > target))
                    {
                        item.Y.AddClamped(translation * sign);
                        
                        if(Math.Sign(0.5f - item.Y.Current.Value) == -sign)
                            item.Y.SetClamped(0.5f);
                    }
                    else
                    {
                        item.Y.SetClamped(target);
                        byte nextStep = (byte)(lastStep + (currentY > 0.5 ? -1 : 1));

                        if (lastStep == HalfCapacity)
                        {
                            if (!_mainQueue.TryOccupy(nextStep))
                                continue;
                            
                            _mainQueue.Off(lastStep);
                            LastSteps[item] = new ItemStep(nextStep, true);
                            continue;
                        }
                        
                        bool isMigrating = nextStep == HalfCapacity;
                        bool isOccupied = (isMigrating && _mainQueue.IsOccupied(HalfCapacity)) ||
                                          (!isMigrating && _additionalQueue.IsOccupied(nextStep));
                            
                        if(isOccupied)
                            continue;

                        _additionalQueue.Off(lastStep);
                        ConveyorQueue pickedQueue = isMigrating ? _mainQueue : _additionalQueue;
                        pickedQueue.TryOccupy(nextStep);
                        LastSteps[item] = new ItemStep(nextStep, false);
                    }
                }
            }
        }

        private bool TryTransfer(ItemOnBelt item, byte n, byte step)
        {
            Vector2Int targetPosition = Position + LookDirection;

            if (!GridMap.FactoryNodes.TryGetValue(targetPosition, out FactoryNode node) ||
                node is not ReceiverNode receiverNode) return false;
            
            if (!receiverNode.TryAccept(new ItemTransfer(item, this))) return false;
            
            Items.RemoveAt(n);
            LastSteps.Remove(item);
            _mainQueue.Off(step);
            return true;
        }
    }
}