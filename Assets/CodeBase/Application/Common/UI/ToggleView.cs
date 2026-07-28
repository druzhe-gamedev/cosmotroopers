using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Application.Common.UI
{
    public class ToggleView : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;
        [SerializeField] private TMP_Text _label;

        public void Setup(string label) => _label.text = label;
    }
}