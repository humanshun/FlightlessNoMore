using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : BasePopup
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button closeButton2;

    private void Start()
    {
        // ボタンにイベントリスナーを追加
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        closeButton2.onClick.AddListener(OnCloseButtonClicked);
    }
    private void OnCloseButtonClicked()
    {
        AudioManager.Instance.PlaySFX("SE_Close");
        Destroy(gameObject); // SetActive(false)ではなくDestroyで削除
    }
}
