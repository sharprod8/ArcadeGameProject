using UnityEngine;

public class EnemyBaseV2 : MonoBehaviour
{
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

    private Vector2 movement;
    int currentDirection;
    float halfWidth;


    private void Awake()
    {
        startDirection = Random.Range(0, 2) * 2 - 1; // maps 0->-1, 1->1
    }

    private void Start()
    {
        halfWidth = sprite.bounds.extents.x;
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
        movement.x = speed * currentDirection;
        movement.y = rb.linearVelocity.y;
        rb.linearVelocity = movement;

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
    }

    private void RecoverFromKnock()
    {
        isKnockedOver = false;

        rb.constraints = RigidbodyConstraints2D.None;

        LeanTween.rotateZ(gameObject, 0f, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(FreezeRigidbodyRotation);
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
        speed = speed + speedStacks * 0.75f;

        Transform pipe = spawner.spawnPipes[Random.Range(0, spawner.spawnPipes.Length)];
        transform.position = pipe.position;

        isKnockedOver = false;
    }
}
