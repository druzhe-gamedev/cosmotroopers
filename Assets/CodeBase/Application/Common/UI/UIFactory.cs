using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Application.Common.UI
{
    public class UIFactory
    {
        private readonly UIConfig _uiConfig;
        private readonly HUD _hud;
        private readonly IObjectResolver _container;

        public UIFactory(UIConfig uiConfig, HUD hud, IObjectResolver container) =>
            (_uiConfig, _hud, _container) = (uiConfig, hud, container);

        public async UniTask<T> Spawn<T>(CancellationToken cancellationToken = default) where T : BaseWindow
        {
            T instance = _container.Instantiate(
                _uiConfig.GetWindow<T>(),
                Vector3.zero,
                Quaternion.identity,
                _hud.transform
            );

            instance.transform.localPosition = Vector3.zero;
            await instance.Show(cancellationToken);
            return instance;
        }
    }
}