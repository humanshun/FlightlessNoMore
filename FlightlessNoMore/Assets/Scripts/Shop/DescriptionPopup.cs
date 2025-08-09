using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupRoot; // ポップアップの本体
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Transform ContentTransform;
    [SerializeField] private StatusBar statusBar; // ステータス情報のプレハブ
    [SerializeField] private Button partSetButton; // パーツセットボタン
    [SerializeField] private CustomPlayer customPlayer; // プレイヤーのカスタムオブジェクト
    private GameObject status;
    public void Show(PartData part, CurrentPartPopup currentPartPopup)
    {
        customPlayer = GameManager.Instance.Player;

        popupRoot.SetActive(true);
        nameText.text = part.partName;

        // 古いステータスを削除
        foreach (Transform child in ContentTransform)
        {
            Destroy(child.gameObject);
        }

        // パーツタイプに応じて表示
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
        partSetButton.onClick.AddListener(() => ButtonClick(part, currentPartPopup));
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


    public void Hide()
    {
        popupRoot.SetActive(false);
    }

    public void ButtonClick(PartData part, CurrentPartPopup currentPartPopup)
    {
        AudioManager.Instance.PlaySFX("SE_ButtonLow");
        string currentName = PlayerData.Instance.GetCurrentPartName(part.partType);
        if (!string.IsNullOrEmpty(currentName) && currentName != part.partName)
        {
            Debug.Log($"{part.partType}の現在の装備{currentName}を削除します");
            PlayerData.Instance.RemoveCurrentPart(part.partType);
        }

        PlayerData.Instance.SaveCurrentPart(part);
        currentPartPopup.Setup(part);

        customPlayer.SetupAll();
    }
}
