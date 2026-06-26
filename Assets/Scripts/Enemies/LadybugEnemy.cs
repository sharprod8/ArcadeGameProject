using UnityEngine;

public class LadybugEnemy : EnemyBase
{
    private float hoverTimer;
    private bool hovering;

    protected override void Start()
    {
        base.Start();
        baseSpeed = 1.5f;
        currentSpeed = baseSpeed;
        usesWallDetection = true;
    }

    private void FixedUpdate()
    {
        if (isKnockedOver) return;

        hoverTimer += Time.deltaTime;

        if (hoverTimer > 3f)
        {
            hovering = true;
            usesWallDetection = false;
            rb.linearVelocity = new Vector2(0, 2f);

            if (hoverTimer > 4f)
            {
                hovering = false;
                usesWallDetection = true;
                hoverTimer = 0;
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(direction * currentSpeed, rb.linearVelocity.y);
        }
    }

    public override void KnockOver()
    {
        if (hovering) return;
        base.KnockOver();
    }
}
