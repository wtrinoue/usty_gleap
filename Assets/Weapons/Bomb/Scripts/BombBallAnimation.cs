using UnityEngine;
[RequireComponent(typeof(Animator))]
public class BombBallAnimation : MonoBehaviour
{
    private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Explosion()
    {
        animator.SetTrigger("explosion");
    }
}
