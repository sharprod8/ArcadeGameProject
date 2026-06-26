using UnityEngine;

public class WormEnemy : EnemyBase
{   
    private Vector2 moveDir = Vector2.right;

    protected override void Start()
    {
        base.Start();
        baseSpeed = 1.2f;
        currentSpeed = baseSpeed;
        usesWallDetection = false;
    }

    private void FixedUpdate()
    {
        if (isKnockedOver) return;

        rb.linearVelocity = moveDir * currentSpeed;

        RaycastHit2D groundCheck = Physics2D.Raycast(transform.position + (Vector3)moveDir * 0.3f, Vector2.down, 0.5f, groundLayers);

        if (!groundCheck)
        {
            moveDir = new Vector2(moveDir.y, -moveDir.x);
            if (moveDir.x > 0)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
            Flip();
        }
    }

    public override void TakeHit()
    {
        RaycastHit2D upCheck = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayers);

        if (!upCheck)
            return;

        base.TakeHit();
    }
}
