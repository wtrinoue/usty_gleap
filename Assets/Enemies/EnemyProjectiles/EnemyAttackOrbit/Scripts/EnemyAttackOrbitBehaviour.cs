using UnityEngine;

public class EnemyAttackOrbitBehaviour : MonoBehaviour
{
    private StatusActionHolder statusActionHolder;
    private TargetStatusAction attackAction;
    private Knockback knockback;

    void Awake()
    {
        statusActionHolder = GetComponent<StatusActionHolder>();
        if (statusActionHolder != null)
        {
            attackAction = statusActionHolder.GetTargetStatusActionFromIndex(0);
        }
        knockback = GetComponent<Knockback>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryAttack(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        TryAttack(collision.gameObject);
    }

    private void TryAttack(GameObject target)
    {
        if (!target.CompareTag("Player")) return;

        if (attackAction != null)
        {
            attackAction.Execute(this.gameObject, target);
        }

        if (knockback != null)
        {
            knockback.DoKnockback(target);
        }
    }
}
