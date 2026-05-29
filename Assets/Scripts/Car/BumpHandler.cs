using UnityEngine;

namespace RacingGoMap.Car
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CarController))]
    public class BumpHandler : MonoBehaviour
    {
        Rigidbody2D   _rb;
        CarController _car;

        void Awake()
        {
            _rb  = GetComponent<Rigidbody2D>();
            _car = GetComponent<CarController>();
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            var other = col.gameObject.GetComponent<CarController>();
            if (other == null) return;

            Vector2 dir       = (col.transform.position - transform.position).normalized;
            float   myForce   = _car.Data != null ? _car.Data.bumpForce      : 3f;
            float   theirRes  = other.Data  != null ? other.Data.bumpResistance : 1f;

            var otherRb = col.gameObject.GetComponent<Rigidbody2D>();
            if (otherRb != null)
                otherRb.AddForce(dir * myForce / theirRes, ForceMode2D.Impulse);

            // 반동
            _rb.AddForce(-dir * myForce * 0.25f, ForceMode2D.Impulse);
        }
    }
}
