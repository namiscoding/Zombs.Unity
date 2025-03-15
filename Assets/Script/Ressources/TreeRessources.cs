using UnityEngine;

public class TreeRessources : Ressources
{
    //protected override void Die()
    //{
    //    Debug.Log("Tree die");
    //    base.Die();
    //    gameManager.AddStone(quality);
    //}
    public override void TakeDamage()
    {
        if (animator != null)
        {
            // Reset animation state before setting it true again
            animator.SetBool("isCollect", false);
            //animator.Play("RockCollect", 0, 0f);
            animator.Play("TreeCollect", 0, 0f);
            animator.SetBool("isCollect", true);
            collectEndTime = Time.time + collectDuration;
        }
    }
}
