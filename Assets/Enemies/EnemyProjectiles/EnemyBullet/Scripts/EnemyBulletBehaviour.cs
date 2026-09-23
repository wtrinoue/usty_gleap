using UnityEngine;

public class EnemyBulletBehaviour : MonoBehaviour
{
    [SerializeField] private float lifeTimeSeconds = 5f;

    private StatusContainer statusContainer;
    private Rigidbody2D rb;
    private bool isDestroyed;

    void Awake()
    {
        statusContainer = GetComponent<StatusContainer>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(LifeTimeRoutine());
    }

    void Update()
    {
        if (isDestroyed) return;
        if (rb == null || statusContainer == null) return;

        float speed = statusContainer.GetStatus().Calculate(StatusCategory.Speed);
        Vector2 direction = transform.right;
        rb.linearVelocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;

        if (collision.CompareTag("Player") &&
            collision.TryGetComponent<StatusContainer>(out var hasSC))
        {
            DamageToken damageToken = new DamageToken();
            damageToken.ExtractStatus(statusContainer.GetStatus());
            hasSC.ApplyOneTimeToken(damageToken);
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        Destroy(this.gameObject);
    }

    private System.Collections.IEnumerator LifeTimeRoutine()
    {
        yield return new WaitForSeconds(lifeTimeSeconds);
        DestroyBullet();
    }
}
