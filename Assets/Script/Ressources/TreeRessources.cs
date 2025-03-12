using UnityEngine;

public class TreeRessources : Ressources
{
    protected override void Die()
    {
        Debug.Log("Tree die");
        base.Die();
        gameManager.AddStone(quality);
    }
}
