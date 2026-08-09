using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Application.Factory.UI
{
    [RequireComponent(typeof(HorizontalLayoutGroup), typeof(RectTransform))]
    public class NodesTabsContainer : MonoBehaviour
    {
        [field: SerializeField] public HorizontalLayoutGroup Layout { get; private set; }
        [field: SerializeField] public RectTransform Transform { get; private set; }

        private void Awake()
        {
            Layout = GetComponent<HorizontalLayoutGroup>();
            Transform = GetComponent<RectTransform>();
        }
    }
}