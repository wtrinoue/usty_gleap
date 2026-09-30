using UnityEngine;

public class EnemyAttackOrbitBehaviour : MonoBehaviour
{

    public StatusContainer statusContainer;
    private Knockback knockback;

    void Awake()
    {
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

        StatusContainer playerSC = target.GetComponent<StatusContainer>();

        if (playerSC == null) return;

        DamageToken dt = new();
        dt.ExtractStatus(statusContainer.GetStatus());
        playerSC.ApplyOneTimeToken(dt);

        if (knockback != null)
        {
            knockback.DoKnockback(target);
        }
    }
}
