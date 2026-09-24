using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class AttackOrbitBehaviour : MonoBehaviour
{
    [SerializeField] private StatusContainer statusContainer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            DamageToken dt = new();
            dt.ExtractStatus(statusContainer.GetStatus());
            enemySC.ApplyOneTimeToken(dt);
        }
    }
}
