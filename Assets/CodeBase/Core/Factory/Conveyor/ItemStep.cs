using System;

namespace CodeBase.Core.Factory.Conveyor
{
    public struct ItemStep : IEquatable<ItemStep>
    {
        public bool IsMainQueue;
        public byte Step;

        public ItemStep(byte step, bool isMainQueue)
        {
            Step = step;
            IsMainQueue = isMainQueue;
        }

        public bool Equals(ItemStep other) => IsMainQueue == other.IsMainQueue && Step == other.Step;

        public override bool Equals(object obj) => obj is ItemStep other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(IsMainQueue, Step);
    }
}