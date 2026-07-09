using UnityEngine;

public class Key : MonoBehaviour
{
    private bool isFollowing;
    public float followSpeed;
    public Transform followTarget;

    void Start()
    {
        
    }

    void Update()
    {
        if (isFollowing)
        {
            transform.position = Vector3.Lerp(transform.position, followTarget.position, followSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isFollowing)
            {
                PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
                followTarget = playerMovement.keyFollowPoint;

                isFollowing = true;
                playerMovement.followingKey = this;
            }

        }
    }
}


