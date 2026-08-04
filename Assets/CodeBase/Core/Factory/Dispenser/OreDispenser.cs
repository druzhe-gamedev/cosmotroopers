using System;
using System.Collections.Generic;
using CodeBase.Core.Factory.Grid;
using CodeBase.Core.Material;
using CodeBase.Core.Mathematics;
using UniRx;

namespace CodeBase.Core.Factory.Dispenser
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
    
    public class OreDispenser : FactoryNode, IDisposable
    {
        public float OrePerSecond { get; }
        public TimeSpan OreTransferInterval { get; }
        public ItemProgress MiningProgress { get; } = ItemProgress.Normalized();
        public OreStorage OreStorage { get; }
        public List<ReceiverNode> Receivers { get; } = new();
        private int _receiverNumber;
        private readonly CompositeDisposable _disposables = new();
        private readonly GridMap _gridMap;

        public OreDispenser(float orePerSecond, OreStorage oreStorage, GridMap gridMap, TimeSpan oreTransferInterval,
                            Vector2Int position, Vector2Int size
        ) : base(position, size)
        {
            OrePerSecond = orePerSecond;
            OreStorage = oreStorage;
            _gridMap = gridMap;

            MiningProgress.Current
                          .Where(_ => MiningProgress.IsMax && !oreStorage.Quantity.IsMax)
                          .Subscribe(_ => Mine()).AddTo(_disposables);

            Observable.Interval(oreTransferInterval).Subscribe(_ => TryTransferItem()).AddTo(_disposables);
        }

        public override void Tick(float deltaTime) => MiningProgress.AddClamped(deltaTime * OrePerSecond);
        
        public void Dispose() => _disposables.Dispose();
        
        private void Mine() 
        {
            OreStorage.Quantity.AddClamped(1);
            
            if (!OreStorage.Quantity.IsMax)
                MiningProgress.Reset();
        }

        private void TryTransferItem()
        {
            if (OreStorage.Quantity.Current.Value < 1)
                return;
            
            ItemOnBelt item = new (OreStorage.MaterialType);

            if (Receivers[_receiverNumber].TryAccept(new ItemTransfer(item, this)))
                OreStorage.Quantity.AddClamped(-1);

            _receiverNumber = (_receiverNumber + 1) % Receivers.Count;
        }
    }
}