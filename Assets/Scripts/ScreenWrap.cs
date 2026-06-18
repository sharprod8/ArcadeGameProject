using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class ScreenWrap : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        float rightSideOfScreenInWorld = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x;
        float leftSideOfScreenInWorld = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).x;

        float topOfScreenInWorld = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).y;
        float bottomOfScreenInWorld = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).y;

        //if moving thru left side of screen
        if (screenPosition.x <= 0 && rb.linearVelocity.x < 0)
        {
            transform.position = new Vector2(rightSideOfScreenInWorld, transform.position.y);
        }

        //if moving thru right side of screen
        else if (screenPosition.x >= Screen.width && rb.linearVelocity.x > 0)
        {
            transform.position = new Vector2(leftSideOfScreenInWorld, transform.position.y);
        }
    }
}
