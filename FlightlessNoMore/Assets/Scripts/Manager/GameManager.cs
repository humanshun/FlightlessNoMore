using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CustomPlayer player;
    private CustomPlayer playerInstance;
    public CustomPlayer Player => playerInstance;

    [SerializeField] private CustomPlayer inGamePlayer;
    private CustomPlayer inGamePlayerInstance;
    public CustomPlayer InGamePlayer => inGamePlayerInstance;

    public InGameUI InGameUI => score;

    // シングルトンインスタンス
    public static GameManager Instance;

    public static event System.Action<CustomPlayer> OnInGamePlayerSpawned;

    private InGameUI score;

    private Canvas canvas;
    public GameOverPopup gameOvarPopup;
    public PausePopup pausePopup;
    public TutorialCustom1 tutorialCustomPopup1;
    public TutorialInGame tutorialInGamePopup;
    public bool isGameOver = false;

    //ゲーム進行フラグ
    public bool isBuyPart = false;
    public bool isChangePart = false;
    public bool isClearTutorial = false;
    public bool isClearInGameTutorial = false;
    public bool isClearCustomTutorial = false;
    public bool isTutorial = false;


    private readonly List<BasePopup> openedPopups = new();
    private bool isPaused = false;
    public bool IsPaused => isPaused;


    public void RegisterScore(InGameUI s)
    {
        score = s;
    }

    public void GameOverPopup(GameOverPopup popup)
    {
        gameOvarPopup = popup;
    }
    public void TutorialCustomPopup1(TutorialCustom1 popup)
    {
        tutorialCustomPopup1 = popup;
    }

    public void TutorialInGamePopup(TutorialInGame popup)
    {
        tutorialInGamePopup = popup;
    }
    public void Canvas(Canvas inCanvas)
    {
        canvas = inCanvas;
    }
    void Awake()
    {
        // シングルトンパターン：すでに存在していれば自分を破棄、いなければ自分をInstanceとして残す
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄されないようにする
            PlayerData.OnPartPurchased += HandlePartPurchased;
            PlayerData.OnEquippedNonInitialPart += HandleChangePart;
        }
        else
        {
            Destroy(gameObject); // すでに存在しているなら自分を削除
        }
    }

    void Update()
    {
        _ = UpdateAsync();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if ((sceneName == "InGame" || sceneName == "Custom") && !isGameOver && pausePopup != null)
            {
                if (openedPopups.Count > 0)
                {
                    CloseAllPopups(); // すべてのポップアップを閉じる
                }
                else if (pausePopup != null)
                {
                    pausePopup.Show(); // ポーズポップアップを開く
                }
            }
        }
    }
    private async Task UpdateAsync()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "InGame")
            {
                sceneName = "Custom";
            }
            PlayerData.Instance.ResetPlayerData(); // F1キーでデータをリセット
            await SceneChanger.Instance.ChangeScene(sceneName, 1.0f, 1.0f);
        }
    }
    void OnDestroy()
    {
        // シーンが破棄されるときにイベントを解除
        PlayerData.OnPartPurchased -= HandlePartPurchased;
        PlayerData.OnEquippedNonInitialPart -= HandleChangePart;
    }
    public void TutorialShow(Scene scene)
    {
        isGameOver = false; // シーンが読み込まれたらゲームオーバー状態をリセット

        if (scene.name == "Custom")
        {
            Vector3 spawnPosition = new Vector3(-3.7f, -1.15f, 0f);

            if (playerInstance != null)
            {
                Destroy(playerInstance.gameObject); // 既存のインスタンスを削除
            }
            playerInstance = Instantiate(player, spawnPosition, Quaternion.identity);

            if (isClearCustomTutorial)
            {
                tutorialCustomPopup1.gameObject.SetActive(false);
                return;
            }
            else
            {
                tutorialCustomPopup1.gameObject.SetActive(true); // チュートリアルポップアップを表示
                tutorialCustomPopup1.DisableAllButtons();
            }
        }
        else if (scene.name == "InGame")
        {
            Vector3 spawnPosition = new Vector3(-3.7f, -32f, 0f);

            if (inGamePlayerInstance != null)
            {
                Destroy(inGamePlayerInstance.gameObject); // 既存のインスタンスを削除
            }
            inGamePlayerInstance = Instantiate(inGamePlayer, spawnPosition, Quaternion.identity);
            OnInGamePlayerSpawned?.Invoke(inGamePlayerInstance); // イベントを発火して、InGamePlayerが生成されたことを通知
            if (tutorialInGamePopup != null)
            {
                tutorialInGamePopup.gameObject.SetActive(false); // チュートリアルポップアップを非表示にする
            }

            if (isClearInGameTutorial)
            {
                tutorialInGamePopup.gameObject.SetActive(false);
            }
            else
            {
                tutorialInGamePopup.gameObject.SetActive(true);
            }
        }
    }
    public void GameOver()
    {
        if (isGameOver) return; // すでにゲームオーバーなら何もしない
        isGameOver = true; // ゲームオーバー状態にする

        //もしscoreがnullで無ければ、Coinsを計算して追加
        int earnedCoins = score != null ? score.CalculateCoins() : 0;
        PlayerData.Instance.AddCoins(earnedCoins);

        bool distanceUpdated = false;
        bool altitudeUpdated = false;

        //ハイスコア記録
        if (score != null)
        {
            (distanceUpdated, altitudeUpdated) = PlayerData.Instance.TryUpdateHighScore(score.distance, score.maxAltitude);
        }

        // ゲームオーバーのポップアップを表示
        if (gameOvarPopup != null)
        {
            gameOvarPopup.Show(distanceUpdated, altitudeUpdated);
        }

        //scoreにゲーム終了したことを通知
        if (score != null)
        {
            score.OnGameOver();
        }
        AudioManager.Instance.StopAllLoopSFX();
    }

    public async void GameClear()
    {
        if (isGameOver) return;
        isGameOver = true;
        await SceneChanger.Instance.ChangeScene("Clear", 1.0f, 1.0f);
        Debug.Log("ゲームクリア");
    }
    private void HandlePartPurchased()
    {
        isBuyPart = true;
    }

    private void HandleChangePart()
    {
        isChangePart = true;
    }

    public void RegisterPopup(BasePopup popup)
    {
        if (!openedPopups.Contains(popup))
        {
            openedPopups.Add(popup);
            UpdateTimeScale();
            PauseAllAudio();
        }
    }

    public void UnregisterPopup(BasePopup popup)
    {
        if (openedPopups.Contains(popup))
        {
            openedPopups.Remove(popup);
            UpdateTimeScale();
            if (openedPopups.Count == 0)
            {
                ResumeAllAudio();
            }
        }
    }

    private void UpdateTimeScale()
    {
        if (openedPopups.Count > 0)
        {
            Time.timeScale = 0f;
            isPaused = true;
        }
        else
        {
            Time.timeScale = 1f;
            isPaused = false;
        }
    }
    public void CloseAllPopups()
    {
        foreach (var popup in openedPopups.ToArray()) // コピーしてループ（変更を避ける）
        {
            popup.Close();
        }
    }
    public void PauseAllAudio()
    {
        AudioManager.Instance?.PauseAllAudio();
    }

    public void ResumeAllAudio()
    {
        AudioManager.Instance?.ResumeAllAudio();
    }

    public void ResetGameState()
    {
        isGameOver = false;
        isBuyPart = false;
        isChangePart = false;
        isClearTutorial = false;
        isClearInGameTutorial = false;
        isClearCustomTutorial = false;

        isTutorial = false;
    }
}
