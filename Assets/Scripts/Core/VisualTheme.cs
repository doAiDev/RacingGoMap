using UnityEngine;

namespace RacingGoMap.Core
{
    /// <summary>
    /// 게임 전체에서 공유하는 파스텔 카트라이더 콼러 팔레트.
    /// </summary>
    [CreateAssetMenu(fileName = "VisualTheme", menuName = "RacingGoMap/Visual Theme")]
    public class VisualTheme : ScriptableObject
    {
        [Header("환경")]
        public Color grassLight   = new Color(0.56f, 0.82f, 0.48f); // #8FD17A
        public Color grassDark    = new Color(0.44f, 0.72f, 0.36f); // #70B75C
        public Color trackColor   = new Color(0.80f, 0.80f, 0.83f); // #CCCCCC
        public Color trackLine    = new Color(1f,    1f,    1f,   1f);
        public Color curbOrange   = new Color(1f,    0.42f, 0.21f); // #FF6B35
        public Color curbWhite    = Color.white;
        public Color skyColor     = new Color(0.56f, 0.82f, 0.96f); // #8FD1F5

        [Header("차량 컴러 (기본 팔레트)")]
        public Color carRed       = new Color(1f,    0.30f, 0.30f); // #FF4D4D
        public Color carBlue      = new Color(0.28f, 0.56f, 1f   ); // #478FFF
        public Color carYellow    = new Color(1f,    0.85f, 0.20f); // #FFD933
        public Color carGreen     = new Color(0.30f, 0.80f, 0.40f); // #4DCC66
        public Color carPurple    = new Color(0.70f, 0.30f, 1f   ); // #B34DFF

        [Header("UI")]
        public Color uiPrimary    = new Color(1f,    0.42f, 0.21f); // 오렌지 액센트
        public Color uiBackground = new Color(0.12f, 0.12f, 0.18f); // 다크 파넥
        public Color uiText       = Color.white;
        public Color uiSubText    = new Color(0.75f, 0.75f, 0.75f);
    }
}
