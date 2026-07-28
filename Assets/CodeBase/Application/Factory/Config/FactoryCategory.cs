using UnityEngine;

namespace CodeBase.Application.Factory
{
    [CreateAssetMenu(fileName = "NewSegmentsList", menuName = "Factory/Segments List")]
    public class FactoryCategory : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public FactoryNodeAsset[] NodeAssets { get; private set; }
    }
}