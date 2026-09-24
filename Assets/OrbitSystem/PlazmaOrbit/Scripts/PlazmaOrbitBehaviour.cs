using UnityEngine;

public class PlazmaOrbitBehaviour : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField] private ModifierDefinition modifierDefinition;
    private StatusContainer statusContainer;

    void Awake()
    {
        statusContainer = GetComponent<StatusContainer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            StatusContainer enemySC = collision.gameObject.GetComponent<StatusContainer>();
            if (enemySC == null) return;
            // 攻撃用Tokenを発行
            DamageToken dt = new();
            dt.ExtractStatus(statusContainer.GetStatus());
            enemySC.ApplyOneTimeToken(dt);

            // Stun effect application
            Modifier modifier = new Modifier(modifierDefinition, this.gameObject);
            enemySC.AddModifier(modifier);
        }
    }
}