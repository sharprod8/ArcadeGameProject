using UnityEngine;

public class FrogEnemy : EnemyBase
{
    public float hopForce = 6f;
    private bool grounded;

    protected override void Start()
    {
        base.Start();
        baseSpeed = 0f;
        usesWallDetection = false;
    }

    private void FixedUpdate()
    {
        if (isKnockedOver) return;

        if (grounded)
        {
            rb.linearVelocity = new Vector2(direction * 0.5f, hopForce);
            grounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        foreach (var contact in c.contacts)
        {
            if (contact.normal.y > 0.5f)
                grounded = true;
        }
    }
}
