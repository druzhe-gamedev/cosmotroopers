using CodeBase.Application.Factory.View;
using CodeBase.Core.Factory;
using CodeBase.Core.Factory.Grid;
using UnityEngine;

namespace CodeBase.Application.Factory.Config
{
    public abstract class FactoryNodeAsset : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; protected set;  }
        [field: SerializeField] public Vector2Int Size { get; protected set; }
        
        public abstract FactoryNodeView FactoryNodeView { get; }
        public abstract FactoryNode CreateNode(GridMap gridMap, NodePositioning nodePositioning);
    }
    
    public abstract class FactoryNodeAsset<TView, TNode> : FactoryNodeAsset
        where TView : FactoryNodeView<TNode>
        where TNode : FactoryNode
    {
        public abstract TView View { get; protected set; }

        public sealed override FactoryNodeView FactoryNodeView => View;

        protected abstract TNode CreateTypedNode(GridMap gridMap, NodePositioning nodePositioning);

        public sealed override FactoryNode CreateNode(GridMap gridMap, NodePositioning nodePositioning)
            => CreateTypedNode(gridMap, nodePositioning);
    }
}