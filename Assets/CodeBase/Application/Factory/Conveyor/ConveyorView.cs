using System.Collections.Generic;
using System.Collections.Specialized;
using CodeBase.Application.Factory.View;
using CodeBase.Application.Material;
using CodeBase.Core.Factory;
using CodeBase.Core.Factory.Conveyor;
using TMPro;
using UniRx;
using UnityEngine;

namespace CodeBase.Application.Factory.Conveyor
{
    public sealed class ConveyorView : FactoryNodeView<ConveyorSegment>
    {
        [SerializeField] private MaterialView _materialView;
        [SerializeField] private TMP_Text _text;
        private readonly Dictionary<ItemOnBelt, MaterialView> _materialViews = new();
        public override ConveyorSegment Node { get; protected set; }
        
        protected override void OnSetup(ConveyorSegment conveyorSegment)
        {
            Node = conveyorSegment;
            Node.Items.CollectionChanged += OnCollectionChanged;
        }

        private void Update() => _text.text = $"MainQueue: {Node.MainQueue} \n AddQueue: {Node.AdditionalQueue}";

        private void SetMaterialMovement(MaterialView materialView, ItemOnBelt item)
        {
            item.X.Current.TakeUntilDestroy(materialView).DistinctUntilChanged()
                .CombineLatest(item.Y.Current, (x, y) => new Vector2(x, y))
                .Subscribe(vector =>
                    materialView.transform.localPosition =
                        new Vector3(vector.y, 0.35f, vector.x) - new Vector3(0.5f, 0, 0.5f)
                ).AddTo(materialView);
        }
        
        private void OnDisable() => Node.Items.CollectionChanged -= OnCollectionChanged;

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if(args.NewStartingIndex > args.OldStartingIndex)
            {
                MaterialView newMaterial = Instantiate(_materialView, transform.position, Quaternion.identity);
                newMaterial.transform.SetParent(transform);
                SetMaterialMovement(newMaterial, Node.Items[args.NewStartingIndex]);
                _materialViews.Add((ItemOnBelt)args.NewItems[0], newMaterial);
            }
            else
            {
                // todo make pool of views and add composite disposable for itemOnBelt reactive properties
                ItemOnBelt item = (ItemOnBelt)args.OldItems[0];
                Destroy(_materialViews[item].gameObject);
                _materialViews.Remove(item);
            }
        }
    }
}