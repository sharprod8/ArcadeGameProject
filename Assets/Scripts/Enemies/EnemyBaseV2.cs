using UnityEngine;
public enum EnemyType
{
     Slime,
     Frog,
     Fly
}

public class EnemyBaseV2 : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] EnemyType enemyType = EnemyType.Slime;

    [Header("References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sprite;
    public EnemySpawner spawner;

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
    float frogGroundIgnoreTime = 0.1f;
    float frogGroundIgnoreTimer = 0f;

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
    }

    private void FixedUpdate()
    {

        switch (enemyType)
        {
            case EnemyType.Slime:
                SlimeMovement();
                break;

            case EnemyType.Frog:
                FrogMovement();
                break;

            case EnemyType.Fly:
                FlyMovement();
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
        // e
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


}
