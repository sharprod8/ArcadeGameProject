using UnityEngine;

public class FireSlimeEnemy : SlimeEnemy
{
    public override void TakeHit()
    {
        if (isKnockedOver)
            return;

        base.TakeHit();
    }

    public override void EnterExitPipe()
    {
        gameObject.AddComponent<SlimeEnemy>();
        Destroy(this);
    }
}
