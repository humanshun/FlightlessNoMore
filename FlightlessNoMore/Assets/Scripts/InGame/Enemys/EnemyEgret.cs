using UnityEngine;
using Cysharp.Threading.Tasks;

public class EnemyEgret : EnemyBase
{
    private Transform player;

    private enum State { Idle, Look, Attack }
    private State currentState = State.Idle;

    [SerializeField] private Animator animator;
    [SerializeField] private float lookRange = 20f;
    [SerializeField] private float chaseSpeedMultiplier = 2f;

    private Vector2 attackDirection;
    private bool isAttackWaiting = false;
    protected override string HitSoundName => "SE_Egret";

    protected override void Start()
    {
        base.Start();
    }

    void OnEnable()
    {
        // GameManagerからプレイヤーを取得
        if (GameManager.Instance != null && GameManager.Instance.InGamePlayer != null)
        {
            player = GameManager.Instance.InGamePlayer.transform;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        currentState = State.Idle;
        // 向き・回転・フラグをリセット
        isAttackWaiting = false;
        attackDirection = Vector2.zero;
        transform.localScale = new Vector3(2, 2, 2);
        transform.rotation = Quaternion.identity;
    }

    protected override void Update()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            switch (currentState)
            {
                case State.Idle:
                    if (distance < lookRange)
                    {
                        currentState = State.Look;
                    }
                    break;

                case State.Look:
                    // プレイヤーの方向を向く（左右反転＋向き）
                    Vector2 dir = (player.position - transform.position).normalized;
                    if (dir.x != 0)
                        transform.localScale = new Vector3(Mathf.Sign(dir.x) * -2, 2, 2); // 左右反転
                    // プレイヤーの方向を向く（回転）
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    // 右向き（scale.x > 0）の場合は+180度回転
                    if (transform.localScale.x > 0)
                        angle += 180f;
                    transform.rotation = Quaternion.Euler(0, 0, angle);

                    if (!isAttackWaiting)
                    {
                        attackDirection = dir;
                        isAttackWaiting = true;
                        AttackDelayAsync().Forget();
                    }
                    else if (distance >= lookRange)
                    {
                        currentState = State.Idle;
                        // 向きをリセット（必要なら）
                        transform.rotation = Quaternion.identity;
                        isAttackWaiting = false;
                    }
                    break;

                case State.Attack:
                    // 何もしない（Moveで直進）
                    break;

            }
            // 状態に応じてアニメーション速度を変化
            if (animator != null)
            {
                if (currentState == State.Look || currentState == State.Attack)
                    animator.speed = 4f;
                else
                    animator.speed = 1f;
            }
        }
        base.Update();
    }

    private async UniTaskVoid AttackDelayAsync()
    {
        await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());

        if (player != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            attackDirection = dir;
        }

        currentState = State.Attack;
        isAttackWaiting = false;
    }

    protected override void Move()
    {
        switch (currentState)
        {
            case State.Idle:
                // 何もしない
                break;
            case State.Look:
                // その場で停止してプレイヤーの方向を見続ける
                break;
            case State.Attack:
                // 最後に記録した方向に突進
                transform.Translate(attackDirection * moveSpeed * chaseSpeedMultiplier * Time.deltaTime, Space.World);
                break;
            default:
                base.Move();
                break;
        }
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController2 player = collision.GetComponentInParent<PlayerController2>();
            if (player != null)
            {
                player.TakeDamage(attackPower);
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
                    rb.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);
                }
            }
        }
        base.OnTriggerEnter2D(collision);
    }
}