using UniRx;
using UnityEngine;
using VContainer;

namespace CodeBase.Application.Grid.Interaction
{
    public class Building : MonoBehaviour
    {
        [Inject] private Camera _camera;
        
        private void Start()
        {
            /*Observable.EveryFixedUpdate().Subscribe(_ =>
            {
                bool raycast = Physics.Raycast(
                    new Ray(_camera.ScreenToWorldPoint(Input.mousePosition), _camera.transform.forward),
                    out RaycastHit raycastHit);
                
                if (!raycast)
                    return;

                Vector3 targetPosition = Vector3.right * Mathf.Floor(raycastHit.point.x + _xzOffset) +
                                         Vector3.forward * Mathf.Floor(raycastHit.point.z + _xzOffset) +
                                         raycastHit.normal * _height;
                
                _hologram.transform.DOMove(targetPosition, 1 / _hologramSpeed);
            });*/
        }
    }
}