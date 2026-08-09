using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Application.Factory.UI
{
    public class NodesTab : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup _buttons;

        public void AddNodeSpawnButton(NodeButton nodeButton) => nodeButton.transform.SetParent(_buttons.transform);
    }
}