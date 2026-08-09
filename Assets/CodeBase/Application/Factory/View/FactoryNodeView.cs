using System;
using CodeBase.Core.Factory;
using UnityEngine;

namespace CodeBase.Application.Factory.View
{
    public abstract class FactoryNodeView : MonoBehaviour
    {
        public abstract FactoryNode FactoryNode { get; }
        [field: SerializeField] public MeshFilter MeshFilter { get; private set; }
        
        public abstract void Setup(FactoryNode node);
    }

    public abstract class FactoryNodeView<TNode> : FactoryNodeView
        where TNode : FactoryNode
    {
        public abstract TNode Node { get; protected set; }
        public override FactoryNode FactoryNode => Node;
        
        public sealed override void Setup(FactoryNode node)
        {
            if (node is not TNode typedNode)
                throw new ArgumentException($"Expected {typeof(TNode).Name}, got {node.GetType().Name}");

            Node = typedNode;
            OnSetup(typedNode);
        }

        protected abstract void OnSetup(TNode node);
    }
}