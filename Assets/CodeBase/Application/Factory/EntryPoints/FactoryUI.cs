using CodeBase.Application.Common.UI;
using CodeBase.Application.Factory.UI;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Application.Factory
{
    public class FactoryUI : IStartable
    {
        [Inject] private UIFactory _factory;
        
        public void Start()
        {
            _factory.Spawn<FactoryBuildingHud>().Forget();
        }
    }
}