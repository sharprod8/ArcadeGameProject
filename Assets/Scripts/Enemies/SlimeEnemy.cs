using UnityEngine;

public class SlimeEnemy : EnemyBase
{
    private int direction = 1;

    protected override void Start()
    {
        base.Start();
        baseSpeed = 1.5f;
        currentSpeed = baseSpeed;
    }

    private void FixedUpdate()
    {
        if (isKnockedOver) return;

        rb.linearVelocity = new Vector2(direction * currentSpeed, rb.linearVelocity.y);
    }

}
