using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class ScreenWrap : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D confiner;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        confiner = GameManager.instance.confiner;
    }

    void Update()
    {
        if (confiner == null)
            return;

        Vector2 pos = transform.position;

        float left = confiner.bounds.min.x;
        float right = confiner.bounds.max.x;

        if (pos.x < left && rb.linearVelocity.x < 0)
            pos.x = right;

        else if (pos.x > right && rb.linearVelocity.x > 0)
            pos.x = left;

        transform.position = pos;
    }

}
