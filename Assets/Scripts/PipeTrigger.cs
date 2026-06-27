using UnityEngine;

public class PipeTrigger : MonoBehaviour
{
    public bool isExitPipe = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyBaseV2 enemy = other.GetComponent<EnemyBaseV2>();
        if (enemy == null) return;

        if (isExitPipe)
        {
            enemy.EnterExitPipe();
        }
    }
}
