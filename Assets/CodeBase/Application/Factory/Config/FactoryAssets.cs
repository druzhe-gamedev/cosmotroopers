using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeBase.Application.Factory
{
    [CreateAssetMenu(fileName = "FactoryAssets", menuName = "Factory/Assets List", order = 0)]
    public class FactoryAssets : ScriptableObject
    {
        [field: SerializeField] public FactoryCategory[] Assets { get; private set; }
/*
        private Dictionary<Type, List<ScriptableObject>> _assetsTypes;

        public Dictionary<Type, List<ScriptableObject>> GetOrCreateAssetsTypes()
        {
            if (_assetsTypes != null)
                return _assetsTypes;

            _assetsTypes = new Dictionary<Type, List<ScriptableObject>>();
            foreach (ScriptableObject asset in Assets.SelectMany(asset => asset.Segments))
            {
                Type type = asset.GetType();
                
                if (_assetsTypes.ContainsKey(type))
                {
                    _assetsTypes[type].Add(asset);
                    continue;
                }

                _assetsTypes[type] = new List<ScriptableObject> { asset };
            }

            return _assetsTypes;
        }*/
    }
}