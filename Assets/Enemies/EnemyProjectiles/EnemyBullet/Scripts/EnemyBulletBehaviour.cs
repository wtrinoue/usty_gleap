using UnityEngine;

public class EnemyBulletBehaviour : MonoBehaviour
{
    [SerializeField] private float lifeTimeSeconds = 5f;

    private StatusActionHolder statusActionHolder;
    private StatusManager statusManager;
    private TargetStatusAction attackAction;
    private Rigidbody2D rb;
    private bool isDestroyed;

    void Awake()
    {
        statusActionHolder = GetComponent<StatusActionHolder>();
        statusManager = GetComponent<StatusManager>();
        rb = GetComponent<Rigidbody2D>();
        if (statusActionHolder != null)
        {
            attackAction = statusActionHolder.GetTargetStatusActionFromIndex(0);
        }

        StartCoroutine(LifeTimeRoutine());
    }

    void Update()
    {
        if (isDestroyed) return;
        if (rb == null || statusManager == null) return;

        float speed = statusManager.BaseStatus.BaseSpeed;
        Vector2 direction = transform.right;
        rb.linearVelocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;

        if (collision.CompareTag("Player") &&
            collision.TryGetComponent<IHasStatusManager>(out var hasStatus))
        {
            if (attackAction != null)
            {
                attackAction.Execute(this.gameObject, collision.gameObject);
            }
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
