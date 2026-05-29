using UnityEngine;
using RacingGoMap.UI;

namespace RacingGoMap.Car
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CarController : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] float _moveSpeed    = 5f;
        [SerializeField] float _rotateSpeed  = 150f;

        Rigidbody2D      _rb;
        DynamicJoystick  _joystick;
        CarData          _data;
        bool             _canMove;

        public bool     IsLocal { get; set; }
        public CarData  Data    => _data;

        void Awake()
        {
            _rb             = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.drag         = 2f;
            _rb.angularDrag  = 5f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        void Start()
        {
            _joystick = FindObjectOfType<DynamicJoystick>();
        }

        public void SetCarData(CarData data)
        {
            _data = data;
            if (_spriteRenderer != null && data.carSprite != null)
            {
                _spriteRenderer.sprite = data.carSprite;
                _spriteRenderer.color  = data.primaryColor;
            }
        }

        public void SetCanMove(bool value) => _canMove = value;

        void FixedUpdate()
        {
            if (!IsLocal || !_canMove) return;

            float steer = _joystick != null ? _joystick.Horizontal : 0f;
            _rb.MoveRotation(_rb.rotation - steer * _rotateSpeed * Time.fixedDeltaTime);
            _rb.velocity = transform.up * _moveSpeed;
        }
    }
}
