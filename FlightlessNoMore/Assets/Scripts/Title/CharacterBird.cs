using UnityEngine;

public class CharacterBird : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    public float stopPosition;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void FixedUpdate()
    {

        Vector2 currentPos = rb.position;

        if (currentPos.x < stopPosition)
        {
            float newX = Mathf.MoveTowards(currentPos.x, stopPosition, moveSpeed * Time.fixedDeltaTime);
            spriteRenderer.flipX = newX > currentPos.x;

            currentPos.x = newX;
        }

        // Yが4未満なら、Y=4まで垂直に移動
        if (currentPos.y < 4f)
        {
            currentPos.y = Mathf.MoveTowards(currentPos.y, 4f, moveSpeed * Time.fixedDeltaTime);
        }

        // 実際に移動
        rb.MovePosition(currentPos);
    }
}
