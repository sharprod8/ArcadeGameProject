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

    private void CheckBlockHit()
    {
        if (rb.linearVelocity.y <= 0)
        {
            hasHitBlock = false;
        }

        if (rb.linearVelocity.y <= 0 || hasHitBlock)
            return;

        Vector2 boxSize = new Vector2(0.5f, 0.1f);
        Vector2 boxCentre = new Vector2(transform.position.x, transform.position.y + 0.6f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCentre, boxSize, 0f);

        Tilemap overlappedTilemap = null;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].CompareTag("Tilemap"))
            {
                overlappedTilemap = hits[i].GetComponent<Tilemap>();
                if (overlappedTilemap != null) break;
            }
        }

        if (overlappedTilemap == null)
            return;

        BlockManager manager = overlappedTilemap.GetComponent<BlockManager>();
        if (manager == null)
            return;

        Vector3Int cellPos = overlappedTilemap.WorldToCell(boxCentre);

        //player hits the top of its current cell instead of the bottom of the target cell, so this just counters that
        cellPos += Vector3Int.up;

        //tries to hit the cell
        Vector3Int chosenCell = cellPos;
        BlockTile tile = overlappedTilemap.GetTile<BlockTile>(cellPos);

        if (tile == null)
        {
            //checks nearby cells
            Vector3Int leftCell = cellPos + new Vector3Int(-1, 0, 0);
            Vector3Int rightCell = cellPos + new Vector3Int(1, 0, 0);

            BlockTile leftTile = overlappedTilemap.GetTile<BlockTile>(leftCell);
            BlockTile rightTile = overlappedTilemap.GetTile<BlockTile>(rightCell);

            float playerX = transform.position.x;
            bool hasLeft = leftTile != null;
            bool hasRight = rightTile != null;

            if (!hasLeft && !hasRight)
            {
                return;
            }
            else if (hasLeft && !hasRight)
            {
                chosenCell = leftCell;
            }
            else if (!hasLeft && hasRight)
            {
                chosenCell = rightCell;
            }
            else
            {
                //choose cell closest to player
                Vector3 leftWorld = overlappedTilemap.GetCellCenterWorld(leftCell);
                Vector3 rightWorld = overlappedTilemap.GetCellCenterWorld(rightCell);

                float distLeft = Mathf.Abs(playerX - leftWorld.x);
                float distRight = Mathf.Abs(playerX - rightWorld.x);

                if (distLeft <= distRight)
                {
                    chosenCell = leftCell;
                }
                else
                {
                    chosenCell = rightCell;
                }
            }
        }

        manager.HitBlock(chosenCell);
        Debug.Log($"hit block at cell: {chosenCell}");
        hasHitBlock = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyBase enemy = collision.collider.GetComponent<EnemyBase>();

        if (enemy != null && enemy.isKnockedOver)
        {
            //enemy.Die();
        }
    }

}
