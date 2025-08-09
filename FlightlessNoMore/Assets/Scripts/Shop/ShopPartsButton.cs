using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopPartsButton : MonoBehaviour
{
    [SerializeField] Button bodyButton;
    [SerializeField] Button rocketButton;
    [SerializeField] Button tireButton;
    [SerializeField] Button wingButton;
    [SerializeField] Button closeButton;
    [SerializeField] GameObject bodyButtonImage;
    [SerializeField] GameObject rocketButtonImage;
    [SerializeField] GameObject tireButtonImage;
    [SerializeField] GameObject wingButtonImage;
    [SerializeField] Sprite enabledSprite;
    [SerializeField] Sprite disabledSprite;
    [SerializeField] Transform contentTransform;
    public ShopData shopData;
    public GameObject itemPrefab;
    void Start()
    {
        bodyButton.onClick.AddListener(() => OnButtonClicked(bodyButton, PartType.Body));
        rocketButton.onClick.AddListener(() => OnButtonClicked(rocketButton, PartType.Rocket));
        tireButton.onClick.AddListener(() => OnButtonClicked(tireButton, PartType.Tire));
        wingButton.onClick.AddListener(() => OnButtonClicked(wingButton, PartType.Wing));
        closeButton.onClick.AddListener(OnCloseButtonClick);

        OnButtonClicked(bodyButton, PartType.Body); // 初期状態の更新

        gameObject.SetActive(false);
    }

    void OnButtonClicked(Button selectedButton, PartType selectedType)
    {
        AudioManager.Instance.PlaySFX("SE_ButtonLow");
        (Button, GameObject)[] buttons =
        {
            (bodyButton, bodyButtonImage),
            (rocketButton, rocketButtonImage),
            (tireButton, tireButtonImage),
            (wingButton, wingButtonImage)
        };

        foreach (var (button, imageObj) in buttons)
        {
            bool isSelected = button == selectedButton;
            button.interactable = !isSelected;

            if (imageObj != null)
            {
                Image img = imageObj.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = isSelected ? enabledSprite : disabledSprite;
                }
            }
        }
        // 選ばれたカテゴリのパーツを表
        UpdatePartListUI(selectedType);


    }
    void UpdatePartListUI(PartType partType)
    {
        // Contentを全クリア
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        List<PartData> parts = null;

        switch (partType)
        {
            case PartType.Body:
                parts = shopData.typeBody.bodyParts;
                break;
            case PartType.Rocket:
                parts = shopData.typeRocket.rocketParts;
                break;
            case PartType.Tire:
                parts = shopData.typeTire.tireParts;
                break;
            case PartType.Wing:
                parts = shopData.typeWing.wingParts;
                break;
        }

        foreach (PartData part in parts)
        {
            GameObject item = Instantiate(itemPrefab, contentTransform);
            SetupItem setupItem = item.GetComponent<SetupItem>();
            if (setupItem != null)
            {
                setupItem.Setup(part); // ← ここでデータを渡す
            }
        }
    }

    private void OnCloseButtonClick()
    {
        AudioManager.Instance.PlaySFX("SE_Close");
        gameObject.SetActive(false);
    }
}
