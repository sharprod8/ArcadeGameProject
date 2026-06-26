using UnityEngine;

public class SlimeEnemy : EnemyBase
{
    protected override void Start()
    {
        base.Start();
        baseSpeed = 1.5f;
        currentSpeed = baseSpeed;
        usesWallDetection = true;
    }
}
