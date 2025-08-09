using UnityEngine;

public class CameraController1 : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [Header("進行方向オフセット設定")]
    [SerializeField] private float directionalOffsetMultiplierX = 0.6f;
    [SerializeField] private float directionalOffsetMultiplierY = 0.6f;
    [SerializeField] private float offsetLerpSpeed = 5f;

    [Header("オフセットの上限")]
    [SerializeField] private float maxOffsetX = 100f;
    [SerializeField] private float maxOffsetY = 60f;

    [Header("ズーム設定")]
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float zoomMultiplier = 0.7f;
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float maxZoom = 100f;

    private Camera mainCamera;
    private Rigidbody2D playerRb;

    private float currentOffsetX = 0f;
    private float currentOffsetY = 0f;

    private void OnEnable()
    {
        GameManager.OnInGamePlayerSpawned += OnPlayerSpawned;
    }

    private void OnDisable()
    {
        GameManager.OnInGamePlayerSpawned -= OnPlayerSpawned;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnPlayerSpawned(CustomPlayer spawnedPlayer)
    {
        player = spawnedPlayer.gameObject;
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (player == null || mainCamera == null || playerRb == null) return;

        Vector2 velocity = playerRb.linearVelocity;
        float speed = velocity.magnitude;

        // ▶️ オフセット目標値を計算（速度 × 倍率、上限付き）
        float targetOffsetX = Mathf.Clamp(velocity.x * directionalOffsetMultiplierX, -maxOffsetX, maxOffsetX);
        float targetOffsetY = Mathf.Clamp(velocity.y * directionalOffsetMultiplierY, -maxOffsetY, maxOffsetY);

        // ▶️ オフセットを滑らかに補間
        currentOffsetX = Mathf.Lerp(currentOffsetX, targetOffsetX, offsetLerpSpeed * Time.fixedDeltaTime);
        currentOffsetY = Mathf.Lerp(currentOffsetY, targetOffsetY, offsetLerpSpeed * Time.fixedDeltaTime);

        // ▶️ カメラの位置を即時更新（Lerpなし）
        transform.position = new Vector3(
            player.transform.position.x + currentOffsetX,
            player.transform.position.y + currentOffsetY,
            transform.position.z
        );

        // ▶️ ズーム（速度に応じて、上限付きで滑らかに）
        float targetZoom = Mathf.Clamp(minZoom + speed * zoomMultiplier, minZoom, maxZoom);
        mainCamera.orthographicSize = Mathf.Lerp(
            mainCamera.orthographicSize,
            targetZoom,
            zoomSpeed * Time.fixedDeltaTime
        );
    }
}
