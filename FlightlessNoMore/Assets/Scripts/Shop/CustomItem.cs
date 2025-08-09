using Ricimi;
using UnityEngine;
using UnityEngine.UI;

public class CustomItem : MonoBehaviour
{
    [SerializeField] private Button itemButton;
    [SerializeField] private Image iconImage; // アイコン画像
    [SerializeField] private GameObject changeImage;

    private PartData currentPart;
    private DescriptionPopup descriptionPopup; // ポップアップの参照
    private PartCustom parentCustom; // 親の参照

    public void Setup(PartData part, DescriptionPopup popup, CurrentPartPopup currentPartPopup, PartType selectedType, PartCustom parent)
    {
        currentPart = part;
        descriptionPopup = popup;
        parentCustom = parent;

        iconImage.sprite = part.partIconImage; // アイコン画像を設定
        iconImage.preserveAspect = true; // アスペクト比を保持する

        itemButton.onClick.RemoveAllListeners(); // 既存のリスナーを削除
        itemButton.onClick.AddListener(() => OnItemClick(currentPartPopup)); // ボタンにクリックイベントを追加

        changeImage.SetActive(part.partType != selectedType);
    }
    public void OnItemClick(CurrentPartPopup currentPartPopup)
    {
        AudioManager.Instance.PlaySFX("SE_ButtonLow");
        descriptionPopup.Show(currentPart, currentPartPopup); // ポップアップを表示
        parentCustom.OnCustomItemClick(this);
    }

    public void SetChangeImageActive(bool active)
    {
        changeImage.SetActive(active); // 変更画像の表示/非表示を設定
    }
}
