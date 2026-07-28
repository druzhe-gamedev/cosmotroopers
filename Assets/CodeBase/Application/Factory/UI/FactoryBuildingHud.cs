using System;
using System.Threading;
using CodeBase.Application.Common.UI;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace CodeBase.Application.Factory.UI
{
    public class FactoryBuildingHud : BaseWindow
    {
        [SerializeField] private GameObject _buildingMenu;
        [SerializeField] private Button _menuToggle;
        [SerializeField] private ToggleGroup _toggles;
        [SerializeField] private ToggleView _togglePrefab;
        [Inject] private UIConfig _uiConfig;
        [Inject] private FactoryAssets _factoryAssets;
        
        public override UniTask Show(CancellationToken cancellationToken) => UniTask.CompletedTask;

        public override UniTask Hide(CancellationToken cancellationToken) => UniTask.CompletedTask;

        private void Start()
        {
            LoadTabs();
            _menuToggle.onClick.AsObservable().Subscribe(_ => _buildingMenu.SetActive(!_buildingMenu.activeSelf)).AddTo(this);
        }

        private void LoadTabs()
        {
            foreach (FactoryCategory category in _factoryAssets.Assets)
            {
                foreach (FactoryNodeAsset nodeAsset in category.NodeAssets)
                {
                    ToggleView toggle = Instantiate(_togglePrefab, _toggles.transform, true);
                    toggle.Setup(category.Name);
                }
            }
        }
    }
}