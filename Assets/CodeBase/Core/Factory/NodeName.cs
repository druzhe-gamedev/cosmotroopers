using System;

namespace CodeBase.Core.Factory
{
    public struct NodeName : IEquatable<NodeName>
    {
        public string Name { get; }

        public static NodeName Create(FactoryNode node) => new($"{node.GetType()} {node.Position}");
        
        public override string ToString() => Name;

        private NodeName(string name) => Name = name;

        public bool Equals(NodeName other) => Name == other.Name;
        public override bool Equals(object obj) => obj is NodeName other && Equals(other);
        public override int GetHashCode() => Name != null ? Name.GetHashCode() : 0;
    }
}