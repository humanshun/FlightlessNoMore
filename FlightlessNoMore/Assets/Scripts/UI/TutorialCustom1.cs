using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TutorialCustom1 : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject caretDownImage;

    // 必要ステップの進行チェック
    private bool checkStep2 = false;
    private bool checkStep3 = false;
    private bool checkStep4 = false;
    private bool checkStep5 = false;

    [Header("ショップボタン")]
    [SerializeField] private Button shopButton;
    [SerializeField] private Button bodyButton;
    [SerializeField] private Button rocketButton;
    [SerializeField] private Button tireButton;
    [SerializeField] private Button wingButton;
    [SerializeField] private Button closeButton;

    [Header("倉庫ボタン")]
    [SerializeField] private Button bodyCustomButton;
    [SerializeField] private Button rocketCustomButton;
    [SerializeField] private Button tireCustomButton;
    [SerializeField] private Button wingCustomButton;
    [SerializeField] private Button customCloseButton;
    [SerializeField] private Button setButton;

    [Header("矢印")]
    [SerializeField] private GameObject[] arrowPrefab;

    private TutorialStep currentStep = TutorialStep.Step1;

    private void OnRocketCustomClicked() => SetButtonAction(RocketSet);
    private void OnTireCustomClicked()   => SetButtonAction(TireSet);
    private void OnWingCustomClicked()   => SetButtonAction(WingSet);

    private const string KEY_CustomTutorialCompleted = "CustomTutorialCompleted";

    private enum TutorialStep
    {
        Step1, Step2, Step3, Step4, Step5
    }

    private void OnEnable()
    {
        PlayerData.OnAnyPartEquipped += CheckAllPartsEquipped;
    }

    private void OnDisable()
    {
        PlayerData.OnAnyPartEquipped -= CheckAllPartsEquipped;

        if (nextButton != null) nextButton.onClick.RemoveListener(OnNextClicked);
        if (bodyCustomButton != null) bodyCustomButton.onClick.RemoveListener(OnBodyCustomClicked);
        if (setButton != null) setButton.onClick.RemoveListener(OnSetButtonClicked);

        if (rocketCustomButton != null) rocketCustomButton.onClick.RemoveListener(OnRocketCustomClicked);
        if (tireCustomButton != null)   tireCustomButton.onClick.RemoveListener(OnTireCustomClicked);
        if (wingCustomButton != null)   wingCustomButton.onClick.RemoveListener(OnWingCustomClicked);

        if (customCloseButton != null) customCloseButton.onClick.RemoveListener(OnCloseButtonClicked);
    }

    private void Start()
    {
        // ★起動時に永続フラグをチェック
        if (PlayerPrefs.GetInt(KEY_CustomTutorialCompleted, 0) == 1)
        {
            // すでに完了してるのでUIを出さない
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
            gameObject.SetActive(false);

            // （任意）GameManagerの一時フラグも合わせておく
            if (GameManager.Instance != null) GameManager.Instance.isClearCustomTutorial = true;
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TutorialCustomPopup1(this);
            Scene currentScene = SceneManager.GetActiveScene();
            GameManager.Instance.TutorialShow(currentScene);

            // クリア済みならチュートリアルUIを非表示にしてreturn
            if (GameManager.Instance.isClearCustomTutorial)
            {
                if (tutorialPanel != null) tutorialPanel.SetActive(false);
                gameObject.SetActive(false);
                return;
            }
        }
        else
        {
            Debug.LogError("GameManagerが先に起動していません");
        }

        // 矢印を全部OFF
        if (arrowPrefab != null)
        {
            foreach (var arrow in arrowPrefab)
            {
                if (arrow != null) arrow.SetActive(false);
            }
        }

        // UI初期化
        if (tutorialPanel != null) tutorialPanel.SetActive(true);

        // リスナー登録
        if (nextButton != null) nextButton.onClick.AddListener(OnNextClicked);
        if (bodyCustomButton != null) bodyCustomButton.onClick.AddListener(OnBodyCustomClicked);
        if (setButton != null) setButton.onClick.AddListener(OnSetButtonClicked);

        if (rocketCustomButton != null) rocketCustomButton.onClick.AddListener(OnRocketCustomClicked);
        if (tireCustomButton != null)   tireCustomButton.onClick.AddListener(OnTireCustomClicked);
        if (wingCustomButton != null)   wingCustomButton.onClick.AddListener(OnWingCustomClicked);

        // caretDownImageを動かし続ける
        StartCaretAnimation();
        if (caretDownImage != null) caretDownImage.SetActive(true); // 最初は表示

        // ★最初は進めず、表示だけ
        ShowFirstPanel();
    }

    private void Update()
    {
        // ★Step1中だけ Enter で次へ
        if (currentStep == TutorialStep.Step1 && Input.GetKeyDown(KeyCode.Return))
        {
            GoToStep2();
            return;
        }
    }

    // =========================
    // Step1の表示だけ行う
    // =========================
    private void ShowFirstPanel()
    {
        currentStep = TutorialStep.Step1;
        SetTutorialText("家の倉庫へようこそ！\n\nここでは、あなたの機体を管理できます。\n\nまずは、ボディ倉庫の中身を確認しましょう。");
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
    }

    // Nextボタンのクリック共通処理
    private void OnNextClicked()
    {
        if (currentStep == TutorialStep.Step1)
        {
            GoToStep2();
        }
        else
        {
            NextStep();
        }
    }

    // Step2へ遷移する共有処理
    private void GoToStep2()
    {
        currentStep = TutorialStep.Step2;
        NextStep();
    }

    // =========================
    // Step2以降の進行
    // =========================
    public void NextStep()
    {
        switch (currentStep)
        {
            case TutorialStep.Step2:
                SetTutorialText("ボディ倉庫の中身を確認するには、倉庫のアイコンをクリックしてください。\n\nボディ倉庫の中身が表示されます。");
                if (caretDownImage != null) caretDownImage.SetActive(false);
                if (!checkStep2) ShowArrow(0, Vector3.down, 20f);
                checkStep2 = true;

                if (bodyCustomButton != null) bodyCustomButton.interactable = true;
                break;

            case TutorialStep.Step3:
                SetTutorialText("次に、パーツを選択して装備しよう!\n\nボディを選択して、装備ボタンをクリックしてください。");
                HideArrow(0);
                if (!checkStep3) ShowArrow(4, Vector3.right, 20f);
                checkStep3 = true;

                if (bodyCustomButton != null) bodyCustomButton.interactable = false;
                if (setButton != null) setButton.interactable = true;
                break;

            case TutorialStep.Step4:
                SetTutorialText("装備が完了しました!\n\nでは、他のパーツを装備してみましょう。");
                HideArrow(4);
                if (!checkStep4)
                {
                    ShowArrow(1, Vector2.down, 20f);
                    ShowArrow(2, Vector2.down, 20f);
                    ShowArrow(3, Vector2.down, 20f);
                }
                checkStep4 = true;

                if (customCloseButton != null) customCloseButton.interactable = true;
                if (rocketCustomButton != null) rocketCustomButton.interactable = true;
                if (tireCustomButton != null) tireCustomButton.interactable = true;
                if (wingCustomButton != null) wingCustomButton.interactable = true;

                break;

            case TutorialStep.Step5:
                SetTutorialText("すべてのパーツを装備しました!\n\nでは、大空へ飛び立ちましょう!");
                if (!checkStep5) ShowArrow(6, Vector2.right, 20f);
                checkStep5 = true;

                if (GameManager.Instance != null) GameManager.Instance.isClearCustomTutorial = true;

                PlayerPrefs.SetInt(KEY_CustomTutorialCompleted, 1);
                PlayerPrefs.Save();

                if (setButton != null) setButton.interactable = false;
                if (bodyCustomButton != null) bodyCustomButton.interactable = false;
                if (rocketCustomButton != null) rocketCustomButton.interactable = false;
                if (tireCustomButton != null) tireCustomButton.interactable = false;
                if (wingCustomButton != null) wingCustomButton.interactable = false;

                // Close押下で最終ガイドへ
                if (customCloseButton != null) customCloseButton.onClick.AddListener(OnCloseButtonClicked);
                break;
        }
    }

    private void OnBodyCustomClicked()
    {
        if (currentStep == TutorialStep.Step2)
        {
            currentStep = TutorialStep.Step3;
            NextStep();
        }
    }

    private void OnSetButtonClicked()
    {
        if (currentStep == TutorialStep.Step3)
        {
            currentStep = TutorialStep.Step4;
            NextStep();
        }
    }

    private void CheckAllPartsEquipped()
    {
        if (currentStep == TutorialStep.Step4 &&
            PlayerData.Instance != null &&
            PlayerData.Instance.HasAllRequiredPartsEquipped())
        {
            currentStep = TutorialStep.Step5;
            NextStep();
        }
    }

    private void OnCloseButtonClicked()
    {
        if (playButton != null) playButton.interactable = true;

        // 全矢印OFF
        for (int i = 0; i <= 6 && i < (arrowPrefab?.Length ?? 0); i++)
            HideArrow(i);

        // 最後の矢印をプレイボタンへ
        if (arrowPrefab != null && arrowPrefab.Length > 5 && arrowPrefab[5] != null)
        {
            arrowPrefab[5].SetActive(true);
            arrowPrefab[5].transform
                .DOMoveY(arrowPrefab[5].transform.position.y + 20f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    private void SetButtonAction(UnityEngine.Events.UnityAction action)
    {
        if (setButton == null) return;
        setButton.onClick.RemoveAllListeners();
        setButton.onClick.AddListener(action);
    }

    private void RocketSet()
    {
        if (arrowPrefab != null && arrowPrefab.Length > 1 && arrowPrefab[1] != null)
            arrowPrefab[1].SetActive(false);
    }

    private void TireSet()
    {
        if (arrowPrefab != null && arrowPrefab.Length > 2 && arrowPrefab[2] != null)
            arrowPrefab[2].SetActive(false);
    }

    private void WingSet()
    {
        if (arrowPrefab != null && arrowPrefab.Length > 3 && arrowPrefab[3] != null)
            arrowPrefab[3].SetActive(false);
    }

    private void ShowArrow(int index, Vector3 direction, float moveAmount)
    {
        if (arrowPrefab == null || index >= arrowPrefab.Length || arrowPrefab[index] == null) return;

        arrowPrefab[index].SetActive(true);
        var tweenTarget = direction.normalized * moveAmount;
        arrowPrefab[index].transform.DOKill();
        arrowPrefab[index].transform
            .DOLocalMove(arrowPrefab[index].transform.localPosition + tweenTarget, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void HideArrow(int index)
    {
        if (arrowPrefab == null || index >= arrowPrefab.Length || arrowPrefab[index] == null) return;

        arrowPrefab[index].SetActive(false);
        arrowPrefab[index].transform.DOKill(); // Tween停止（安全）
    }

    private void SetTutorialText(string text)
    {
        if (tutorialText != null) tutorialText.text = text;
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
    }

    private void StartCaretAnimation()
    {
        if (caretDownImage == null) return;

        float moveAmount = 20f;
        Vector3 target = Vector3.down * moveAmount;
        caretDownImage.transform.DOLocalMove(caretDownImage.transform.localPosition + target, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void DisableAllButtons()
    {
        if (playButton != null) playButton.interactable = false;
        if (shopButton != null) shopButton.interactable = false;
        if (bodyButton != null) bodyButton.interactable = false;
        if (rocketButton != null) rocketButton.interactable = false;
        if (tireButton != null) tireButton.interactable = false;
        if (wingButton != null) wingButton.interactable = false;
        if (closeButton != null) closeButton.interactable = false;

        if (bodyCustomButton != null) bodyCustomButton.interactable = false;
        if (rocketCustomButton != null) rocketCustomButton.interactable = false;
        if (tireCustomButton != null) tireCustomButton.interactable = false;
        if (wingCustomButton != null) wingCustomButton.interactable = false;
        if (customCloseButton != null) customCloseButton.interactable = false;
        if (setButton != null) setButton.interactable = false;
    }
}
