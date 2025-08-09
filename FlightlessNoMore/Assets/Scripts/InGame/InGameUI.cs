using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private CustomPlayer player;
    [SerializeField] private PlayerController2 playerController;
    [SerializeField] private Transform playerPosition;
    public float distance;
    public float altitude;
    public float maxAltitude = 0f;

    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI altitudeText;

    // CoinUIManagerにまとめるので、CoinDisplayやAddCoinEffectの直接参照は不要
    [SerializeField] private CoinUIManager coinUIManager;

    [SerializeField] private float startX = 250f;
    [SerializeField] private Slider slider;
    private float goal = 7000f;

    private bool hasStarted = false;
    private float startPosX;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider rocketSlider;

    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI rocketText;

    [SerializeField] private GameObject playerUI;

    private int collectionCoins = 0;
    private float initialHealth = 1f;
    private float initialRocketTime = 1f;

    void OnEnable()
    {
        GameManager.OnInGamePlayerSpawned += OnPlayerSpawned;
    }

    void OnDisable()
    {
        GameManager.OnInGamePlayerSpawned -= OnPlayerSpawned;

        if (playerController != null)
        {
            playerController.OnHealthChanged -= UpdateHealthUI;
            playerController.OnRocketTimeChanged -= UpdateRocketUI;
        }
    }

    public void Setup(float health, float rocket)
    {
        initialHealth = health;
        initialRocketTime = rocket;

        UpdateHealthUI(health);
        UpdateRocketUI(rocket);
    }

    void Start()
    {
        coinUIManager.gameObject.SetActive(false);
        GameManager.Instance.RegisterScore(this);
    }

    private void OnPlayerSpawned(CustomPlayer spawnedPlayer)
    {
        playerPosition = spawnedPlayer.transform;
        hasStarted = false;
        distance = 0f;
        altitude = 0f;
        distanceText.text = "距離: 0.0 m";
        altitudeText.text = "高度: 0.0 m";

        playerController = spawnedPlayer.GetComponent<PlayerController2>();
        if (playerController != null)
        {
            Setup(playerController.TotalHealth, playerController.TotalRocketTime);

            playerController.OnHealthChanged += UpdateHealthUI;
            playerController.OnRocketTimeChanged += UpdateRocketUI;
        }
    }

    void Update()
    {
        if (playerPosition == null) return;

        altitude = Mathf.Max(0f, (playerPosition.position.y + 36f) / 5f);
        altitudeText.text = $"高度: {altitude:F1} m";

        if (altitude > maxAltitude)
        {
            maxAltitude = altitude;
        }

        if (!hasStarted)
        {
            if (playerPosition.position.x >= startX)
            {
                startPosX = playerPosition.position.x;
                hasStarted = true;
            }
        }
        else
        {
            distance = Mathf.Max(0f, (playerPosition.position.x - startPosX) / 5f);
            distanceText.text = $"距離: {distance:F1} m";
        }

        if (slider != null)
        {
            slider.value = Mathf.Clamp01(playerPosition.position.x / goal);
        }
    }

    public void AddCollectedCoins(int coins)
    {
        collectionCoins += coins;
    }

    public int CalculateCoins()
    {
        int baseCoins = Mathf.FloorToInt(distance);
        return baseCoins + collectionCoins;
    }

    public void OnGameOver()
    {
        // 距離＆高度のUIを非表示
        distanceText.gameObject.SetActive(false);
        altitudeText.gameObject.SetActive(false);

        // プレイヤーUIも非表示
        slider.gameObject.SetActive(false);
        playerUI.SetActive(false);

        // コインUIを表示（ここでアクティブ化）
        coinUIManager.gameObject.SetActive(true);

        // 獲得コインを計算
        int totalCoins = CalculateCoins();

        // ✅ ここでCoinUIManagerに演出開始を任せる
        coinUIManager.PlayCoinUI(totalCoins);
    }
    
    private void UpdateHealthUI(float health)
    {
        float percent = (health / initialHealth) * 100f;
        healthSlider.value = percent;
        healthText.text = $"{percent:F0}%";
    }

    private void UpdateRocketUI(float rocket)
    {
        float percent = (rocket / initialRocketTime) * 100f;
        rocketSlider.value = percent;
        rocketText.text = $"{percent:F0}%";
    }
}
