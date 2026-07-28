using CodeBase.Core.Factory;
using CodeBase.Core.Factory.Grid;
using UnityEngine;
using VContainer.Unity;

namespace CodeBase.Application.Factory
{
    public class FactoryPresenter : ITickable
    {
        private readonly GridMap _gridMap;

        public FactoryPresenter(GridMap gridMap)
        {
            _gridMap = gridMap;
        }

        public void Tick()
        {
            foreach(FactoryNode node in _gridMap.FactoryNodes.Values)
                node.Tick(Time.deltaTime);
        }
    }
}