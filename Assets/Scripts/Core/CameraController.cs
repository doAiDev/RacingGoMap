using UnityEngine;

namespace RacingGoMap.Core
{
    /// <summary>
    /// 살짝 기울어진 쿠터뷰 카메라.
    /// 카트라이더 / 마리오카트 스타일.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("타겟")]
        [SerializeField] Transform _target;

        [Header("쿠터뷰 옵션")]
        [Tooltip("카메라가 타겟 위로 떠다니는 높이")]
        [SerializeField] float _height       = 14f;
        [Tooltip("타겟 진행 방향 반대로 몇 유닛 뒤로릴지")]
        [SerializeField] float _behindOffset  = 6f;
        [Tooltip("수평 기울기 (0 = 완전 탑뷰, 45 = 쿠터뷰, 60 = 카트라이더스타)")] 
        [SerializeField] float _tiltAngle     = 55f;
        [SerializeField] float _smoothTime    = 0.18f;
        [SerializeField] float _rotSmoothTime = 0.12f;

        Vector3 _posVel;
        float   _rotVel;

        public void SetTarget(Transform t) => _target = t;

        void LateUpdate()
        {
            if (_target == null) return;

            // 스카이에서 바라보는 방향 (타겟의 forward 반대로 반교)
            Vector3 behind = -_target.forward;
            behind.y = 0f;
            if (behind.sqrMagnitude < 0.01f) behind = Vector3.back;
            behind.Normalize();

            Vector3 goalPos = _target.position
                            + behind * _behindOffset
                            + Vector3.up * _height;

            transform.position = Vector3.SmoothDamp(
                transform.position, goalPos, ref _posVel, _smoothTime);

            // X 축 기울기 (쿠터뷰)
            Quaternion goalRot = Quaternion.Euler(_tiltAngle, _target.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, goalRot, Time.deltaTime / _rotSmoothTime);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (_target == null) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, _target.position);
        }
#endif
    }
}
