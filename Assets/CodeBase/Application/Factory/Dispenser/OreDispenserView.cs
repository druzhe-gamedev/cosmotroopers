using CodeBase.Application.Factory.View;
using CodeBase.Core.Factory;
using CodeBase.Core.Factory.Dispenser;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Application.Factory.Dispenser
{
    public class OreDispenserView : FactoryNodeView
    {
        [SerializeField] private Slider _miningProgress;
        [SerializeField] private float _progressBarSpeed;
        private OreDispenser _dispenser;

        public override FactoryNode FactoryNode => _dispenser;
        
        public void Setup(OreDispenser dispenser)
        {
            _dispenser = dispenser;
            _dispenser.MiningProgress.Current
                      .DistinctUntilChanged()
                      .Subscribe(progress => _miningProgress.value = progress).AddTo(this);
        }

        private void OnDisable()
        {
            _dispenser.Dispose();
        }
    }
}