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
        [Header("참조")]
        [SerializeField] CarDatabase _db;

        [Header("목록")]
        [SerializeField] Transform  _listContent;
        [SerializeField] GameObject _carItemPrefab;

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
            foreach (var car in _db.cars)
            {
                var item = Instantiate(_carItemPrefab, _listContent);
                var icon = item.transform.Find("Icon")?.GetComponent<Image>();
                if (icon != null && car.carSprite != null) icon.sprite = car.carSprite;
                item.GetComponentInChildren<TMP_Text>().text = car.carName;

                var btn = item.GetComponent<Button>();
                var c   = car;
                if (btn != null) btn.onClick.AddListener(() => SelectCar(c));
            }
        }

        void SelectCar(CarData car)
        {
            _selected = car;
            var profile = PlayerDataStore.Instance.Profile;
            bool owned   = profile.ownedCarIds.Contains(car.carId);
            bool equipped = profile.equippedCarId == car.carId;

            if (car.carSprite != null) _previewImg.sprite = car.carSprite;
            _carNameText.text  = car.carName;
            _bumpForceText.text = $"프로필: 박치기 {car.bumpForce:F1}  /  내성 {car.bumpResistance:F1}";
            _priceText.text    = car.isDefault ? "기본 차량" : $"{car.purchaseCost:N0} P";
            _buyFeedback.text  = "";

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
