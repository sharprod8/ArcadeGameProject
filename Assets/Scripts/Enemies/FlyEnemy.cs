using UnityEngine;

public class FlyEnemy : EnemyBase
{
    private float flyTimer;
    private bool flying;

    protected override void Start()
    {
        base.Start();
        baseSpeed = 0f;
        usesWallDetection = false;
    }

    private void FixedUpdate()
    {
        if (isKnockedOver) return;

        flyTimer += Time.deltaTime;

        if (flyTimer > 2f)
        {
            flying = true;
            rb.linearVelocity = new Vector2(0, 2f);

            if (flyTimer > 3f)
            {
                flying = false;
                flyTimer = 0;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public override void KnockOver()
    {
        if (flying) return;
        base.KnockOver();
    }
}
