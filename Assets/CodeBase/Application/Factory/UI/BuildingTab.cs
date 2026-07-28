using CodeBase.Application.Common.UI;
using UnityEngine;

namespace CodeBase.Application.Factory.UI
{
    public class BuildingTab : MonoBehaviour
    {
        [field: SerializeField] public ToggleView Toggle;
        [SerializeField] private GridLayout _grid;
    }
}