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

    [Header("Knock stuff")]
    public float knockDuration = 3f;
    public Color normalColor = Color.white;
    public Color knockedColor = Color.red;

    float knockTimer;
    SpriteRenderer sprite;
    Vector3 originalScale;

    private bool lockX = false;
    private float lockedX;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    protected virtual void Start()
    {
        currentSpeed = baseSpeed;
        if (sprite != null) sprite.color = normalColor;
    }

    protected virtual void Update()
    {
        if (isKnockedOver)
        {
            knockTimer -= Time.deltaTime;
            if (knockTimer <= 0f)
                RecoverFromKnock();
        }
    }

    protected virtual void FixedUpdate()
    {
        Debug.Log($"{name} direction during movement = {direction}");

        if (isKnockedOver)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        Move();
    }

    private void LateUpdate()
    {
        if (lockX)
        {
            transform.position = new Vector3(lockedX, transform.position.y, transform.position.z);
        }
    }



    // CHILDREN OVERRIDE THIS IF THEY WANT CUSTOM MOVEMENT
    protected virtual void Move()
    {
        rb.linearVelocity = new Vector2(direction * currentSpeed, rb.linearVelocity.y);
    }

    public void SetStartingDirection(Transform pipe)
    {
        direction = pipe.position.x < 0 ? 1 : -1;
        Debug.Log($"Spawned at {pipe.position.x}, direction = {direction}");
        Flip();
    }

    protected void Flip()
    {
        float sx = Mathf.Abs(originalScale.x);
        transform.localScale = new Vector3(sx * direction, originalScale.y, originalScale.z);
    }

    public virtual void KnockOver()
    {
        if (isKnockedOver)
            return;

        isKnockedOver = true;
        knockTimer = knockDuration;

        // freeze X position while knocked
        lockX = true;
        lockedX = transform.position.x;

        rb.linearVelocity = new Vector2(0, -2f);
        rb.gravityScale = 1f;

        currentSpeed = 0f;

        if (sprite != null) sprite.color = knockedColor;
    }



    private void RecoverFromKnock()
    {
        isKnockedOver = false;
        lockX = false;

        rb.linearVelocity = Vector2.zero;
        currentSpeed = baseSpeed;

        if (sprite != null) sprite.color = normalColor;
    }



    public virtual void TakeHit()
    {
        if (isBoss)
        {
            bossHP--;

            if (bossHP <= 0)
                Die();
            else
                rb.linearVelocity = new Vector2(-direction * 5f, 2f);
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
        SetStartingDirection(pipe);
    }

    public virtual void Die()
    {
        spawner.EnemyDied(this);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (usesWallDetection)
        {
            if (((1 << c.collider.gameObject.layer) & groundLayers) != 0)
            {
                foreach (var contact in c.contacts)
                {
                    if (Mathf.Abs(contact.normal.x) > 0.5f)
                    {
                        direction *= -1;
                        Flip();
                        return;
                    }
                }
            }
        }

        EnemyBase otherEnemy = c.collider.GetComponent<EnemyBase>();
        if (otherEnemy != null)
        {
            foreach (var contact in c.contacts)
            {
                if (contact.normal.x > 0.5f)
                {
                    direction *= -1;
                    Flip();
                    return;
                }
                if (contact.normal.x < -0.5f)
                {
                    otherEnemy.direction *= -1;
                    otherEnemy.Flip();
                    return;
                }
            }
        }
    }
}
