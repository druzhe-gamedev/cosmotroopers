using System;
using System.Collections.ObjectModel;
using CodeBase.Core.Factory.Grid;
using CodeBase.Core.Mathematics;
using UnityEngine;
using Vector2Int = CodeBase.Core.Mathematics.Vector2Int;

namespace CodeBase.Core.Factory.Conveyor
{
    public class TJunction : ConveyorBase
    {
        private readonly ConveyorQueue _mainQueue;
        private readonly ConveyorQueue _additionalQueue;
        private bool _directionFlag;

        public TJunction(byte queueCapacity, float speed, GridMap gridMap, NodePositioning nodePositioning) : base(
            gridMap,
            nodePositioning,
            speed,
            queueCapacity
        )
        {
            _mainQueue = new();
            _additionalQueue = new();
        }

        public override bool TryAccept(ItemTransfer transfer)
        {
            if (!this.IsInRange(transfer.Emitter, 1))
                return false;

            Vector2Int emitterCenter = transfer.Emitter.Position + transfer.Emitter.Size / 2;
            Vector2Int difference = Position - emitterCenter;
            
            int dotProduct = (int)difference.Dot(LookDirection);

            return dotProduct switch
            {
                // back
                1 => AddToQueue(),
                _ => false
            };

            bool AddToQueue()
            {
                if (_mainQueue.IsOccupied(1)) return false;

                LastSteps[transfer.Item] = new(0, true);
                Items.Add(transfer.Item);
                transfer.Item.X.SetClamped(0);
                transfer.Item.Y.SetClamped(0.5f);
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
                    
                    // move to the target
                    if(item.X.Current.Value < target)
                        item.X.AddClamped(translation);
                    else
                    {
                        item.X.SetClamped(target);
                        
                        if (lastStep == HalfCapacity)
                        {
                            nextStep = GetNextStepOnMigration();
                            _directionFlag = !_directionFlag;
                            
                            if (_additionalQueue.IsOccupied(nextStep))
                                continue;
                            
                            _additionalQueue.Off(lastStep);
                            _additionalQueue.TryOccupy(nextStep);
                            LastSteps[item] = new ItemStep(nextStep, false);
                            continue;
                        }

                        byte GetNextStepOnMigration() => (byte)(lastStep + (_directionFlag ? 1 : -1));
                        
                        bool isMigrating = nextStep == HalfCapacity;
                        bool isOccupied = (isMigrating && _additionalQueue.IsOccupied(HalfCapacity)) ||
                                          (!isMigrating && _mainQueue.IsOccupied(nextStep));
                        
                        if(isOccupied)
                            continue;

                        _mainQueue.Off(lastStep);
                        ConveyorQueue pickedQueue = isMigrating ? _additionalQueue : _mainQueue;
                        pickedQueue.TryOccupy(nextStep);
                        LastSteps[item] = new ItemStep(nextStep, true);
                    }
                }
                else
                {
                    float currentY = item.Y.Current.Value;
                    int sign = Math.Sign(lastStep - HalfCapacity);
                    float target = lastStep * ItemSize;
                    
                    if ((sign == 1 && currentY < target) ||
                        (sign == -1 && currentY > target))
                        item.Y.AddClamped(translation * sign);
                    else
                    {
                        item.Y.SetClamped(target);
                        byte nextStep = (byte)(lastStep + sign);

                        if (lastStep == QueueCapacity - 1)
                        {
                            TryTransfer(item, i, lastStep, true);
                            continue;
                        }

                        if (lastStep == 0)
                        {
                            TryTransfer(item, i, lastStep, false);
                            continue;
                        }
                        
                        if(_additionalQueue.IsOccupied(nextStep))
                            continue;

                        _additionalQueue.Off(lastStep);
                        _additionalQueue.TryOccupy(nextStep);
                        LastSteps[item] = new ItemStep(nextStep, false);
                    }
                }
            }
        }

        private bool TryTransfer(ItemOnBelt item, byte n, byte step, bool isRight)
        {
            Vector2Int right = new (LookDirection.Y, -LookDirection.X);

            if (!isRight)
                right = Vector2Int.Zero - right;
            
            Vector2Int targetPosition = Position + right;

            if (!GridMap.FactoryNodes.TryGetValue(targetPosition, out FactoryNode node) ||
                node is not ReceiverNode receiverNode) return false;
            
            if (!receiverNode.TryAccept(new ItemTransfer(item, this))) return false;
            
            Items.RemoveAt(n);
            LastSteps.Remove(item);
            _additionalQueue.Off(step);
            return true;
        }
    }
}