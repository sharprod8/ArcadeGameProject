using UnityEngine;

public class FrogEnemy : EnemyBase
{
    public float hopForce = 6f;
    private bool isGrounded;

    protected override void Start()
    {
        base.Start();
        baseSpeed = 0f;
    }

    private void FixedUpdate()
    {
        if (isKnockedOver) return;

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, hopForce);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    public override void KnockOver()
    {
        if (!isGrounded) return;
        base.KnockOver();
    }
}
