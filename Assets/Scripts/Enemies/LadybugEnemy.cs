using UnityEngine;

public class LadybugEnemy : EnemyBase
{
    private float hoverTimer;
    private bool isHovering;

    protected override void Start()
    {
        base.Start();
        baseSpeed = 1.5f;
        currentSpeed = baseSpeed;
    }

    private void FixedUpdate()
    {
        if (isKnockedOver) return;

        hoverTimer += Time.deltaTime;

        if (hoverTimer > 3f)
        {
            isHovering = true;
            rb.linearVelocity = new Vector2(0, 2f);

            if (hoverTimer > 4f)
            {
                isHovering = false;
                hoverTimer = 0;
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
        }
    }

    public override void KnockOver()
    {
        if (isHovering) return;
        base.KnockOver();
    }
}
