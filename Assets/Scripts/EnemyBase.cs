using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Core")]
    public EnemySpawner spawner;
    public Rigidbody2D rb;

    [Header("State")]
    public bool isKnockedOver = false;
    public bool isBoss = false;
    public int bossHP = 1;

    [Header("Settings")]
    public LayerMask groundLayers;
    public bool usesWallDetection = true;
    public int direction = 1;

    [Header("Speed Stacks")]
    public int speedStacks = 0;
    public int maxSpeedStacks = 2;

    protected float baseSpeed = 1.5f;
    protected float currentSpeed;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = baseSpeed;
    }

    public void SetStartingDirection(Transform pipe)
    {
        direction = pipe.position.x < 0 ? 1 : -1;
        transform.localScale = new Vector3(direction, 1, 1);
    }
    public virtual void KnockOver()
    {
        if (isKnockedOver) return;

        isKnockedOver = true;
        rb.linearVelocity = new Vector2(0, -2f);
    }

    public virtual void TakeHit()
    {
        if (isBoss)
        {
            bossHP--;

            if (bossHP <= 0)
            {
                Die();
            }
            else
            {
                rb.linearVelocity = new Vector2(-transform.localScale.x * 5f, 2f);
            }
        }
        else
        {
            Die();
        }
    }

    public virtual void EnterExitPipe()
    {
        if (!isBoss)
        {
            speedStacks = Mathf.Min(speedStacks + 1, maxSpeedStacks);
            currentSpeed = baseSpeed + speedStacks * 0.75f;
        }

        Transform pipe = spawner.spawnPipes[Random.Range(0, spawner.spawnPipes.Length)];
        transform.position = pipe.position;

        isKnockedOver = false;
    }

    public virtual void Die()
    {
        spawner.EnemyDied(this);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (!usesWallDetection) return;

        if (((1 << c.collider.gameObject.layer) & groundLayers) == 0)
            return;

        foreach (var contact in c.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                direction *= -1;
                transform.localScale = new Vector3(direction, 1, 1);
                return;
            }
        }
    }
}
