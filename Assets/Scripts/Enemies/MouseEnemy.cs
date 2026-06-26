using UnityEngine;

public class MouseEnemy : EnemyBase
{
    protected override void Start()
    {
        base.Start();
        baseSpeed = 3f;
        currentSpeed = baseSpeed;
        usesWallDetection = true;
    }

    private void FixedUpdate()
    {
        if (isKnockedOver) return;

        rb.linearVelocity = new Vector2(direction * currentSpeed, rb.linearVelocity.y);
    }
}