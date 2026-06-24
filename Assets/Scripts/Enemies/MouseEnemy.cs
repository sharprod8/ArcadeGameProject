using UnityEngine;

public class MouseEnemy : EnemyBase
{
    private int direction = 1;

    protected override void Start()
    {
        base.Start();
        baseSpeed = 3f;
        currentSpeed = baseSpeed;
    }

    private void FixedUpdate()
    {
        if (isKnockedOver) return;

        rb.linearVelocity = new Vector2(direction * currentSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.collider.CompareTag("Ground"))
        {
            direction *= -1;
            transform.localScale = new Vector3(direction, 1, 1);
        }
    }
}