using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;

public class PartCustom : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button bodyPopupButton;
    [SerializeField] private Button rocketPopupButton;
    [SerializeField] private Button tirePopupButton;
    [SerializeField] private Button wingPopupButton;
    [SerializeField] private CurrentPartPopup bodyCurrentPartPopup;
    [SerializeField] private CurrentPartPopup rocketCurrentPartPopup;
    [SerializeField] private CurrentPartPopup tireCurrentPartPopup;
    [SerializeField] private CurrentPartPopup wingCurrentPartPopup;
    [SerializeField] private Image bodyPopupChangeImage;
    [SerializeField] private Image rocketPopupChangeImage;
    [SerializeField] private Image tirePopupChangeImage;
    [SerializeField] private Image wingPopupChangeImage;
    [SerializeField] Transform contentTransform;
    [SerializeField] private GameObject partCostom;
    [SerializeField] private DescriptionPopup descriptionPopup;
    [SerializeField] private ShopData shopData;
    [SerializeField] private GameObject itemPrefab;

    private List<CustomItem> currentItems = new List<CustomItem>();

    void Start()
    {
        // ボタンのクリックイベントを設定
        closeButton.onClick.AddListener(CloseButtonClick); // Contentを非アクティブにする
        bodyPopupButton.onClick.AddListener(() => OnButtonClick(bodyPopupButton, PartType.Body));
        rocketPopupButton.onClick.AddListener(() => OnButtonClick(rocketPopupButton, PartType.Rocket));
        tirePopupButton.onClick.AddListener(() => OnButtonClick(tirePopupButton, PartType.Tire));
        wingPopupButton.onClick.AddListener(() => OnButtonClick(wingPopupButton, PartType.Wing));

        bodyPopupChangeImage.gameObject.SetActive(false);
        rocketPopupChangeImage.gameObject.SetActive(false);
        tirePopupChangeImage.gameObject.SetActive(false);
        wingPopupChangeImage.gameObject.SetActive(false);

        descriptionPopup.gameObject.SetActive(false); // 初期状態で非表示
        partCostom.SetActive(false); // 初期状態で非表示

        // 装備中パーツをUIに反映
        SetupEquippedParts();
    }
    private void OnButtonClick(Button selectedButton, PartType selectedType)
    {
        AudioManager.Instance.PlaySFX("SE_ButtonLow");
        partCostom.SetActive(true); // Contentをアクティブにする
        // Contentを全クリア
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        currentItems.Clear(); // 現在のアイテムリストをクリア

        List<PartData> parts = null;
        CurrentPartPopup currentPartPopup = null;

        switch (selectedType)
        {
            case PartType.Body:
                parts = shopData.typeBody.bodyParts;
                currentPartPopup = bodyCurrentPartPopup;
                break;
            case PartType.Rocket:
                parts = shopData.typeRocket.rocketParts;
                currentPartPopup = rocketCurrentPartPopup;
                break;
            case PartType.Tire:
                parts = shopData.typeTire.tireParts;
                currentPartPopup = tireCurrentPartPopup;
                break;
            case PartType.Wing:
                parts = shopData.typeWing.wingParts;
                currentPartPopup = wingCurrentPartPopup;
                break;
        }

        foreach (PartData part in parts)
        {
            if (PlayerData.Instance.IsPartPurchased(part.partName))
            {
                GameObject item = Instantiate(itemPrefab, contentTransform);
                CustomItem setupItem = item.GetComponent<CustomItem>();
                if (setupItem != null)
                {
                    setupItem.Setup(part, descriptionPopup, currentPartPopup, selectedType, this); // データを渡してセットアップ
                    currentItems.Add(setupItem); // 現在のアイテムリストに追加
                }
            }
        }
        SetPopupImageVisibility(selectedType);
    }
    private void SetupEquippedParts()
    {
        Dictionary<PartType, string> parts = PlayerData.Instance.GetAllCurrentParts();

        foreach (var kvp in parts)
        {
            PartType type = kvp.Key;
            string resourceName = kvp.Value;
            PartData part = Resources.Load<PartData>($"Parts/{resourceName}");
            if (part == null)
            {
                Debug.LogWarning($"{type} の {resourceName} が見つかりませんでした。");
                continue;
            }

            switch (type)
            {
                case PartType.Body:
                    bodyCurrentPartPopup.Setup(part);
                    break;
                case PartType.Rocket:
                    rocketCurrentPartPopup.Setup(part);
                    break;
                case PartType.Tire:
                    tireCurrentPartPopup.Setup(part);
                    break;
                case PartType.Wing:
                    wingCurrentPartPopup.Setup(part);
                    break;
            }
        }
    }
    private void SetPopupImageVisibility(PartType selectedType)
    {
        bodyPopupChangeImage.gameObject.SetActive(true);
        rocketPopupChangeImage.gameObject.SetActive(true);
        tirePopupChangeImage.gameObject.SetActive(true);
        wingPopupChangeImage.gameObject.SetActive(true);

        switch (selectedType)
        {
            case PartType.Body:
                bodyPopupChangeImage.gameObject.SetActive(false);
                break;
            case PartType.Rocket:
                rocketPopupChangeImage.gameObject.SetActive(false);
                break;
            case PartType.Tire:
                tirePopupChangeImage.gameObject.SetActive(false);
                break;
            case PartType.Wing:
                wingPopupChangeImage.gameObject.SetActive(false);
                break;
            default:
                Debug.LogError("Invalid PartType selected.");
                break;
        }
    }

    private void CloseButtonClick()
    {
        AudioManager.Instance.PlaySFX("SE_Close");
        partCostom.SetActive(false); // Contentを非アクティブにする
        bodyPopupChangeImage.gameObject.SetActive(false);
        rocketPopupChangeImage.gameObject.SetActive(false);
        tirePopupChangeImage.gameObject.SetActive(false);
        wingPopupChangeImage.gameObject.SetActive(false);
    }

    public void OnCustomItemClick(CustomItem clickedItem)
    {
        // クリックされたアイテム以外の変更画像を非表示にする
        foreach (var item in currentItems)
        {
            item.SetChangeImageActive(item != clickedItem); //自分だけ非表示、他は表示
        }
    }
}
