using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    //https://gamedevbeginner.com/glossary/what-are-virtual-and-override-functions-in-unity/

    //https://gamedevbeginner.com/what-public-private-and-protected-mean-in-unity/

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

    [Header("Direction settings")]
    [SerializeField] private int startDirection = 1;
    private int currentDirection;
    private float halfWidth;

    private Vector2 movement;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    void Start()
    {
        halfWidth = sprite.bounds.extents.x;
        
        //currentSpeed = baseSpeed;
        if (sprite != null) sprite.color = normalColor;
    }

    /*protected virtual void Update()
    {
        if (isKnockedOver)
        {
            knockTimer -= Time.deltaTime;
            if (knockTimer <= 0f)
                RecoverFromKnock();
        }
    }*/

    void FixedUpdate()
    {
        Debug.Log($"{name} direction during movement = {direction}");

        if (isKnockedOver)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        Move();
    }

    /*private void LateUpdate()
    {
        if (lockX)
        {
            transform.position = new Vector3(lockedX, transform.position.y, transform.position.z);
        }
    }*/



    
    void Move()
    {
        //rb.linearVelocity = new Vector2(direction * currentSpeed, rb.linearVelocity.y);

        movement.x = baseSpeed * currentDirection;
        movement.y = rb.linearVelocity.y;
        rb.linearVelocity = movement;
    }

    /*public void SetStartingDirection(Transform pipe)
    {
        if (pipe.position.x < 0)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
        Debug.Log($"Spawned at {pipe.position.x}, facing = {direction}");
        Flip();
    }*/

    /*protected void Flip()
    {
        float x = Mathf.Abs(originalScale.x);
        transform.localScale = new Vector3(x * direction, originalScale.y, originalScale.z);
    }*/

    /*public virtual void KnockOver()
    {
        if (isKnockedOver)
            return;

        isKnockedOver = true;
        knockTimer = knockDuration;

        //xant move left or right
        lockX = true;
        lockedX = transform.position.x;

        rb.linearVelocity = new Vector2(0, -2f);
        rb.gravityScale = 1f;

        currentSpeed = 0f;

        if (sprite != null) sprite.color = knockedColor;
    }*/



    /*private void RecoverFromKnock()
    {
        isKnockedOver = false;
        lockX = false;

        rb.linearVelocity = Vector2.zero;
        currentSpeed = baseSpeed;

        if (sprite != null) sprite.color = normalColor;
    }*/



    /*public virtual void TakeHit()
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
    }*/

    /*public virtual void EnterExitPipe()
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
    }*/

    /* virtual void Die()
    {
        spawner.EnemyDied(this);
        Destroy(gameObject);
    }*/

    /*private void OnCollisionEnter2D(Collision2D c)
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
    }*/


}
