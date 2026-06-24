using UnityEngine;

public class WormEnemy : EnemyBase
{
    private Vector2 direction = Vector2.right;

    protected override void Start()
    {
        base.Start();
        baseSpeed = 1.2f;
        currentSpeed = baseSpeed;
    }

    private void FixedUpdate()
    {
        if (isKnockedOver) return;

        rb.linearVelocity = direction * currentSpeed;

        RaycastHit2D groundCheck = Physics2D.Raycast(transform.position + (Vector3)direction * 0.3f, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));

        if (!groundCheck)
        {
            direction = new Vector2(direction.y, -direction.x);
        }
    }

    public override void TakeHit()
    {
        RaycastHit2D upCheck = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

        if (!upCheck)
            return;

        base.TakeHit();
    }
}
