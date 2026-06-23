using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float enemySpeed = 3f;
    [SerializeField] private int startDirection = 1;

    private Vector2 movement;
    private int currentDirection;
    private float halfWidth;

    void Start()
    {
        halfWidth = spriteRenderer.bounds.extents.x;
        currentDirection = startDirection;
    }

    void FixedUpdate()
    {
        movement.x = enemySpeed * currentDirection;
        movement.y = rb.linearVelocity.y;
        
        rb.linearVelocity = movement;

        SetDirection();
    }

    void SetDirection()
    {
        if (Physics2D.Raycast(transform.position, Vector2.right, halfWidth + 0.1f, LayerMask.GetMask("Ground")) && rb.linearVelocity.x > 0)
        {
            currentDirection *= -1;
        }
        else if (Physics2D.Raycast(transform.position, Vector2.left, halfWidth + 0.1f, LayerMask.GetMask("Ground")) & rb.linearVelocity.x < 0)
        {
            currentDirection *= -1;
        }

        Debug.DrawRay(transform.position, Vector2.right * (halfWidth + 0.1f), Color.red);
        Debug.DrawRay(transform.position, Vector2.left * (halfWidth + 0.1f), Color.red);
    }
}

