using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatusBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI statusNameText;
    [SerializeField] private TextMeshProUGUI statusValueText;
    [SerializeField] private RectTransform statusValue;

    private PartType partType;
    private StatusType statusType;

    public void Setup(PartType part, StatusType status, string displayName, float value, float maxValue)
    {
        partType = part;
        statusType = status;
        statusNameText.text = displayName;
        statusValueText.text = value.ToString("F1");
        float normalized = Mathf.Clamp01(value / maxValue);
        statusValue.anchorMax = new Vector2(normalized, statusValue.anchorMax.y);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string tooltip = GetTooltip(partType, statusType);
        TooltipManager.Instance.Show(tooltip, Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.Hide();
    }

    private string GetTooltip(PartType part, StatusType status)
    {
        switch (status)
        {
            case StatusType.Weight:
                return "数値が高いほど衝突時の減速率が小さくなります。";
            case StatusType.AirResistance:
                return "数値が高いほど減速しやすくなります";
            case StatusType.HP:
                return "数値が高いほど壊れにくくなります。";
            case StatusType.Thrust:
                return "数値が高いほど加速力が高くなります。";
            case StatusType.RocketTime:
                return "数値が高いほど噴射時間が長くなります。";
            case StatusType.Acceleration:
                return "数値が高いほど地面での加速力が高くなります。";
            case StatusType.AirControl:
                return "数値が高いほど空中での姿勢制御がしやすくなります。";
            default:
                return "ステータス情報がありません。";
        }
    }
}
