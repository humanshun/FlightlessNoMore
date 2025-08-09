using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("基本ステータス")]
    [SerializeField] protected float maxHP = 100f; // 最大HP
    protected float currentHP; // 現在のHP

    [Header("移動速度")]
    [SerializeField] protected float moveSpeed = 10f;

    [Header("攻撃力")]
    [SerializeField] protected float attackPower = 5f;

    [Header("衝突時のダメージ")]
    [SerializeField] protected float selfDamageOnCollision = 5f;
    [Header("鳴き声のSE名")]
    protected abstract string HitSoundName { get; }

    protected virtual void Start()
    {
        currentHP = maxHP;
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }

    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    public virtual void Attack() { }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController2 player = collision.GetComponentInParent<PlayerController2>();
            Rigidbody2D playerRigidbody = collision.GetComponentInParent<Rigidbody2D>();
            if (player != null)
            {
                player.TakeDamage(selfDamageOnCollision);

                float weight = player.TotalWeight;
                float decelerationRate = CalculateDecelerationRate(weight);
                playerRigidbody.linearVelocity *= decelerationRate;

            }
            PlayHitSound();
        }
        TakeDamage(selfDamageOnCollision);
    }

    protected virtual void PlayHitSound()
    {
        if (!string.IsNullOrEmpty(HitSoundName))
        {
            // ここはあなたのプロジェクトのSE再生システムに合わせる
            Debug.Log($"{gameObject.name} が鳴き声を再生: {HitSoundName}");

            AudioManager.Instance.PlaySFX(HitSoundName);
        }
    }

    private float CalculateDecelerationRate(float weight)
    {
        float t = Mathf.InverseLerp(0, 300, weight);
        return Mathf.Lerp(0.1f, 0.8f, t);
    }
}
