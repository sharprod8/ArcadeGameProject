using UnityEngine;

public class PipeTrigger : MonoBehaviour
{
    public bool isExitPipe = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy == null) return;

        if (isExitPipe)
        {
            enemy.EnterExitPipe();
        }
    }
}
