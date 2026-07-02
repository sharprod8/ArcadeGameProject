using UnityEngine;
using UnityEngine.Tilemaps;
public enum EnemyType
{
     Basic,
     Frog,
     Fly,
     Ladybug,
     Saw
}

public class EnemyBaseV2 : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] EnemyType enemyType = EnemyType.Basic;

    [Header("References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] public EnemySpawner spawner;
    [SerializeField] BlockManager blockManager;

    [SerializeField] LayerMask layersToRaycastTo;

    [Header("Enemy Stats")]
    [SerializeField] float speed = 3f;
    [SerializeField] int speedStacks = 0;
    [SerializeField] int maxSpeedStacks = 2;

    [Header("Debug things")]
    [SerializeField] int startDirection = 1;
    [SerializeField] float knockTimer;
    [SerializeField] public bool isKnockedOver;

    [Header("Knock stuff")]
    public float knockDuration = 3f;
    public Color normalColor = Color.white;
    public Color knockedColor = Color.red;

    [Header("Frog Settings")]
    [SerializeField] float frogHopForce = 5f;
    [SerializeField] float frogTimeBetweenHops = 1.2f;
    [SerializeField] float frogTimer = 0f;
    [SerializeField] float frogHorizontalSpeed = 3f;
    float frogGroundIgnoreTime = 0.2f;
    float frogGroundIgnoreTimer = 0f;


    [Header("Fly Settings")]
    [SerializeField] float flyHoverHeight = 1.2f;
    [SerializeField] float flyRiseTime = 0.4f;
    [SerializeField] float flyHoverTime = 2f;
    [SerializeField] float flyIdleTime = 1.2f;
    float flyTimer = 0f;
    float flyGroundIgnoreTime = 0.2f;
    float flyGroundIgnoreTimer = 0f;
    bool flyIsHovering = false;
    bool flyIsRising = false;
    float flyGroundY;

    [Header("Saw Settings")]
    [SerializeField] Vector2 sawBoxSize = new Vector2(0.6f, 0.4f);
    [SerializeField] float sawBoxDistance = 0.1f;

    [Header("Ground Check")]
    public Transform groundCheckPosition;
    public Vector2 groundCheckSize = new Vector2(0.4f, 0.02f);
    public LayerMask groundLayer;
    bool isGrounded = false;

    private Vector2 movement;
    int currentDirection;
    float halfWidth;
    float halfHeight;

    private void Awake()
    {
        startDirection = Random.Range(0, 2) * 2 - 1;
        blockManager = FindObjectOfType<BlockManager>();
    }

    private void Start()
    {
        halfWidth = sprite.bounds.extents.x;
        halfHeight = sprite.bounds.extents.y;
        currentDirection = startDirection;
        if (startDirection == 1)
        {
            sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
        }

        if (enemyType == EnemyType.Fly)
            flyTimer = flyIdleTime;

        if (enemyType == EnemyType.Ladybug)
            flyTimer = flyIdleTime;

    }

    private void FixedUpdate()
    {

        switch (enemyType)
        {
            case EnemyType.Basic:
                SlimeMovement();
                break;

            case EnemyType.Frog:
                FrogMovement();
                break;

            case EnemyType.Fly:
                FlyMovement();
                break;

            case EnemyType.Ladybug:
                LadybugMovement();
                break;

            case EnemyType.Saw:
                SawMovement();
                break;
        }

        SetDirection();
    }

    private void Update()
    {
        if (isKnockedOver)
        {
            knockTimer -= Time.deltaTime;
            if (knockTimer <= 0f)
                RecoverFromKnock();
        }
    }

    private void SetDirection()
    {
        if (Physics2D.Raycast(transform.position, Vector2.right, halfWidth + 0.1f, layersToRaycastTo) && rb.linearVelocity.x > 0)
        {
            currentDirection *= -1;
            sprite.flipX = false;
        }
        else if (Physics2D.Raycast(transform.position, Vector2.left, halfWidth + 0.1f, layersToRaycastTo) && rb.linearVelocity.x < 0)
        {
            currentDirection *= -1;
            sprite.flipX = true;
        }
        Debug.DrawRay(transform.position, Vector2.right * (halfWidth + 0.1f), Color.red);
        Debug.DrawRay(transform.position, Vector2.left * (halfWidth + 0.1f), Color.red);
        //neeed to uncheck queries start in colliders in the project settings
        //https://stackoverflow.com/questions/24563085/raycast-but-ignore-the-collider-of-the-gameobject-its-being-called-from
    }

    public void KnockOver()
    {
        if (enemyType == EnemyType.Saw) return;

        isKnockedOver = true;
        knockTimer = knockDuration;

        rb.constraints = RigidbodyConstraints2D.FreezePositionX;

        sprite.color = Color.red;
    }

    private void RecoverFromKnock()
    {
        isKnockedOver = false;

        rb.constraints = RigidbodyConstraints2D.None;

        LeanTween.rotateZ(gameObject, 0f, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(FreezeRigidbodyRotation);

        sprite.color = Color.white;
    }

    private void FreezeRigidbodyRotation()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Die()
    {
        spawner.EnemyDied(this);
        Destroy(gameObject);
    }

    public void EnterExitPipe()
    {
        speedStacks = Mathf.Min(speedStacks + 1, maxSpeedStacks);
        speed += speedStacks * 0.75f;
        frogHorizontalSpeed += speedStacks * 0.5f;
        frogTimeBetweenHops -= 0.1f;

        Transform pipe = spawner.spawnPipes[Random.Range(0, spawner.spawnPipes.Length)];
        transform.position = pipe.position;

        isKnockedOver = false;
    }

    private void SlimeMovement()
    {
        movement.x = speed * currentDirection;
        movement.y = rb.linearVelocity.y;
        rb.linearVelocity = movement;
    }

    private void FrogMovement()
    {
        if (isKnockedOver) return;
        
        if (frogGroundIgnoreTimer > 0f)
            frogGroundIgnoreTimer -= Time.deltaTime;

        isGrounded = frogGroundIgnoreTimer <= 0f && IsGrounded();

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(0f, 0f);

            frogTimer -= Time.deltaTime;

            if (frogTimer > 0f)
                return;

            frogTimer = frogTimeBetweenHops;

            rb.AddForce(Vector2.up * frogHopForce, ForceMode2D.Impulse);
            rb.linearVelocity = new Vector2(currentDirection * frogHorizontalSpeed, rb.linearVelocity.y);
            frogGroundIgnoreTimer = frogGroundIgnoreTime;

            return;
        }
    }


    private void FlyMovement()
    {
        if (isKnockedOver) return;

        if (flyGroundIgnoreTimer > 0f)
        {
            flyGroundIgnoreTimer -= Time.deltaTime;
        }

        bool flyGrounded = flyGroundIgnoreTimer <= 0f && IsGrounded();

        if (!flyIsRising && !flyIsHovering)
        {
            if (flyGrounded)
                FlyGrounded();

            return;
        }

        if (flyIsHovering)
        {
            flyTimer -= Time.deltaTime;
            movement.x = speed * currentDirection;
            movement.y = rb.linearVelocity.y;
            rb.linearVelocity = movement;

            if (flyTimer <= 0f)
            {
                flyIsHovering = false;
                StartFlyDrop();
            }

            return;
        }
    }

    private void LadybugMovement()
    {
        if (isKnockedOver) return;

        if (flyGroundIgnoreTimer > 0f)
            flyGroundIgnoreTimer -= Time.deltaTime;

        bool grounded = flyGroundIgnoreTimer <= 0f && IsGrounded();

        if (!flyIsRising && !flyIsHovering)
        {
            movement.x = speed * currentDirection;
            movement.y = rb.linearVelocity.y;
            rb.linearVelocity = movement;

            flyTimer -= Time.deltaTime;

            if (flyTimer > 0f)
                return;

            flyTimer = flyRiseTime;
            flyIsRising = true;
            flyGroundY = transform.position.y;

            StartFlyRise();
            return;
        }

        if (flyIsHovering)
        {
            flyTimer -= Time.deltaTime;

            movement.x = speed * currentDirection;
            movement.y = rb.linearVelocity.y;
            rb.linearVelocity = movement;

            if (flyTimer <= 0f)
            {
                flyIsHovering = false;
                StartFlyDrop();
            }

            return;
        }
    }

    private void SawMovement()
    {
        movement.x = speed * currentDirection;
        movement.y = rb.linearVelocity.y;
        rb.linearVelocity = movement;

        Vector2 origin = transform.position;
        Vector2 dir;
        if (currentDirection == 1)
        {
            dir = Vector2.right;
        }
        else
        {
            dir = Vector2.left;
        }

        Vector2 boxCenter = origin + dir * sawBoxDistance;

        Vector2 half = sawBoxSize * 0.5f;
        Vector2 min = boxCenter - half;
        Vector2 max = boxCenter + half;

        bool hitSolid = false;

        float step = 0.1f;

        for (float x = min.x; x <= max.x; x += step)
        {
            for (float y = min.y; y <= max.y; y += step)
            {
                Vector3Int tilePos = blockManager.tilemap.WorldToCell(new Vector3(x, y, 0));
                BlockTile tile = blockManager.tilemap.GetTile<BlockTile>(tilePos);

                if (tile == null || tile.data == null)
                    continue;

                BlockData data = tile.data;

                if (data.blockType == BlockData.BlockType.Brick)
                {
                    blockManager.HitBlock(tilePos);
                }
                else
                {
                    hitSolid = true;
                }
            }
        }


        if (hitSolid)
        {
            currentDirection *= -1;
            sprite.flipX = currentDirection == -1;
        }
    }



    private void FlyGrounded()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        flyTimer -= Time.deltaTime;

        if (flyTimer > 0f)
            return;

        flyTimer = flyRiseTime;
        flyIsRising = true;

        flyGroundY = transform.position.y;

        StartFlyRise();
    }

    private void StartFlyRise()
    {
        float targetY = flyGroundY + flyHoverHeight;

        rb.bodyType = RigidbodyType2D.Kinematic;

        LeanTween.moveY(gameObject, targetY, flyRiseTime)
            .setEaseOutSine()
            .setOnComplete(StartFlyHover);

        flyGroundIgnoreTimer = flyGroundIgnoreTime;
    }

    private void StartFlyHover()
    {
        flyIsRising = false;
        flyIsHovering = true;

        flyTimer = flyHoverTime;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
    }


    private void StartFlyDrop()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        flyTimer = flyIdleTime;
        flyGroundIgnoreTimer = flyGroundIgnoreTime;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPosition.position, groundCheckSize, 0f, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPosition.position, groundCheckSize);
    }

    private void OnDrawGizmos()
    {
        if (enemyType != EnemyType.Saw) return;

        Vector2 origin = transform.position;
        Vector2 dir;
        if (currentDirection == 1)
        {
            dir = Vector2.right;
        }
        else
        {
            dir = Vector2.left;
        }

        Vector2 boxCenter = origin + dir * sawBoxDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boxCenter, sawBoxSize);
    }


}
