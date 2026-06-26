using UnityEngine;

public class EnemyBaseV2 : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] int startDirection = 1;

    [Header("Enemy Stats")]
    [SerializeField] float speed = 3f;

    private Vector2 movement;
    int currentDirection;
    float halfWidth;

    private void Start()
    {
        halfWidth = sprite.bounds.extents.x;
        currentDirection = startDirection;
        if (startDirection == 1)
        {
            sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        movement.x = speed * currentDirection;
        movement.y = rb.linearVelocity.y;
        rb.linearVelocity = movement;

        SetDirection();
    }

    private void SetDirection()
    {
        if (Physics2D.Raycast(transform.position, Vector2.right, halfWidth + 0.1f, LayerMask.GetMask("Ground")) && rb.linearVelocity.x > 0)
        {
            currentDirection *= -1;
            sprite.flipX = false;
        }
        else if (Physics2D.Raycast(transform.position, Vector2.left, halfWidth + 0.1f, LayerMask.GetMask("Ground")) && rb.linearVelocity.x < 0)
        {
            currentDirection *= -1;
            sprite.flipX = true;
        }
        Debug.DrawRay(transform.position, Vector2.right * (halfWidth + 0.1f), Color.red);
        Debug.DrawRay(transform.position, Vector2.left * (halfWidth + 0.1f), Color.red);
    } 
}
