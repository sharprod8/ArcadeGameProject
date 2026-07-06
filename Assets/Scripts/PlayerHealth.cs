using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHearts = 3;
    public int currentHearts;

    public bool isDead;

    public Transform respawnPoint;

    private PlayerMovement movement;
    private SpriteRenderer sprite;

    private void Awake()
    {
        currentHearts = maxHearts;
        movement = GetComponent<PlayerMovement>();
        sprite = GetComponent<SpriteRenderer>();


    }

    private void Start()
    {
        GameManager.instance.players.Clear();
        
        if (GameManager.instance != null)
            GameManager.instance.RegisterPlayer(this);
    }

    public void TakeDamage()
    {
        if (isDead)
            return;

        currentHearts--;

        if (currentHearts <= 0)
        {
            Die();
            return;
        }

        Respawn();
    }

    public void Respawn()
    {
        movement.enabled = false;
        sprite.enabled = false;

        transform.position = respawnPoint.position;

        movement.enabled = true;
        sprite.enabled = true;
    }

    public void Die()
    {
        isDead = true;
        movement.enabled = false;
        sprite.enabled = false;

        GameManager.instance.PlayerDied(this);
    }

    public void ReviveWithOneHeart()
    {
        isDead = false;
        currentHearts = Mathf.Min(currentHearts + 1, maxHearts);

        movement.enabled = true;
        sprite.enabled = true;

        transform.position = respawnPoint.position;
    }

    public void FullRevive()
    {
        isDead = false;
        currentHearts = maxHearts;

        movement.enabled = true;
        sprite.enabled = true;

        transform.position = respawnPoint.position;
    }
}
