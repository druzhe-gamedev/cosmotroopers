using System;
using System.Collections.Generic;
using CodeBase.Core.Factory.Grid;
using CodeBase.Core.Material;
using CodeBase.Core.Mathematics;
using UniRx;

namespace CodeBase.Core.Factory.Miner
{
    public struct OreStorage
    {
        public MaterialType MaterialType { get; }
        public ItemProgress Quantity { get; }

        public OreStorage(MaterialType materialType, int capacity)
        {
            MaterialType = materialType;
            Quantity = new ItemProgress(0, capacity);
        }
    }
    
    public class OreMiner : FactoryNode, IDisposable
    {
        public float OrePerSecond { get; }
        public ItemProgress MiningProgress { get; } = ItemProgress.Normalized();
        public OreStorage OreStorage { get; }
        private int _receiverNumber;
        private readonly CompositeDisposable _disposables = new();

        public OreMiner(GridMap gridMap, NodePositioning nodePositioning, OreStorage oreStorage, float orePerSecond,
                        TimeSpan oreTransferInterval
        ) : base(gridMap, nodePositioning)
        {
            OrePerSecond = orePerSecond;
            OreStorage = oreStorage;

            Observable.EveryUpdate()
                      .Where(_ => MiningProgress.IsMax && !oreStorage.Quantity.IsMax)
                      .Subscribe(_ => Mine()).AddTo(_disposables);

            Observable.Interval(oreTransferInterval).Subscribe(_ => TryTransferItem()).AddTo(_disposables);
        }

        public override void Tick(float deltaTime) => MiningProgress.AddClamped(deltaTime * OrePerSecond);
        
        public void Dispose() => _disposables.Dispose();
        
        private void Mine() 
        {
            MiningProgress.Reset();
            
            OreStorage.Quantity.AddClamped(1);
        }

        private void TryTransferItem()
        {
            if (OreStorage.Quantity.Current.Value < 1)
                return;
            
            ItemOnBelt item = new (OreStorage.MaterialType);

            Vector2Int targetPosition = Position + LookDirection;
            if (!GridMap.FactoryNodes.TryGetValue(targetPosition, out FactoryNode node) ||
                node is not ReceiverNode receiverNode) return;

            if (!receiverNode.TryAccept(new ItemTransfer(item, this))) return;
            OreStorage.Quantity.AddClamped(-1);
        }
    }
}