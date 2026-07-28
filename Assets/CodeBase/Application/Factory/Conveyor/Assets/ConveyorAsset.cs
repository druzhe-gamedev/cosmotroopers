using CodeBase.Application.Factory.View;
using CodeBase.Core.Factory;
using UnityEngine;

namespace CodeBase.Application.Factory.Conveyors.Assets
{
    [CreateAssetMenu(fileName = "ConveyorAsset", menuName = "Factory/Conveyor")]
    public class ConveyorAsset : FactoryNodeAsset
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        [field: SerializeField] public ConveyorView ConveyorView { get; private set; }
        [field: SerializeField] public int Capacity { get; private set;  }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public override Sprite Icon { get; protected set; }
        public override FactoryNodeView FactoryNodeView => ConveyorView;
    }
}