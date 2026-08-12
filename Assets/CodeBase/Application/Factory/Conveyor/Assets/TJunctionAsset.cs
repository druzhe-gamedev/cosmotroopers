using CodeBase.Application.Factory.Config;
using CodeBase.Core.Factory;
using CodeBase.Core.Factory.Conveyor;
using CodeBase.Core.Factory.Grid;
using UnityEngine;

namespace CodeBase.Application.Factory.Conveyor.Assets
{
    [CreateAssetMenu(fileName = "TJunctionAsset", menuName = "Factory/TJunction")]
    public class TJunctionAsset : FactoryNodeAsset<ConveyorView, ConveyorBase>
    {
        [field: SerializeField] public override ConveyorView View { get; protected set; }
        [field: SerializeField] public byte Capacity { get; private set;  }
        [field: SerializeField] public float Speed { get; private set; }

        protected override ConveyorBase CreateTypedNode(GridMap gridMap, NodePositioning nodePositioning) =>
            new TJunction(Capacity, Speed, gridMap, nodePositioning);
    }
}