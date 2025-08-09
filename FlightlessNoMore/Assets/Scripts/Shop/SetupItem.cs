using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetupItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button costButton;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite active;
    [SerializeField] private Sprite inactive;
    [SerializeField] private Transform ContentTransform;
    [SerializeField] private StatusBar statusBar;

    private PartData currentPart;
    private GameObject status;

    void Start()
    {
        // 購入ボタンにクリックイベント登録
        if (costButton != null)
        {
            costButton.onClick.AddListener(OnCostButtonClick);
        }
    }

    public void Setup(PartData part)
    {
        currentPart = part;

        // UIに情報を反映
        nameText.text = part.partName;
        costText.text = $"¥{part.partCost.value}";
        descriptionText.text = part.partDescription;

        if (iconImage != null)
        {
            iconImage.sprite = part.partIconImage;
            iconImage.preserveAspect = true;
        }

        // ステータスバーを反映
        UpdateStatusBars(part);

        // 購入状態に応じてボタンの状態を更新
        UpdateButtonVisual();
    }

    private void UpdateStatusBars(PartData part)
    {
        // 既存のステータスバーを削除
        foreach (Transform child in ContentTransform)
        {
            Destroy(child.gameObject);
        }

        switch (part.partType)
        {
            case PartType.Body:
                BodyData body = (BodyData)part;
                AddStatus(PartType.Body, StatusType.Weight, body.weight.displayName, body.weight.value);
                AddStatus(PartType.Body, StatusType.HP, body.hp.displayName, body.hp.value);
                AddStatus(PartType.Body, StatusType.AirResistance, body.airResistance.displayName, body.airResistance.value * 1000f);
                break;
            case PartType.Rocket:
                RocketData rocket = (RocketData)part;
                AddStatus(PartType.Rocket, StatusType.Weight, rocket.weight.displayName, rocket.weight.value);
                AddStatus(PartType.Rocket, StatusType.Thrust, rocket.jetThrust.displayName, rocket.jetThrust.value);
                AddStatus(PartType.Rocket, StatusType.RocketTime, rocket.jetTime.displayName, rocket.jetTime.value);
                break;
            case PartType.Tire:
                TireData tire = (TireData)part;
                AddStatus(PartType.Tire, StatusType.Weight, tire.weight.displayName, tire.weight.value);
                AddStatus(PartType.Tire, StatusType.AirResistance, tire.airResistance.displayName, tire.airResistance.value * 1000f);
                AddStatus(PartType.Tire, StatusType.Acceleration, tire.torque.displayName, tire.torque.value);
                break;
            case PartType.Wing:
                WingData wing = (WingData)part;
                AddStatus(PartType.Wing, StatusType.Weight, wing.weight.displayName, wing.weight.value);
                AddStatus(PartType.Wing, StatusType.AirResistance, wing.airResistance.displayName, wing.airResistance.value * 1000f);
                AddStatus(PartType.Wing, StatusType.AirControl, wing.airControl.displayName, wing.airControl.value);
                break;
        }
    }

    private void AddStatus(PartType partType, StatusType statusType, string displayName, float value)
    {
        float maxValue = GetMaxValue(partType, statusType);
        status = Instantiate(statusBar.gameObject, ContentTransform);
        StatusBar statusBarInstance = status.GetComponent<StatusBar>();
        statusBarInstance.Setup(partType, statusType, displayName, value, maxValue);
    }

    private float GetMaxValue(PartType partType, StatusType statusType)
    {
        switch (statusType)
        {
            case StatusType.Weight: return 100f;
            case StatusType.AirResistance: return 150f;
            case StatusType.HP: return 50f;
            case StatusType.Thrust: return 50f;
            case StatusType.RocketTime: return 30f;
            case StatusType.Acceleration: return 50f;
            case StatusType.AirControl: return 100f;
            default: return 1f;
        }
    }

    private void UpdateButtonVisual()
    {
        if (costButton != null)
        {
            bool isPurchased = PlayerData.Instance.IsPartPurchased(currentPart.partName);
            costButton.image.sprite = isPurchased ? inactive : active;
            costButton.interactable = !isPurchased;
        }
    }

    private void OnCostButtonClick()
    {
        // コインを消費して購入を試みる
        if (PlayerData.Instance.TryBuyPart(currentPart, currentPart.partCost.value))
        {
            PlayerData.Instance.SavePurchasedPart(currentPart); // JSONに保存
            UpdateButtonVisual(); // 最新状態をUIに反映
            AudioManager.Instance.PlaySFX("SE_Cash");

            Debug.Log("アイテムの購入に成功しました！");
        }
        else
        {
            Debug.Log("このアイテムを買うにはコインが足りません。");
        }
    }
}
