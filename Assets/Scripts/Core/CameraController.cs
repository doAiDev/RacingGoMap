using UnityEngine;

namespace RacingGoMap.Core
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] Transform _target;
        [SerializeField] float _smoothTime = 0.15f;
        [SerializeField] float _zOffset = -10f;

        Vector3 _velocity;

        public void SetTarget(Transform target) => _target = target;

        void LateUpdate()
        {
            if (_target == null) return;
            Vector3 goal = new Vector3(_target.position.x, _target.position.y, _zOffset);
            transform.position = Vector3.SmoothDamp(transform.position, goal, ref _velocity, _smoothTime);
        }
    }
}
