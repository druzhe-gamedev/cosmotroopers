using System;
using CodeBase.Application.Factory.Config;
using CodeBase.Application.Factory.View;
using CodeBase.Core.Factory;
using CodeBase.Core.Factory.Grid;
using CodeBase.Core.Mathematics;
using DG.Tweening;
using UniRx;
using UnityEngine;
using VContainer;
using Vector2Int = CodeBase.Core.Mathematics.Vector2Int;

namespace CodeBase.Application.Grid.Interaction
{
    public record ValidNodePositioning(NodePositioning NodePositioning, float Y) : NodeViewPositioning;
    public record WrongNodePositioning : NodeViewPositioning;
    public abstract record NodeViewPositioning;
    
    public class Building : MonoBehaviour
    {
        [Inject] private Camera _camera;
        [Inject] private GridMap _gridMap;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private float _hologramSpeed;
        [SerializeField] private float _hologramRotationSpeed;
        [SerializeField] private Hologram _hologram;
        [SerializeField] private FactoryNodeAsset _factoryNodeAsset;
        private NodeRotation _nodeRotation;
        private float _offsetY;

        public void SetHologram(Mesh mesh, float offsetY, FactoryNodeAsset factoryNodeAsset)
        {
            _hologram.gameObject.SetActive(true);
            _hologram.SetMesh(mesh);
            _offsetY = offsetY;
            _factoryNodeAsset = factoryNodeAsset;
        }

        private void Awake()
        {
            Vector2Int lookVector = _nodeRotation.ToVector();
            Vector3 forward = Vector3.right * lookVector.X + Vector3.forward * lookVector.Y;

            _hologram.transform.forward = forward;
        }

        private void Start()
        {
            IObservable<ValidNodePositioning> validPositioningStream =
                Observable.EveryFixedUpdate()
                          .Select(_ => GetNodePositioning())
                          .OfType<NodeViewPositioning, ValidNodePositioning>();
            
            validPositioningStream.Subscribe(nodePositioning =>
            {
                ValidNodePositioning validPositioning = nodePositioning;
                Vector2Int nodePosition = validPositioning.NodePositioning.Position;
                _hologram.transform.DOMove(
                    new Vector3(nodePosition.X, validPositioning.Y, nodePosition.Y),
                    1 / _hologramSpeed
                );
            }).AddTo(this);
            
            Observable.EveryUpdate().Where(_ => Input.anyKeyDown && _factoryNodeAsset != null).Select(_ => Input.inputString)
                      .Subscribe(key =>
                      {
                          switch (key.ToLowerInvariant())
                          {
                              case "a":
                                  _nodeRotation.RotateCcw();
                                  break;
                              case "d":
                                  _nodeRotation.RotateCw();
                                  break;
                          }
                          
                          Vector2Int lookVector = _nodeRotation.ToVector();
                          Vector3 forward = Vector3.right * lookVector.X + Vector3.forward * lookVector.Y;

                          _hologram.transform.DORotateQuaternion(
                              Quaternion.LookRotation(forward, Vector3.up),
                              1 / _hologramRotationSpeed
                          );
                      }).AddTo(this);

            Observable.EveryUpdate()
                      .WithLatestFrom(validPositioningStream, (_, positioning) => positioning)
                      .Where(CanSetupNode)
                      .Where(_ => Input.GetMouseButtonDown(0))
                      .Subscribe(InstantiateNode)
                      .AddTo(this);
        }

        private bool CanSetupNode(ValidNodePositioning nodePositioning) => 
            _gridMap.CanSetup(
                nodePositioning.NodePositioning.Position,
                new Vector2Int(_factoryNodeAsset.Size.x, _factoryNodeAsset.Size.y)
            );
        
        private void InstantiateNode(ValidNodePositioning nodePositioning)
        {
            FactoryNodeView factoryNodeView = Instantiate(_factoryNodeAsset.FactoryNodeView);
            ValidNodePositioning positioning = nodePositioning;
            Vector2Int position = positioning.NodePositioning.Position;
            factoryNodeView.transform.position = new Vector3(position.X, positioning.Y, position.Y);

            Vector2Int lookVector = _nodeRotation.ToVector();
            Vector3 forward = Vector3.right * lookVector.X + Vector3.forward * lookVector.Y;
            factoryNodeView.transform.forward = forward;

            FactoryNode node = _factoryNodeAsset.CreateNode(_gridMap, positioning.NodePositioning);
            _gridMap.TryAddNode(node);
            factoryNodeView.Setup(node);
        }
        
        private NodeViewPositioning GetNodePositioning() 
        {
            bool raycast = Physics.Raycast(
                new Ray(_camera.ScreenToWorldPoint(Input.mousePosition), _camera.transform.forward),
                out RaycastHit raycastHit);

            if (!raycast || _factoryNodeAsset == null)
                return new WrongNodePositioning();
            
            Vector3 targetPosition = Vector3.right * (int)Math.Ceiling(raycastHit.point.x + _offset.x) +
                                     Vector3.forward * (int)Math.Ceiling(raycastHit.point.z + _offset.y) +
                                     raycastHit.normal * _offsetY;

            return new ValidNodePositioning(
                new NodePositioning(
                    new Vector2Int((int)targetPosition.x, (int)targetPosition.z),
                    new Vector2Int(_factoryNodeAsset.Size.x, _factoryNodeAsset.Size.y),
                    _nodeRotation
                ),
                targetPosition.y
            );
        }
    }
}