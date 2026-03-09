using UnityEngine;
[RequireComponent(typeof(Animator))]
public class FollowEnemyAnimation : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Idle()
    {
        animator.SetBool("isRun", false);
    }
    public void Run()
    {
        animator.SetBool("isRun", true);
    }
    public void Attack()
    {
        animator.SetTrigger("Attack");
    }
    public void Death()
    {
        animator.SetTrigger("Death");
    }
    public void Hurt()
    {
        animator.SetTrigger("Hurt");
    }
}
