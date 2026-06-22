using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public ParticleSystem skidFX;

    [Header("Movement")]
    public float moveSpeed = 5f;
    private float horizontalMovement;

    [Header("Jumping")]
    public float jumpForce = 8f;
    private bool jumpHeld;

    [Header("Gravity")]
    public float lowJumpGravity = 1.2f;
    public float normalGravity = 2.5f;
    public float fallGravity = 4f;

    [Header("Ground Check")]
    public Transform groundCheckPosition;
    public Vector2 groundCheckSize = new Vector2(0.4f, 0.02f);
    public LayerMask groundLayer;
    
    [SerializeField] private bool isSkidding;
    [SerializeField] private bool hasHitBlock;

    void Update()
    {
        CheckForSkidding();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyGravity();
        CheckBlockHit();
    }

    private void ApplyGravity()
    {
        if (rb.linearVelocity.y > 0) //rising
        {
            if (jumpHeld)
            {
                rb.gravityScale = lowJumpGravity;
            }
            else
            {
                rb.gravityScale = normalGravity;
            }
        }
        else if (rb.linearVelocity.y < 0) //falling
        {
            rb.gravityScale = fallGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
    }

    private void ApplyMovement()
    {
        float targetSpeed = horizontalMovement * moveSpeed;
        float speedDifference = targetSpeed - rb.linearVelocity.x;

        float accelRate;

        if (IsGrounded())
        {
            if (Mathf.Abs(targetSpeed) > 0.01f)
            {
                accelRate = 2f; //accel on ground
            }
            else
            {
                accelRate = 3f; //decel on ground
            }
        }
        else
        {
            if (Mathf.Abs(targetSpeed) > 0.01f)
            {
                accelRate = 3f;   //accel in air
            }
            else
            {
                accelRate = 4f;   //decel in air
            }
        }

        float movement = accelRate * speedDifference;
        rb.AddForce(Vector2.right * movement);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && IsGrounded())
        {
            jumpHeld = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (context.canceled)                                                                                                                                      
        {
            jumpHeld = false;
        }
    }

    private void CheckForSkidding()
    {
        bool changingDirection = false;

        if (horizontalMovement > 0 && rb.linearVelocity.x < 0)
        {
            changingDirection = true;
        }
        else if (horizontalMovement < 0 && rb.linearVelocity.x > 0)
        {
            changingDirection = true;
        }
        
        isSkidding = IsGrounded() && changingDirection;

        if (isSkidding)
        {
            skidFX.Play();
        }
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

    /*private void CheckBlockHit()
    {
        if (rb.linearVelocity.y <= 0)
            return;

        if (hasHitBlock)
            return;

        Vector2 boxSize = new Vector2(0.2f, 0.1f);
        Vector2 boxCentre = new Vector2(transform.position.x, transform.position.y + 0.6f);

        Collider2D[] hit = Physics2D.OverlapBoxAll(boxCentre, boxSize, 0f);
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].CompareTag("Tilemap"))
            {
                Tilemap tilemap = hit[i].GetComponent<Tilemap>();

                if (tilemap != null)
                {
                    BlockManager manager = tilemap.GetComponent<BlockManager>();
                    if (manager != null)
                    {
                        manager.HitBlock(boxCentre);
                        Debug.Log($"Hit block at {boxCentre}");
                        break;
                    }
                }
            }
        }
    }*/

    private void CheckBlockHit()
    {
        Vector2 boxSize = new Vector2(0.8f, 0.1f);
        Vector2 boxCentre = new Vector2(transform.position.x, transform.position.y + 0.6f);

        Collider2D[] hit = Physics2D.OverlapBoxAll(boxCentre, boxSize, 0f);

        Tilemap overlappedTilemap = null;
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].CompareTag("Tilemap"))
            {
                overlappedTilemap = hit[i].GetComponent<Tilemap>();
                if (overlappedTilemap != null) break;
            }
        }

        bool overlappingBlock = overlappedTilemap != null;

        if (overlappingBlock && rb.linearVelocity.y > 0 && !hasHitBlock)
        {
            BlockManager manager = overlappedTilemap.GetComponent<BlockManager>();
            if (manager != null)
            {
                manager.HitBlock(boxCentre);
                Debug.Log($"Hit block at {boxCentre}");
                hasHitBlock = true;
            }
        }

        if (!overlappingBlock)
        {
            hasHitBlock = false;
        }
    }
}
