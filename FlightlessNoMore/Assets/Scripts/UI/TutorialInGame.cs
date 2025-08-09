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

    // 初期化済みかどうか
    private bool initialized = false;

    // 各ステップが開始されるX座標
    [SerializeField] private float[] stepTriggers;

    // ステップごとの表示状態
    private bool[] stepActive;

    // チュートリアルが完了しているかどうか
    private bool tutorial = false;

    // 現在のステップ番号
    private int currentStep = 0;

    // プレイヤーがスポーンされた時に呼ばれるイベントの登録
    void OnEnable()
    {
        GameManager.OnInGamePlayerSpawned += OnPlayerSpawned;
    }

    // イベント登録解除
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
        // チュートリアル完了フラグを取得（PlayerPrefsから）
        int completed = PlayerPrefs.GetInt("InGameTutorialCompleted", 0);
        Debug.Log($"チュートリアル完了フラグ: {completed}");

        // 既にチュートリアルが完了していたら処理スキップ
        if (completed == 1)
        {
            Debug.Log("チュートリアルはすでに完了しています。");
            tutorial = true;
            this.enabled = false;
            Time.timeScale = 1f; // ゲーム進行を通常速度に戻す
            GameManager.Instance.isTutorial = false;
            caretDownImage.SetActive(false);
        }

        // GameManagerにチュートリアルポップアップを通知
        GameManager.Instance.TutorialInGamePopup(tutorialPopup);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TutorialInGamePopup(this);
            Scene currentScene = SceneManager.GetActiveScene();
            GameManager.Instance.TutorialShow(currentScene);
        }

        // ステップのアクティブ状態を初期化
        stepActive = new bool[tutorialPanels.Length];

        // 各パネルにクリック時のリスナー登録、初期非表示設定
        for (int i = 0; i < tutorialPanels.Length; i++)
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

        // 最初のステップだけ表示・停止
        if (tutorialPanels.Length > 0 && completed == 0)
        {
            tutorialPanels[0].gameObject.SetActive(true);
            stepActive[0] = true;
            Time.timeScale = 0f; // ゲームを一時停止
            GameManager.Instance.isTutorial = true;
            GameManager.Instance.PauseAllAudio();
        }

        // 矢印のアニメーション開始
        if (completed != 1)
        {
            StartCaretAnimation();
            caretDownImage.SetActive(true);
        }
    }

    void Update()
    {
        if (tutorial || !initialized) return;

        // プレイヤーが次のステップに到達したらチュートリアルを進める
        if (currentStep < stepTriggers.Length &&
            playerTransform.position.x >= stepTriggers[currentStep] &&
            !stepActive[currentStep])
        {
            NextStep();
            Time.timeScale = 0f;
            GameManager.Instance.isTutorial = true;
        }

        // Enterキー（Return）でもステップを非表示にできる
        if (Input.GetKeyDown(KeyCode.Return))
        {
            DeactivateCurrentStep();
        }
    }

    // 次のステップに進む処理
    private void NextStep()
    {
        // 全ステップ終了していたら終了処理
        if (currentStep >= tutorialPanels.Length)
        {
            Debug.Log("チュートリアルはすでに終了しています。");
            tutorial = true;
            return;
        }

        // 現在のパネルを非表示
        if (currentStep < tutorialPanels.Length)
        {
            tutorialPanels[currentStep].gameObject.SetActive(false);
            caretDownImage.SetActive(false);
            stepActive[currentStep] = false;
        }

        currentStep++;

        // 特定のステップ（例：3）でチェックリストの確認
        if (currentStep == 3)
        {
            tutorialInGameCheckList.CheckList();
        }

        // 次のステップのパネルを表示
        if (currentStep < tutorialPanels.Length)
        {
            Debug.Log($"チュートリアルステップ {currentStep} を表示します。");
            tutorialPanels[currentStep].gameObject.SetActive(true);
            caretDownImage.SetActive(true);
            stepActive[currentStep] = true;
            GameManager.Instance.PauseAllAudio();
        }

        // 最後のステップに到達したらチュートリアル完了処理
        if (currentStep == tutorialPanels.Length - 1)
        {
            GameManager.Instance.isClearInGameTutorial = true;
            tutorial = true;
            Time.timeScale = 1f; // ゲームを再開
            GameManager.Instance.isTutorial = false;

            // チュートリアル完了フラグを保存
            PlayerPrefs.SetInt("InGameTutorialCompleted", 1);
            PlayerPrefs.Save();
            Debug.Log("✅ チュートリアル完了として保存しました");
        }
    }

    // 現在のステップを非表示にする処理（Enterキーやクリックで呼ばれる）
    private void DeactivateCurrentStep()
    {
        if (currentStep < tutorialPanels.Length && stepActive[currentStep])
        {
            tutorialPanels[currentStep].gameObject.SetActive(false);
            caretDownImage.SetActive(false);
            stepActive[currentStep] = false;

            // ゲームを再開
            Time.timeScale = 1f;
            GameManager.Instance.isTutorial = false;
            GameManager.Instance.ResumeAllAudio();
        }
    }

    // 矢印画像を上下に動かすアニメーション
    private void StartCaretAnimation()
    {
        float moveAmount = 20f;
        Vector3 target = Vector3.down * moveAmount;

        caretDownImage.transform.DOLocalMove(caretDownImage.transform.localPosition + target, 0.5f)
            .SetLoops(-1, LoopType.Yoyo) // 無限に往復
            .SetEase(Ease.InOutSine)     // スムーズなイージング
            .SetUpdate(true);            // ゲームが止まっていてもアニメーション更新
    }
}
