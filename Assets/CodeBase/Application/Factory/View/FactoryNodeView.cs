using CodeBase.Core.Factory;
using UnityEngine;

namespace CodeBase.Application.Factory.View
{
    public abstract class FactoryNodeView : MonoBehaviour
    {
        public abstract FactoryNode FactoryNode { get; } 
    }
}