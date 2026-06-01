using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RacingGoMap.Car;
using RacingGoMap.Core;
using RacingGoMap.Player;

namespace RacingGoMap.UI
{
    public class GarageUI : MonoBehaviour
    {
        [Header("목록")]
        [SerializeField] Transform  _listContent;

        [Header("미리보기 패널")]
        [SerializeField] Image    _previewImg;
        [SerializeField] TMP_Text _carNameText;
        [SerializeField] TMP_Text _bumpForceText;
        [SerializeField] TMP_Text _priceText;
        [SerializeField] Button   _buyButton;
        [SerializeField] Button   _equipButton;
        [SerializeField] TMP_Text _buyFeedback;

        [Header("HUD")]
        [SerializeField] TMP_Text _pointsText;
        [SerializeField] Button   _backButton;

        CarData _selected;

        void Start()
        {
            _backButton.onClick.AddListener(() => SceneLoader.Instance.Load(SceneLoader.Scenes.MainMenu));
            _buyButton.onClick.AddListener(BuyCar);
            _equipButton.onClick.AddListener(EquipCar);
            RefreshPoints();
            BuildList();
        }

        void RefreshPoints() =>
            _pointsText.text = $"{PlayerDataStore.Instance.Profile.totalPoints:N0} P";

        void BuildList()
        {
            foreach (Transform c in _listContent) Destroy(c.gameObject);

            var gd = RuntimeGameData.Instance;
            if (gd == null) return;

            foreach (var car in gd.Cars)
            {
                var item = new GameObject(car.carId);
                item.transform.SetParent(_listContent, false);

                var bg = item.AddComponent<Image>();
                bg.color = new Color(
                    Mathf.Lerp(car.primaryColor.r, 1f, 0.45f),
                    Mathf.Lerp(car.primaryColor.g, 1f, 0.45f),
                    Mathf.Lerp(car.primaryColor.b, 1f, 0.45f),
                    0.88f);

                var le = item.AddComponent<LayoutElement>();
                le.minHeight     = 72f;
                le.flexibleWidth = 1f;

                var btn = item.AddComponent<Button>();
                var cb  = btn.colors;
                cb.highlightedColor = Color.Lerp(car.primaryColor, Color.white, 0.3f);
                cb.pressedColor     = Color.Lerp(car.primaryColor, Color.black, 0.12f);
                btn.colors = cb;

                var labelGO = new GameObject("Label");
                labelGO.transform.SetParent(item.transform, false);
                var label    = labelGO.AddComponent<TextMeshProUGUI>();
                label.text      = car.carName;
                label.fontSize  = 22f;
                label.color     = new Color(0.15f, 0.15f, 0.15f);
                label.alignment = TextAlignmentOptions.Center;
                label.fontStyle = FontStyles.Bold;
                var labelRT     = labelGO.GetComponent<RectTransform>();
                labelRT.anchorMin = Vector2.zero;
                labelRT.anchorMax = Vector2.one;
                labelRT.offsetMin = labelRT.offsetMax = Vector2.zero;

                var carRef = car;
                btn.onClick.AddListener(() => SelectCar(carRef));
            }
        }

        void SelectCar(CarData car)
        {
            _selected = car;
            var profile = PlayerDataStore.Instance.Profile;
            bool owned   = profile.ownedCarIds.Contains(car.carId);
            bool equipped = profile.equippedCarId == car.carId;

            _previewImg.color   = car.primaryColor;
            if (car.carSprite != null) _previewImg.sprite = car.carSprite;
            _carNameText.text   = car.carName;
            _bumpForceText.text = $"박치기 힘: {car.bumpForce:F1}  /  내성: {car.bumpResistance:F1}";
            _priceText.text     = car.isDefault ? "기본 차량 (무료)" : $"{car.purchaseCost:N0} P";
            _buyFeedback.text   = "";

            _buyButton.gameObject.SetActive(!owned && !car.isDefault);
            _equipButton.gameObject.SetActive(owned && !equipped);
        }

        void BuyCar()
        {
            if (_selected == null) return;
            bool ok = PlayerDataStore.Instance.BuyCar(_selected.carId, _selected.purchaseCost);
            _buyFeedback.text = ok ? "구매 완료!" : "포인트가 부족합니다.";
            if (ok) { RefreshPoints(); SelectCar(_selected); }
        }

        void EquipCar()
        {
            if (_selected == null) return;
            PlayerDataStore.Instance.EquipCar(_selected.carId);
            SelectCar(_selected);
        }
    }
}
