using UnityEngine;

namespace CodeBase.Application.Factory.Config
{
    [CreateAssetMenu(fileName = "FactoryAssets", menuName = "Factory/Assets List", order = 0)]
    public class FactoryAssets : ScriptableObject
    {
        [field: SerializeField] public FactoryCategory[] Assets { get; private set; }
    }
}