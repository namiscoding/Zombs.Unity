using UnityEngine;

public class CheckAnimator : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            animator.SetBool("isNearPlayer", distance < 5f);
        }
    }
}
