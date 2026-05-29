using UnityEngine;

namespace RacingGoMap.Race
{
    /// <summary>
    /// 속도감을 주는 바퀴 트레일 이펙트.
    /// </summary>
    [RequireComponent(typeof(TrailRenderer))]
    public class SpeedTrail : MonoBehaviour
    {
        [SerializeField] float _minAlpha = 0f;
        [SerializeField] float _maxAlpha = 0.6f;
        [SerializeField] float _fullSpeedThreshold = 4.5f;

        TrailRenderer _trail;
        Rigidbody2D   _rb;

        void Awake()
        {
            _trail = GetComponent<TrailRenderer>();
            _rb    = GetComponentInParent<Rigidbody2D>();
        }

        void Update()
        {
            if (_rb == null) return;
            float t = Mathf.Clamp01(_rb.velocity.magnitude / _fullSpeedThreshold);
            var   c = _trail.startColor;
            c.a = Mathf.Lerp(_minAlpha, _maxAlpha, t);
            _trail.startColor = c;
            c = _trail.endColor;
            c.a = 0f;
            _trail.endColor = c;
        }
    }
}
