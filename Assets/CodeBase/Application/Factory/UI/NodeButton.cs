using CodeBase.Application.Factory.Config;
using CodeBase.Application.Grid.Interaction;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Application.Factory.UI
{
    public class NodeButton : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Button _button;
        private Building _building;
        private FactoryNodeAsset _nodeAsset;

        public void Setup(FactoryNodeAsset nodeAsset, Building building)
        {
            _nodeAsset = nodeAsset;
            (_icon.sprite, _building) = (nodeAsset.Icon, building);
            _button.onClick.AsObservable().Subscribe(_ => _building.SetHologram(
                    _nodeAsset.FactoryNodeView.MeshFilter.sharedMesh,
                    0f,
                    nodeAsset
                )
            ).AddTo(this);
        }
    }
}