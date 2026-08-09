using System;
using CodeBase.Application.Factory.Config;
using CodeBase.Application.Factory.Miner;
using CodeBase.Application.Factory.View;
using CodeBase.Core.Factory;
using CodeBase.Core.Factory.Grid;
using CodeBase.Core.Factory.Miner;
using CodeBase.Core.Material;
using UnityEngine;

namespace CodeBase.Application.Factory.OreMiner.Assets
{
    [CreateAssetMenu(fileName = "OreMiner", menuName = "Factory/Miner")]
    public class OreMinerAsset : FactoryNodeAsset
    {
        [SerializeField] private float _orePerSecond;
        [SerializeField] private float _oreTransferIntervalInSeconds;
        [SerializeField] private MaterialType _materialType;
        [SerializeField] private int _capacity;
        [SerializeField] private OreMinerView _oreMinerView;
        private TimeSpan _oreTransferInterval => TimeSpan.FromSeconds(_oreTransferIntervalInSeconds);
        public override FactoryNodeView FactoryNodeView => _oreMinerView;

        public override FactoryNode CreateNode(GridMap gridMap, NodePositioning nodePositioning) =>
            new Core.Factory.Miner.OreMiner(
                gridMap,
                nodePositioning,
                new OreStorage(_materialType, _capacity),
                _orePerSecond,
                _oreTransferInterval
            );
    }
}