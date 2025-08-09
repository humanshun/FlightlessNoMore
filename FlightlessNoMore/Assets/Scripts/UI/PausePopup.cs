using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausePopup : BasePopup
{
    [SerializeField] private Canvas myCanvas;
    [SerializeField] private GameObject pausePopup;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button titleButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject settingPopupPrefab;
    private GameObject settingsPopupInstance;
    private GameObject popupInstance;

    private Button[] closeButton;
    private void Start()
    {
        popupInstance = pausePopup;
        GameManager.Instance.pausePopup = this;
        popupInstance.gameObject.SetActive(false);

        resumeButton.onClick.AddListener(OnClickResume);
        restartButton.onClick.AddListener(OnClickRestart);
        titleButton.onClick.AddListener(OnClickTitle);
        quitButton.onClick.AddListener(OnClickQuit);
        settingsButton.onClick.AddListener(OnClickSettings);
    }
    public bool IsShowing()
    {
        return popupInstance != null && popupInstance.activeSelf;
    }
    public void Show()
    {
        if (popupInstance != null && !popupInstance.activeSelf)
        {
            popupInstance.SetActive(true);
        }
    }

    public void Hide()
    {
        if (popupInstance != null && popupInstance.activeSelf)
        {
            popupInstance.SetActive(false);
        }
    }
    private void OnClickResume()
    {
        AudioManager.Instance.PlaySFX("SE_ButtonLow");
        Hide(); // 時間再開
    }

    private async void OnClickRestart()
    {
        Time.timeScale = 1f; // 時間再開
        AudioManager.Instance.StopAllLoopSFX();
        AudioManager.Instance.PlaySFX("SE_ButtonLow");
        await SceneChanger.Instance.ChangeScene(SceneManager.GetActiveScene().name, 0.5f, 0.5f);
    }

    private async void OnClickTitle()
    {
        Time.timeScale = 1f; // 時間再開
        AudioManager.Instance.StopAllLoopSFX();
        AudioManager.Instance.PlaySFX("SE_ButtonLow");
        await SceneChanger.Instance.ChangeScene("Title", 0.5f, 0.5f);
    }

    private void OnClickQuit()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.PlaySFX("SE_ButtonLow");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // エディタ再生を止める
#else
        Application.Quit(); // ビルド版ではアプリ終了
#endif
    }
    private void OnClickSettings()
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
