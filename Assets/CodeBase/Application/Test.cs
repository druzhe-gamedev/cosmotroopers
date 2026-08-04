using System;
using CodeBase.Application.Factory.Conveyors;
using CodeBase.Application.Factory.Dispenser;
using CodeBase.Core.Factory.Conveyor;
using CodeBase.Core.Factory.Dispenser;
using CodeBase.Core.Factory.Grid;
using CodeBase.Core.Material;
using CodeBase.Core.Mathematics;
using UniRx;
using UnityEngine;
using VContainer;
using Vector2Int = CodeBase.Core.Mathematics.Vector2Int;

namespace CodeBase.Application
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private OreDispenserView _oreDispenserView;
        [SerializeField] private OreDispenserView _oreDispenserView2;
        [SerializeField] private ConveyorView _conveyorView;
        [SerializeField] private ConveyorView _conveyorView2;
        [Inject] private GridMap _gridMap;
        
        private void Start()
        {
            OreDispenser oreDispenser = new(
                0.6f,
                new OreStorage(MaterialType.Copper, 5),
                _gridMap,
                TimeSpan.FromSeconds(0.2),
                new Vector2Int(0, 0),
                new Vector2Int(1, 1)
            );
            _gridMap.TryAddNode(oreDispenser);
            _oreDispenserView.Setup(oreDispenser);
        
            OreDispenser oreDispenser2 = new(
                0.6f,
                new OreStorage(MaterialType.Copper, 5),
                _gridMap,
                TimeSpan.FromSeconds(0.2),
                new Vector2Int(1, 1),
                new Vector2Int(1, 1)
            );
            _gridMap.TryAddNode(oreDispenser2);
            _oreDispenserView2.Setup(oreDispenser2);

            ConveyorSegment conveyor = new(
                5,
                0.25f,
                new NodeRotation(0),
                _gridMap,
                new Vector2Int(1, 0),
                new Vector2Int(1, 1)
            );
            _gridMap.TryAddNode(conveyor);
            _conveyorView.Setup(conveyor);
            
            ConveyorSegment conveyor2 = new(
                5,
                0.25f,
                new NodeRotation(0),
                _gridMap,
                new Vector2Int(2, 0),
                new Vector2Int(1, 1)
            );
            _gridMap.TryAddNode(conveyor2);
            _conveyorView2.Setup(conveyor2);
            
            oreDispenser.Receivers.Add(conveyor);
            oreDispenser2.Receivers.Add(conveyor);
        }
    }
}