using System.Threading;
using CodeBase.Application.Common.UI;
using CodeBase.Application.Factory.Config;
using CodeBase.Application.Grid.Interaction;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        [SerializeField] private Transform _buttons;
        [SerializeField] private ButtonView _buttonView;
        [SerializeField] private NodeButton _nodeButton;
        [SerializeField] private NodesTab _nodesTab;
        [SerializeField] private NodesTabsContainer _tabsContainer;
        [Inject] private UIConfig _uiConfig;
        [Inject] private FactoryAssets _factoryAssets;
        [Inject] private Building _building;
        
        public override UniTask Show(CancellationToken cancellationToken) => UniTask.CompletedTask;

        public override UniTask Hide(CancellationToken cancellationToken) => UniTask.CompletedTask;

        private void Start()
        {
            LoadTabs();
            _menuToggle.onClick.AsObservable().Subscribe(_ => _buildingMenu.SetActive(!_buildingMenu.activeSelf)).AddTo(this);
        }

        private void LoadTabs()
        {
            int tabNumber = 0;
            float offsetX = 0;
            
            foreach (FactoryCategory category in _factoryAssets.Assets)
            {
                int num = tabNumber;
                NodesTab nodesTab = Instantiate(_nodesTab, _tabsContainer.transform);
                ButtonView button = Instantiate(_buttonView, _buttons.transform, true);
                button.Setup(category.Name);
                
                button.Button.onClick.AsObservable().Subscribe(_ =>
                {
                    float targetX = num == 0 ? 0 : _tabsContainer.Transform.rect.width / tabNumber * num;
                    _tabsContainer.transform.DOLocalMoveX(offsetX - targetX, 0.15f);
                });
                
                foreach (FactoryNodeAsset nodeAsset in category.NodeAssets)
                {
                    NodeButton nodeButton = Instantiate(_nodeButton);
                    nodeButton.Setup(nodeAsset, _building);
                    nodesTab.AddNodeSpawnButton(nodeButton);
                }

                tabNumber++;
                offsetX = _tabsContainer.transform.localPosition.x;
            }
        }
    }
}