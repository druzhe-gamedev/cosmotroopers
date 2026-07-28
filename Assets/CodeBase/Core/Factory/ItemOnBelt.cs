using System;
using CodeBase.Core.Material;
using UniRx;
using UnityEngine;

namespace CodeBase.Core.Factory
{
    public struct ItemProgress
    {
        public float Min { get; set; }
        public float Max { get; set; }
        
        public bool IsMax => Mathf.Approximately(_current.Value, Max);
        public bool IsMin => Mathf.Approximately(_current.Value, Min);

        public ReactiveProperty<float> Current => _current;
        private readonly ReactiveProperty<float> _current;

        public void AddClamped(float step) => _current.Value = Mathf.Clamp(_current.Value + step, Min, Max);
        public void SetClamped(float value) => _current.Value = Mathf.Clamp(value, Min, Max);

        public void Reset() => _current.Value = Min;

        public ItemProgress(float min, float max, float current) => (Min, Max, _current) =
            (min, max, new ReactiveProperty<float>(Math.Clamp(current, min, max)));
        public ItemProgress(float min, float max) => (Min, Max, _current) = (min, max, new ReactiveProperty<float>(0));
        public static ItemProgress Normalized() => new(0, 1);

        public override string ToString() => $"({Min} - {_current} - {Max})";
    }
    
    public class ItemOnBelt
    {
        public MaterialType MaterialType { get; set; }

        public ItemProgress X { get; private set; } = ItemProgress.Normalized();
        public ItemProgress Y { get; private set; } = ItemProgress.Normalized();

        public ReactiveProperty<Vector2> Position = new(Vector2.zero);

        public ItemOnBelt(MaterialType materialType) => MaterialType = materialType;

        public override string ToString() => $"{MaterialType} {X} {Y}";
    }
}