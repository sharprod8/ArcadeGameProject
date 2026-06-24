using UnityEngine;

public class FlyEnemy : EnemyBase
{
    private float flyTimer;
    private bool isFlying;

    protected override void Start()
    {
        base.Start();
        baseSpeed = 0f;
    }

    private void FixedUpdate()
    {
        if (isKnockedOver) return;

        flyTimer += Time.deltaTime;

        if (flyTimer > 2f)
        {
            isFlying = true;
            rb.linearVelocity = new Vector2(0, 2f);

            if (flyTimer > 3f)
            {
                isFlying = false;
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
        if (isFlying) return;
        base.KnockOver();
    }
}
