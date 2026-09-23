using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] private float lifeTimeSeconds = 5f; // 生存時間（秒）
    private StatusActionHolder statusActionHolder;
    private StatusManager statusManager;
    private StatusContainer statusContainer;
    private TargetStatusAction attackAction;
    private Rigidbody2D rb;
    private float lifeTimer;
    private bool isDestroyed;

    void Awake()
    {
        statusActionHolder = GetComponent<StatusActionHolder>();
        statusManager = GetComponent<StatusManager>();
        rb = GetComponent<Rigidbody2D>();
        attackAction = statusActionHolder.GetTargetStatusActionFromIndex(0);

        // 寿命カウントをコルーチンで開始
        StartCoroutine(LifeTimeRoutine());
    }

    void Update()
    {
        if (isDestroyed) return;

        // ①毎フレームごとにTransform.rotationの方向に進む
        float speed = statusManager.BaseStatus.BaseSpeed;
        Vector2 direction = transform.right;
        rb.linearVelocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;

        // ②Enemyのオブジェクトにぶつかったときに、ダメージを与えてから消滅
        if (collision.CompareTag("Enemy") &&
            collision.TryGetComponent<IHasStatusManager>(out var hasStatus))
        {
            // AttackActionを用いて攻撃
            attackAction.Execute(this.gameObject, collision.gameObject);
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
