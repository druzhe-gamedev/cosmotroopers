using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Application.Common.UI
{
    public class ButtonView : MonoBehaviour
    {
        [field: SerializeField] public Button Button { get; private set; }
        [SerializeField] private TMP_Text _label;

        public void Setup(string label) => _label.text = label;
    }
}