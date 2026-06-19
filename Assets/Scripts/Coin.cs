using UnityEngine;

public class Coin : MonoBehaviour, Item
{
    public void Collect()
    {
        Destroy(gameObject);
    }
}
