using UnityEngine;

public class Parallax : MonoBehaviour
{
    Renderer rend;
    public float speed = 0.5f;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    void Update()
    {
        float offset = Time.time * speed;
        rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0f));
    }
}
