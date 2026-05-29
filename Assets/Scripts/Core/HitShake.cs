using System.Collections;
using UnityEngine;

namespace RacingGoMap.Core
{
    /// <summary>
    /// 박치기 시 짧은 카트라이더 스타일 흔들림 효과.
    /// </summary>
    public class HitShake : MonoBehaviour
    {
        [SerializeField] float _duration  = 0.18f;
        [SerializeField] float _magnitude = 0.08f;

        Vector3 _origin;
        bool    _shaking;

        public void Shake()
        {
            if (!_shaking) StartCoroutine(DoShake());
        }

        IEnumerator DoShake()
        {
            _shaking = true;
            _origin  = transform.localPosition;
            float elapsed = 0f;

            while (elapsed < _duration)
            {
                float x = Random.Range(-1f, 1f) * _magnitude;
                float y = Random.Range(-1f, 1f) * _magnitude;
                transform.localPosition = _origin + new Vector3(x, y, 0f);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = _origin;
            _shaking = false;
        }
    }
}
