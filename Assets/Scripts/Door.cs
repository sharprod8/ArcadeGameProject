using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    private PlayerMovement player;

    public SpriteRenderer doorSprite;
    public Sprite doorOpenSprite;

    public bool isOpen;
    public bool waitingToOpen;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        if (waitingToOpen)
        {
            if (Vector3.Distance(player.followingKey.transform.position, transform.position) < 0.1f)
            {
                waitingToOpen = false;
                isOpen = true;

                doorSprite.sprite = doorOpenSprite;
                player.followingKey.gameObject.SetActive(false);
                player.followingKey = null;
            }
        }

        if (isOpen && Vector3.Distance(player.transform.position, transform.position) < 1f)
        {
            LevelManager.instance.NextLevel();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player.followingKey != null)
            {
                player.followingKey.followTarget = transform;
                waitingToOpen = true;
            }
        }
    }
}

