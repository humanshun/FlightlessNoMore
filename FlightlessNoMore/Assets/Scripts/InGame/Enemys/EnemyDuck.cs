using UnityEngine;

public class EnemyDuck : EnemyBase
{
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float speedForce = 50f;
    protected override string HitSoundName => "SE_Duck";
    
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController2 player = collision.GetComponentInParent<PlayerController2>();
            if (player != null)
            {
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x + speedForce, rb.linearVelocity.y + jumpForce);
                }
            }
            AudioManager.Instance.PlaySFX("SE_Duck", 1f, 0.3f);
        }
    }
}
