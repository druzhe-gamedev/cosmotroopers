using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Application.Factory.UI
{
    public class BuildingNodeButton
    {
        [SerializeField] private Image _icon;

        public void Setup(Sprite icon)
        {
            _icon.sprite = icon;
        }
    }
}