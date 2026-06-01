using UnityEngine;
using UnityEngine.UI;
using RacingGoMap.Core;

namespace RacingGoMap.Art
{
    /// <summary>
    /// 게임 시작 시 모든 프로시저럴 스프라이트를 생성하고
    /// VisualTheme 팔레트에 맞게 씬에 적용합니다.
    /// </summary>
    public class ArtInitializer : MonoBehaviour
    {
        [Header("차량 컬러 (5인 기준)")]
        [SerializeField] Color[] _playerColors = new Color[]
        {
            new(1f,    0.30f, 0.30f), // P1 레드
            new(0.28f, 0.56f, 1f   ), // P2 블루
            new(1f,    0.85f, 0.20f), // P3 옐로
            new(0.30f, 0.80f, 0.40f), // P4 그린
            new(0.70f, 0.30f, 1f   ), // P5 퍼플
        };

        public static ArtInitializer Instance { get; private set; }

        Sprite[] _carSprites;
        Sprite   _roadSprite;
        Sprite   _grassSprite;
        Sprite   _finishSprite;

        void Awake()
        {
            Instance = this;
            GenerateAll();
        }

        void GenerateAll()
        {
            // 차량
            _carSprites = new Sprite[_playerColors.Length];
            for (int i = 0; i < _playerColors.Length; i++)
                _carSprites[i] = CarSpriteBuilder.Build(_playerColors[i]);

            // 트랙 타일
            _roadSprite   = TrackTileBuilder.BuildRoadStraight();
            _grassSprite  = TrackTileBuilder.BuildGrass();
            _finishSprite = TrackTileBuilder.BuildFinishLine();

            // 조이스틱 스킨 자동 적용
            ApplyJoystickSkin();
        }

        public Sprite GetCarSprite(int playerIndex)
        {
            if (_carSprites == null || playerIndex >= _carSprites.Length) return null;
            return _carSprites[playerIndex];
        }

        public Color GetPlayerColor(int index)
            => index < _playerColors.Length ? _playerColors[index] : Color.white;

        void ApplyJoystickSkin()
        {
            var images = FindObjectsOfType<Image>();
            Image bg = null, handle = null;
            foreach (var img in images)
            {
                if (img.name == "JoystickBackground") bg = img;
                if (img.name == "JoystickHandle")     handle = img;
            }
            if (bg != null && handle != null)
                UIStyleBuilder.ApplyJoystickSkin(bg, handle);
        }

        public Sprite RoadSprite   => _roadSprite;
        public Sprite GrassSprite  => _grassSprite;
        public Sprite FinishSprite => _finishSprite;
    }
}
