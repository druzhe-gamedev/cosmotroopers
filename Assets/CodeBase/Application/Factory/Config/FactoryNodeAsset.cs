using CodeBase.Application.Factory.View;
using UnityEngine;

namespace CodeBase.Application.Factory
{
    public abstract class FactoryNodeAsset : ScriptableObject
    {
        public abstract Sprite Icon { get; protected set;  }
        public abstract FactoryNodeView FactoryNodeView { get; }
    }
}