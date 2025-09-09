using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private bool isAttackState = false;
    private PlayerManager playerManager; // To check current weapon state

    void Start()
    {
        animator = GetComponent<Animator>();
        playerManager = FindObjectOfType<PlayerManager>(); // Get reference to PlayerManager
        if (animator == null)
        {
            Debug.LogError("Animator not found on " + gameObject.name);
        }
        if (playerManager == null)
        {
            Debug.LogError("PlayerManager not found in scene!");
        }
    }

    void Update()
    {
        ChangeAnimation();
    }

    void ChangeAnimation()
    {
        // Only update animation if animator is enabled (i.e., not using bow)
        if (animator != null && animator.enabled)
        {
            // Hold mouse button - continuous attack
            if (Input.GetMouseButton(1))
            {
                isAttackState = true;
                animator.SetBool("isAttack", isAttackState);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                isAttackState = false;
                animator.SetBool("isAttack", isAttackState);
            }

            // Space key toggle
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isAttackState = !isAttackState;
                animator.SetBool("isAttack", isAttackState);
                Debug.Log("isAttackState: " + isAttackState);
            }
        }
    }
}