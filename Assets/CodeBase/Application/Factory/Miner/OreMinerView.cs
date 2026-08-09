using CodeBase.Application.Factory.View;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Application.Factory.Miner
{
    public class OreMinerView : FactoryNodeView<Core.Factory.Miner.OreMiner>
    {
        [SerializeField] private Slider _miningProgress;

        public override Core.Factory.Miner.OreMiner Node { get; protected set; }
        
        protected override void OnSetup(Core.Factory.Miner.OreMiner node)
        {
            Node = node;
            Node.MiningProgress.Current
                  .DistinctUntilChanged()
                  .Subscribe(progress => _miningProgress.value = progress).AddTo(this);
        }

        private void OnDisable() => Node.Dispose();
    }
}