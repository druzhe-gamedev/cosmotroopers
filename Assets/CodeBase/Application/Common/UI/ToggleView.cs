using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Application.Common.UI
{
    public class ToggleView : MonoBehaviour
    {
        [field: SerializeField] public Toggle Toggle { get; private set; }
        [SerializeField] private TMP_Text _label;

        public void Setup(string label) => _label.text = label;
    }
}