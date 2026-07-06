using UnityEngine;

public class Coin : MonoBehaviour, Item
{
    public void Collect()
    {
        GameManager.instance.AddCoinToCount();
        Destroy(gameObject);
    }
}
