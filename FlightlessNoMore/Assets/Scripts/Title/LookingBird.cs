using UnityEngine;

public class LookingBird : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private GameObject headRight;
    [SerializeField] private GameObject bodyRight;
    [SerializeField] private GameObject footRight;
    [SerializeField] private GameObject headLeft;
    [SerializeField] private GameObject bodyLeft;
    [SerializeField] private GameObject footLeft;

    public Transform birdTransform;
    private bool isFacingRight = true;

    void OnEnable()
    {
        GameManager.OnInGamePlayerSpawned += OnPlayerSpawned;
    }

    void OnDisable()
    {
        GameManager.OnInGamePlayerSpawned -= OnPlayerSpawned;
    }

    private void OnPlayerSpawned(CustomPlayer spawnedPlayer)
    {
        birdTransform = spawnedPlayer.transform;
        playerSprite.enabled = false;
        SetRight(true);  
        UpdateHeadRotation(); // 初回の回転更新
    }

    void Start()
    {
        playerSprite.enabled = false;
        SetRight(true);
        // birdTransform がまだ null の可能性があるのでここでは回転処理しない
    }

    void Update()
    {
        if (birdTransform == null) return; // まだプレイヤーがいないなら何もしない
        UpdateHeadRotation();
    }

    private void UpdateHeadRotation()
    {
        bool isLeft = birdTransform.position.x < transform.position.x;

        if (isFacingRight == isLeft)
        {
            isFacingRight = !isLeft;
            SetRight(!isLeft);
        }

        GameObject activeHead = isLeft ? headLeft : headRight;

        Vector3 direction = (birdTransform.position - activeHead.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        activeHead.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
    }

    void SetRight(bool isRight)
    {
        headRight.SetActive(isRight);
        bodyRight.SetActive(isRight);
        footRight.SetActive(isRight);

        headLeft.SetActive(!isRight);
        bodyLeft.SetActive(!isRight);
        footLeft.SetActive(!isRight);
    }

    void Set()
    {
        headRight.SetActive(false);
        bodyRight.SetActive(false);
        footRight.SetActive(false);

        headLeft.SetActive(false);
        bodyLeft.SetActive(false);
        footLeft.SetActive(false);
    }

    public void BackAnim()
    {
        Set();
        playerSprite.enabled = true;
        this.enabled = false;
    }
}
