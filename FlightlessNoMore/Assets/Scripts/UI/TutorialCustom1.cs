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
    // private bool checkStep1 = false;
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
    private void OnTireCustomClicked() => SetButtonAction(TireSet);
    private void OnWingCustomClicked() => SetButtonAction(WingSet);

    enum TutorialStep
    {
        Step1, Step2, Step3, Step4, Step5
    }
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TutorialCustomPopup1(this);
            Scene currentScene = SceneManager.GetActiveScene();
            GameManager.Instance.TutorialShow(currentScene);
            // クリア済みならチュートリアルUIを非表示にしてreturn
            if (GameManager.Instance.isClearCustomTutorial)
            {
                tutorialPanel.SetActive(false);
                this.gameObject.SetActive(false);
                return;
            }
        }
        else
        {
            Debug.LogError("GameManagerが先に起動していません");
        }

        foreach (var arrow in arrowPrefab)
        {
            arrow.SetActive(false);
        }

        tutorialPanel.SetActive(true);
        nextButton.onClick.AddListener(NextStep);
        bodyCustomButton.onClick.AddListener(OnBodyCustomClicked);
        setButton.onClick.AddListener(OnSetButtonClicked);

        rocketCustomButton.onClick.AddListener(OnRocketCustomClicked);
        tireCustomButton.onClick.AddListener(OnTireCustomClicked);
        wingCustomButton.onClick.AddListener(OnWingCustomClicked);

        // caretDownImageを最初から動かす続ける
        StartCaretAnimation();

        caretDownImage.SetActive(true); //最初は表示

        NextStep();
    }
    private void OnEnable()
    {
        PlayerData.OnAnyPartEquipped += CheckAllPartsEquipped;
    }

    public void DisableAllButtons()
    {
        playButton.interactable = false;
        shopButton.interactable = false;
        bodyButton.interactable = false;
        rocketButton.interactable = false;
        tireButton.interactable = false;
        wingButton.interactable = false;
        closeButton.interactable = false;
        bodyCustomButton.interactable = false;
        rocketCustomButton.interactable = false;
        tireCustomButton.interactable = false;
        wingCustomButton.interactable = false;
        customCloseButton.interactable = false;
        setButton.interactable = false;
    }
    private void OnDisable()
    {
        PlayerData.OnAnyPartEquipped -= CheckAllPartsEquipped;
        nextButton.onClick.RemoveListener(NextStep);

    }

    public void NextStep()
    {
        switch (currentStep)
        {
            case TutorialStep.Step1:
                SetTutorialText("家の倉庫へようこそ！\n\nここでは、あなたの機体を管理できます。\n\nまずは、ボディ倉庫の中身を確認しましょう。");
                tutorialPanel.SetActive(true);
                currentStep++;
                break;
            case TutorialStep.Step2:
                SetTutorialText("ボディ倉庫の中身を確認するには、倉庫のアイコンをクリックしてください。\n\nボディ倉庫の中身が表示されます。");
                caretDownImage.SetActive(false);
                if (!checkStep2)
                {
                    ShowArrow(0, Vector3.down, 20f);
                }
                checkStep2 = true;

                bodyCustomButton.interactable = true;
                break;
            case TutorialStep.Step3:
                SetTutorialText("次に、パーツを選択して装備しよう!\n\nボディを選択して、装備ボタンをクリックしてください。");
                HideArrow(0);
                if (!checkStep3)
                {
                    ShowArrow(4, Vector3.right, 20f);
                }
                checkStep3 = true;

                bodyCustomButton.interactable = false;
                setButton.interactable = true;
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

                customCloseButton.interactable = true;
                rocketCustomButton.interactable = true;
                tireCustomButton.interactable = true;
                wingCustomButton.interactable = true;

                break;
            case TutorialStep.Step5:
                SetTutorialText("すべてのパーツを装備しました!\n\nでは、大空へ飛び立ちましょう!");
                if (!checkStep5)
                {
                    ShowArrow(6, Vector2.right, 20f);
                }
                checkStep5 = true;
                GameManager.Instance.isClearCustomTutorial = true;
                setButton.interactable = false;
                bodyCustomButton.interactable = false;
                rocketCustomButton.interactable = false;
                tireCustomButton.interactable = false;
                wingCustomButton.interactable = false;
                customCloseButton.onClick.AddListener(OnCloseButtonClicked);
                break;
        }
    }

    void OnBodyCustomClicked()
    {
        if (currentStep == TutorialStep.Step2)
        {
            currentStep++;
            NextStep();
        }
    }

    void OnSetButtonClicked()
    {
        if (currentStep == TutorialStep.Step3)
        {
            currentStep++;
            NextStep();
        }
    }
    private void CheckAllPartsEquipped()
    {
        if (currentStep == TutorialStep.Step4 &&
            PlayerData.Instance.HasAllRequiredPartsEquipped())
        {
            currentStep++;
            NextStep();
        }
    }

    private void OnCloseButtonClicked()
    {
        playButton.interactable = true;

        for (int i = 0; i <= 6; i++)
        {
            HideArrow(i);
        }
        arrowPrefab[5].SetActive(true);
        arrowPrefab[5].transform
            .DOMoveY(arrowPrefab[5].transform.position.y + 20f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void SetButtonAction(UnityEngine.Events.UnityAction action)
    {
        setButton.onClick.RemoveAllListeners();
        setButton.onClick.AddListener(action);
    }
    private void RocketSet()
    {
        arrowPrefab[1].SetActive(false);
    }
    private void TireSet()
    {
        arrowPrefab[2].SetActive(false);
    }
    private void WingSet()
    {
        arrowPrefab[3].SetActive(false);
    }
    private void ShowArrow(int index, Vector3 direction, float moveAmount)
    {
        if (index >= arrowPrefab.Length) return;
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
        if (index >= arrowPrefab.Length) return;
        arrowPrefab[index].SetActive(false);
        arrowPrefab[index].transform.DOKill(); // Tween停止（安全）
    }

    private void SetTutorialText(string text)
    {
        tutorialText.text = text;
        tutorialPanel.SetActive(true);
    }

    private void StartCaretAnimation()
    {
        float moveAmount = 20f;
        Vector3 target = Vector3.down * moveAmount;
        caretDownImage.transform.DOLocalMove(caretDownImage.transform.localPosition + target, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
