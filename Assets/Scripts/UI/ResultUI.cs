using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RacingGoMap.Core;
using RacingGoMap.Player;
using RacingGoMap.Race;

namespace RacingGoMap.UI
{
    public class ResultUI : MonoBehaviour
    {
        [SerializeField] Transform  _listContent;
        [SerializeField] TMP_Text   _pointsEarnedText;
        [SerializeField] Button     _playAgainButton;
        [SerializeField] Button     _mainMenuButton;

        static readonly int[] Rewards = { 100, 60, 40, 20, 10 };

        void Start()
        {
            _playAgainButton.onClick.AddListener(() => SceneLoader.Instance.Load(SceneLoader.Scenes.Matchmaking));
            _mainMenuButton.onClick.AddListener(()  => SceneLoader.Instance.Load(SceneLoader.Scenes.MainMenu));
        }

        public void ShowResults(List<LapTracker> order, LapTracker localPlayer)
        {
            foreach (Transform c in _listContent) Destroy(c.gameObject);

            int localPos = -1;
            for (int i = 0; i < order.Count; i++)
            {
                bool isLocal = order[i] == localPlayer;

                var item = new GameObject($"Result_{i}");
                item.transform.SetParent(_listContent, false);

                var bg = item.AddComponent<Image>();
                bg.color = isLocal
                    ? new Color(1.00f, 0.90f, 0.35f, 0.95f)
                    : new Color(1.00f, 1.00f, 1.00f, 0.75f);

                var le = item.AddComponent<LayoutElement>();
                le.minHeight     = 58f;
                le.flexibleWidth = 1f;

                var h = item.AddComponent<HorizontalLayoutGroup>();
                h.padding               = new RectOffset(15, 15, 6, 6);
                h.spacing               = 18f;
                h.childForceExpandWidth  = false;
                h.childForceExpandHeight = true;
                h.childAlignment         = TextAnchor.MiddleLeft;

                AppendLabel(item.transform, "Rank", $"{i + 1}위",
                    isLocal ? 26f : 22f,
                    isLocal ? new Color(0.55f, 0.25f, 0f) : new Color(0.3f, 0.3f, 0.3f),
                    isLocal ? FontStyles.Bold : FontStyles.Normal,
                    minWidth: 70f);

                string displayName = isLocal
                    ? $"{order[i].gameObject.name} (나)"
                    : order[i].gameObject.name;
                AppendLabel(item.transform, "Name", displayName,
                    22f, new Color(0.15f, 0.15f, 0.15f), FontStyles.Normal, flexWidth: 1f);

                if (isLocal) localPos = i;
            }

            if (localPos >= 0)
            {
                int reward = localPos < Rewards.Length ? Rewards[localPos] : 5;
                PlayerDataStore.Instance.AddPoints(reward);
                _pointsEarnedText.text = $"+{reward} P 획득!";
            }
            else
            {
                _pointsEarnedText.text = "레이스 완료!";
            }
        }

        static void AppendLabel(Transform parent, string goName, string text,
            float fontSize, Color color, FontStyles style,
            float minWidth = -1f, float flexWidth = -1f)
        {
            var go  = new GameObject(goName);
            go.transform.SetParent(parent, false);
            var tmp       = go.AddComponent<TextMeshProUGUI>();
            tmp.text      = text;
            tmp.fontSize  = fontSize;
            tmp.color     = color;
            tmp.fontStyle = style;
            tmp.alignment = TextAlignmentOptions.Left;

            var le = go.AddComponent<LayoutElement>();
            if (minWidth  >= 0f) { le.minWidth = minWidth; le.preferredWidth = minWidth; }
            if (flexWidth >= 0f)   le.flexibleWidth = flexWidth;
        }
    }
}
