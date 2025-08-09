using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TutorialInGame : MonoBehaviour
{
    // チュートリアルポップアップ自体（別のTutorialInGame型）
    [SerializeField] private TutorialInGame tutorialPopup;

    // 各ステップのチュートリアルボタン（説明パネル）
    [SerializeField] private Button[] tutorialPanels;

    // 下向き矢印の画像（アニメーションする）
    [SerializeField] private GameObject caretDownImage;

    // チェックリスト制御用のスクリプト
    [SerializeField] private TutorialInGameCheckList tutorialInGameCheckList;

    // プレイヤーのTransform（位置検出用）
    [SerializeField] private Transform playerTransform;

    // 初期化済みかどうか（プレイヤーspawn待ち）
    private bool initialized = false;

    // 各ステップが開始されるX座標
    [SerializeField] private float[] stepTriggers;

    // ステップごとの表示状態
    private bool[] stepActive;

    // チュートリアルが完了しているかどうか
    private bool tutorial = false;

    // 現在のステップ番号（表示中のインデックス）
    private int currentStep = 0;

    // イベント登録
    void OnEnable()
    {
        GameManager.OnInGamePlayerSpawned += OnPlayerSpawned;
    }

    // イベント解除
    void OnDisable()
    {
        GameManager.OnInGamePlayerSpawned -= OnPlayerSpawned;
    }

    // プレイヤーが生成されたときにTransformを取得
    private void OnPlayerSpawned(CustomPlayer spawnedPlayer)
    {
        playerTransform = spawnedPlayer.transform;
        initialized = true;
    }

    void Start()
    {
        int completed = PlayerPrefs.GetInt("InGameTutorialCompleted", 0);
        Debug.Log($"チュートリアル完了フラグ: {completed}");

        // 既に完了ならすべて無効化して早期return
        if (completed == 1)
        {
            Debug.Log("チュートリアルはすでに完了しています。");
            tutorial = true;
            Time.timeScale = 1f;
            if (caretDownImage != null) caretDownImage.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.isTutorial = false;
            this.enabled = false;
            return;
        }

        // GameManagerへ通知（nullセーフ）
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TutorialInGamePopup(tutorialPopup);
            GameManager.Instance.TutorialInGamePopup(this);
            Scene currentScene = SceneManager.GetActiveScene();
            GameManager.Instance.TutorialShow(currentScene);
        }

        // ステップ状態を初期化
        int panelCount = tutorialPanels != null ? tutorialPanels.Length : 0;
        stepActive = new bool[panelCount];
        for (int i = 0; i < panelCount; i++)
        {
            if (tutorialPanels[i] == null)
            {
                Debug.LogError($"チュートリアルパネル{i}が設定されていません。");
                continue;
            }
            tutorialPanels[i].onClick.AddListener(DeactivateCurrentStep);
            tutorialPanels[i].gameObject.SetActive(false);
            stepActive[i] = false;
        }

        // 最初のステップ表示＆ポーズ
        if (panelCount > 0)
        {
            currentStep = 0;
            tutorialPanels[0]?.gameObject.SetActive(true);
            stepActive[0] = true;
            Time.timeScale = 0f;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.isTutorial = true;
                GameManager.Instance.PauseAllAudio();
            }
        }

        // 矢印アニメーション開始
        if (caretDownImage != null)
        {
            StartCaretAnimation();
            caretDownImage.SetActive(true);
        }
    }

    void Update()
    {
        if (tutorial || !initialized) return;

        // 位置トリガーで次のステップへ（現在非アクティブなら）
        if (currentStep < stepTriggers.Length &&
            playerTransform != null &&
            playerTransform.position.x >= stepTriggers[currentStep] &&
            (currentStep >= stepActive.Length || !stepActive[currentStep]))
        {
            NextStep();
            Time.timeScale = 0f;
            if (GameManager.Instance != null) GameManager.Instance.isTutorial = true;
        }

        // Enterキーで現在のステップを閉じる
        if (Input.GetKeyDown(KeyCode.Return))
        {
            DeactivateCurrentStep();
        }
    }

    // 次のステップに進む処理（自動遷移時）
    private void NextStep()
    {
        int panelCount = tutorialPanels != null ? tutorialPanels.Length : 0;

        // すでに全ステップを越えていたら何もしない
        if (currentStep >= panelCount)
        {
            Debug.Log("チュートリアルはすでに終了しています。");
            tutorial = true;
            return;
        }

        // 現在のパネルを非表示
        if (currentStep < panelCount)
        {
            tutorialPanels[currentStep]?.gameObject.SetActive(false);
            if (caretDownImage != null) caretDownImage.SetActive(false);
            if (currentStep < stepActive.Length) stepActive[currentStep] = false;
        }

        // 次へ
        currentStep++;

        // 特定ステップでチェックリスト
        if (currentStep == 3 && tutorialInGameCheckList != null)
        {
            tutorialInGameCheckList.CheckList();
        }

        // 次のパネルを表示（※最後のステップもここで表示して、閉じられた時に完了扱い）
        if (currentStep < panelCount)
        {
            Debug.Log($"チュートリアルステップ {currentStep} を表示します。");
            tutorialPanels[currentStep]?.gameObject.SetActive(true);
            if (caretDownImage != null) caretDownImage.SetActive(true);
            if (currentStep < stepActive.Length) stepActive[currentStep] = true;
            if (GameManager.Instance != null) GameManager.Instance.PauseAllAudio();
        }
    }

    // 現在のステップを非表示にする（Enter/クリック）
    private void DeactivateCurrentStep()
    {
        int panelCount = tutorialPanels != null ? tutorialPanels.Length : 0;

        if (currentStep < panelCount &&
            currentStep < stepActive.Length &&
            stepActive[currentStep])
        {
            tutorialPanels[currentStep]?.gameObject.SetActive(false);
            if (caretDownImage != null) caretDownImage.SetActive(false);
            stepActive[currentStep] = false;

            // 最後のパネルならここで完了処理
            if (currentStep == panelCount - 1)
            {
                FinishTutorial();
                return;
            }

            // まだ続きがあるなら一旦再開（次のトリガーでまた止める）
            Time.timeScale = 1f;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.isTutorial = false;
                GameManager.Instance.ResumeAllAudio();
            }
        }
    }

    // 共通の完了処理（最後のパネルを閉じた瞬間に呼ぶ）
    private void FinishTutorial()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isClearInGameTutorial = true;
            GameManager.Instance.isTutorial = false;
            GameManager.Instance.ResumeAllAudio();
        }

        tutorial = true;
        Time.timeScale = 1f;

        if (caretDownImage != null) caretDownImage.SetActive(false);

        PlayerPrefs.SetInt("InGameTutorialCompleted", 1);
        PlayerPrefs.Save();
        Debug.Log("✅ チュートリアル完了として保存しました");
    }

    // 矢印画像を上下に動かすアニメーション
    private void StartCaretAnimation()
    {
        if (caretDownImage == null) return;

        float moveAmount = 20f;
        Vector3 target = Vector3.down * moveAmount;

        caretDownImage.transform.DOLocalMove(
                caretDownImage.transform.localPosition + target,
                0.5f
            )
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true); // ポーズ中も更新
    }
}
