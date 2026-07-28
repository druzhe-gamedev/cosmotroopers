using System.Collections.Generic;
using System.Collections.Specialized;
using CodeBase.Application.Factory.View;
using CodeBase.Application.Material;
using CodeBase.Core.Factory;
using CodeBase.Core.Factory.Conveyor;
using UniRx;
using UnityEngine;

namespace CodeBase.Application.Factory.Conveyors
{
    public class ConveyorView : FactoryNodeView
    {
        [SerializeField] private MaterialView _materialView; 
        private readonly List<MaterialView> _materialViews = new();
        public ConveyorSegment ConveyorSegment;
        public override FactoryNode FactoryNode => ConveyorSegment;

        public void Setup(ConveyorSegment conveyorSegment)
        {
            ConveyorSegment = conveyorSegment;
            ConveyorSegment.Items.CollectionChanged += OnCollectionChanged;
        }

        private void SetMaterialMovement(MaterialView materialView, ItemOnBelt item)
        {
            item.X.Current.DistinctUntilChanged().CombineLatest(item.Y.Current, (x, y) => new Vector2(x, y))
                .Subscribe(vector =>
                    {
                        Vector3 pos = materialView.transform.position;
                        materialView.transform.position.Set(vector.x, vector.y, pos.z);
                    }
                ).AddTo(materialView);
        }
        
        private void OnDisable() => ConveyorSegment.Items.CollectionChanged -= OnCollectionChanged;

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if(args.NewStartingIndex > args.OldStartingIndex)
            {
                MaterialView newMaterial = Instantiate(_materialView, transform.position, Quaternion.identity);
                newMaterial.transform.SetParent(transform);
                SetMaterialMovement(newMaterial, ConveyorSegment.Items[args.NewStartingIndex]);
                _materialViews.Add(newMaterial);
            }
            else
            {
                // todo make pool of views and add composite disposable for itemOnBelt reactive properties
                if (_materialViews.Count <= 0)
                    return;
                Destroy(_materialViews[^1].gameObject);
                _materialViews.RemoveAt(_materialViews.Count - 1);
            }
        }
    }
}