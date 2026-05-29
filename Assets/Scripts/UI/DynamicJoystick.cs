using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RacingGoMap.UI
{
    /// <summary>
    /// 탕탑특공대 스타일 조이스틱:
    /// - 터치한 자리에 생성, 손 떼면 사라짐
    /// - 화면 왼쪽 영역에서만 작동
    /// </summary>
    public class DynamicJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] RectTransform _joystickRoot;   // 외부 원 (Background)
        [SerializeField] RectTransform _handle;          // 내부 핸들
        [SerializeField] float         _maxRadius = 70f;
        [SerializeField] float         _activeFraction = 0.5f; // 화면 왜쪽 이 비율만 입력

        Canvas    _canvas;
        int       _pointerId = -99;

        public float Horizontal { get; private set; }
        public float Vertical   { get; private set; }

        void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            HideJoystick();
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (_pointerId != -99) return;

            // 화면 왼쪽 영역만
            if (data.position.x > Screen.width * _activeFraction) return;

            _pointerId = data.pointerId;

            // 터치 지점으로 조이스틱 이동
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                data.position,
                _canvas.worldCamera,
                out Vector2 localPoint
            );
            _joystickRoot.anchoredPosition = localPoint;
            _joystickRoot.gameObject.SetActive(true);
            _handle.anchoredPosition = Vector2.zero;
        }

        public void OnDrag(PointerEventData data)
        {
            if (data.pointerId != _pointerId) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _joystickRoot,
                data.position,
                _canvas.worldCamera,
                out Vector2 delta
            );

            Vector2 clamped = Vector2.ClampMagnitude(delta, _maxRadius);
            _handle.anchoredPosition = clamped;

            Horizontal = clamped.x / _maxRadius;
            Vertical   = clamped.y / _maxRadius;
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (data.pointerId != _pointerId) return;
            Reset();
        }

        void Reset()
        {
            _pointerId  = -99;
            Horizontal  = 0f;
            Vertical    = 0f;
            HideJoystick();
        }

        void HideJoystick()
        {
            if (_joystickRoot != null)
                _joystickRoot.gameObject.SetActive(false);
        }
    }
}
