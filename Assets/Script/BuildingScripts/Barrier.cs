using UnityEngine;

public class Barrier : Building
{
    protected override void Start()
    {
        base.Start();
        BarrierData barrierData = (BarrierData)data;
        Collider2D collider = gameObject.AddComponent<BoxCollider2D>();
        if (barrierData.isDoor)
        {
            collider.isTrigger = true; // Player can pass, enemies blocked
        }
    }
}