using Ricimi;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;

public class ShopCanvasManager : MonoBehaviour
{
    [SerializeField] private Canvas myCanvas;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject partShopPopup;
    [SerializeField] private Button shopCloseButton;
    [SerializeField] private GameObject partCustomPopup;
    [SerializeField] private Button customCloseButton;

    [SerializeField] private Button bodyButton;
    [SerializeField] private Button rocketButton;
    [SerializeField] private Button tireButton;
    [SerializeField] private Button wingButton;

    [SerializeField] private GameObject desctiptionPopupPrefab;
    [SerializeField] private GameObject coinText;
    public GameObject settingPopupPrefab;
    private GameObject settingsPopupInstance;

    private void Start()
    {
        // ボタンにイベントリスナーを追加
        shopButton.onClick.AddListener(OnShopButtonClicked);
        shopCloseButton.onClick.AddListener(OnShopCloseButtonClicked);
        playButton.onClick.AddListener(OnPlayButtonClicked);
        settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        customCloseButton.onClick.AddListener(OnCustomCloseButtonClicked);

        bodyButton.onClick.AddListener(CustomButtonOnClick);
        rocketButton.onClick.AddListener(CustomButtonOnClick);
        tireButton.onClick.AddListener(CustomButtonOnClick);
        wingButton.onClick.AddListener(CustomButtonOnClick);

    }

    private void OnShopButtonClicked()
    {
        AudioManager.Instance.PlaySFX("SE_ButtonClick");
        partShopPopup.SetActive(true);
        partCustomPopup.SetActive(false);
        shopButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        coinText.SetActive(true);
    }

    private void OnShopCloseButtonClicked()
    {
        AudioManager.Instance.PlaySFX("SE_Close");
        partShopPopup.SetActive(false);
        partCustomPopup.SetActive(true);
        shopButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        coinText.SetActive(true);
    }

    private void OnCustomCloseButtonClicked()
    {
        AudioManager.Instance.PlaySFX("SE_Close");
        shopButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        desctiptionPopupPrefab.SetActive(false);
        coinText.SetActive(true);
    }

    private void CustomButtonOnClick()
    {
        AudioManager.Instance.PlaySFX("SE_ButtonClick");
        shopButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        desctiptionPopupPrefab.SetActive(false);
        coinText.SetActive(false);
    }

    private async void OnPlayButtonClicked()
    {
        AudioManager.Instance.PlaySFX("SE_ButtonClick");
        await SceneChanger.Instance.ChangeScene("InGame", 1.0f, 3.0f);
    }

    private void OnSettingsButtonClicked()
    {
        AudioManager.Instance.PlaySFX("SE_ButtonClick");

        // すでに存在している場合は再アクティブ化だけ
        if (settingsPopupInstance != null)
        {
            if (!settingsPopupInstance.activeSelf)
            {
                settingsPopupInstance.SetActive(true);
            }
            return;
        }

        // 存在していなければ生成
        settingsPopupInstance = Instantiate(settingPopupPrefab);
        settingsPopupInstance.transform.SetParent(myCanvas.transform, false);
        settingsPopupInstance.transform.localScale = Vector3.one;
    }
}
