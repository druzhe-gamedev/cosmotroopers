using CodeBase.Application.Factory.View;
using UniRx;
using UnityEngine;
using VContainer;

namespace CodeBase.Application.Grid.Interaction
{
    public class Building : MonoBehaviour
    {
        [Inject] private Camera _camera;
        [SerializeField] private float _offset;
        [SerializeField] private float _height;
        [SerializeField] private FactoryNodeView _hologram;
        
        /*private void Start()
        {
            Observable.EveryFixedUpdate().Subscribe(_ =>
            {
                bool raycast = Physics.Raycast(
                    new Ray(_camera.ScreenToWorldPoint(Input.mousePosition), _camera.transform.forward),
                    out RaycastHit raycastHit);
                
                if (!raycast)
                    return;

                Vector3 targetPosition = Vector3.right * Mathf.Floor(raycastHit.point.x + _offset) +
                                         Vector3.forward * Mathf.Floor(raycastHit.point.z + _offset) +
                                         raycastHit.normal * _height;
                
                _hologram.transform.DOMove(targetPosition, 1 / _hologramSpeed);
            }).AddTo(this);
        }*/
    }
}