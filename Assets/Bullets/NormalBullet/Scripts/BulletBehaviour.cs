using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] private float lifeTimeSeconds = 5f; // 生存時間（秒）
    private StatusActionHolder statusActionHolder;
    private StatusContainer statusContainer;
    private TargetStatusAction attackAction;
    private Rigidbody2D rb;
    private float lifeTimer;
    private bool isDestroyed;

    void Awake()
    {
        statusActionHolder = GetComponent<StatusActionHolder>();
        statusContainer = GetComponent<StatusContainer>();
        rb = GetComponent<Rigidbody2D>();
        attackAction = statusActionHolder.GetTargetStatusActionFromIndex(0);

        // 寿命カウントをコルーチンで開始
        StartCoroutine(LifeTimeRoutine());
    }

    void Update()
    {
        if (isDestroyed) return;

        // ①毎フレームごとにTransform.rotationの方向に進む
        float speed = statusContainer.GetStatus().Calculate(StatusCategory.Speed);
        Vector2 direction = transform.right;
        rb.linearVelocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;

        // ②Enemyのオブジェクトにぶつかったときに、ダメージを与えてから消滅
        if (collision.CompareTag("Enemy") &&
            collision.TryGetComponent<StatusContainer>(out StatusContainer hasSC))
        {
            // AttackActionを用いて攻撃
            DamageToken dt = new DamageToken();
            dt.ExtractStatus(statusContainer.GetStatus());
            hasSC.ApplyOneTimeToken(dt);
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
