using UnityEngine;

public class RockRessources : Ressources
{

    //protected override void Die()
    //{
    //    Debug.Log("Rock die");
    //    base.Die();
    //    gameManager.AddWood(quality);
    //}
    public override void TakeDamage()
    {
        if (animator != null)
        {
            // Reset animation state before setting it true again
            animator.SetBool("isCollect", false);
            animator.Play("RockCollect", 0, 0f);
            //animator.Play("TreeCollect", 0, 0f);
            animator.SetBool("isCollect", true);
            collectEndTime = Time.time + collectDuration;
        }
    }
}
