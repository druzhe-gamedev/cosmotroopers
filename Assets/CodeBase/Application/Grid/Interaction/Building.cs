using System;
using CodeBase.Application.Factory.Config;
using CodeBase.Application.Factory.View;
using CodeBase.Core.Factory;
using CodeBase.Core.Factory.Grid;
using CodeBase.Core.Mathematics;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using Vector2Int = CodeBase.Core.Mathematics.Vector2Int;

namespace CodeBase.Application.Grid.Interaction
{
    public record ValidNodePositioning(NodePositioning NodePositioning, float Y) : NodeViewPositioning;
    public record WrongNodePositioning : NodeViewPositioning
    {
        public static WrongNodePositioning Default
        {
            get
            {
                _default ??= new WrongNodePositioning();

                return _default;
            }
        }
        private static WrongNodePositioning _default;
        
    }
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
            
            Observable.EveryUpdate()
                      .Select(_ => Input.GetAxisRaw("Mouse ScrollWheel"))
                      .Where(scroll => scroll != 0)
                      .ThrottleFirst(TimeSpan.FromMilliseconds(150))
                      .Subscribe(scroll =>
                      {
                          if(scroll > 0)
                              _nodeRotation.RotateCcw();
                          else 
                              _nodeRotation.RotateCw();
                          
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
                      .Where(_ => !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
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
            Vector2Int position = nodePositioning.NodePositioning.Position;
            factoryNodeView.transform.position = new Vector3(position.X, nodePositioning.Y, position.Y);

            Vector2Int lookVector = _nodeRotation.ToVector();
            Vector3 forward = Vector3.right * lookVector.X + Vector3.forward * lookVector.Y;
            factoryNodeView.transform.forward = forward;

            FactoryNode node = _factoryNodeAsset.CreateNode(_gridMap, nodePositioning.NodePositioning);
            _gridMap.TryAddNode(node);
            factoryNodeView.Setup(node);
        }
        
        private NodeViewPositioning GetNodePositioning() 
        {
            bool raycast = Physics.Raycast(
                new Ray(_camera.ScreenToWorldPoint(Input.mousePosition), _camera.transform.forward),
                out RaycastHit raycastHit);

            if (!raycast || _factoryNodeAsset == null)
                return WrongNodePositioning.Default;
            
            Vector3 targetPosition = Vector3.right * (int)Math.Ceiling(raycastHit.point.x + _offset.x) +
                                     Vector3.forward * (int)Math.Ceiling(raycastHit.point.z + _offset.y) +
                                     raycastHit.normal * _offsetY;

            Vector2Int gridPosition = new ((int)targetPosition.x, (int)targetPosition.z);

            if(_gridMap.FactoryNodes.TryGetValue(gridPosition, out _))
                return WrongNodePositioning.Default;
            
            return new ValidNodePositioning(
                new NodePositioning(
                    gridPosition,
                    new Vector2Int(_factoryNodeAsset.Size.x, _factoryNodeAsset.Size.y),
                    _nodeRotation
                ),
                targetPosition.y
            );
        }
    }
}