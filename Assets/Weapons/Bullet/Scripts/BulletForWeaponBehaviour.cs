using UnityEngine;

public class BulletForWeaponBehaviour : MonoBehaviour
{
    [SerializeField] private float lifeTimeSeconds = 5f; // 生存時間（秒）

    private StatusContainer statusContainer;
    private Rigidbody2D rb;
    private float lifeTimer;
    private bool isDestroyed;
    private Vector2 moveDirection;

    void Awake()
    {
        statusContainer = GetComponent<StatusContainer>();
        rb = GetComponent<Rigidbody2D>();
        moveDirection = transform.right.normalized;

        // 寿命カウントをコルーチンで開始
        StartCoroutine(LifeTimeRoutine());
    }

    void Update()
    {
        if (isDestroyed) return;

        // ①毎フレームごとにTransform.rotationの方向に進む
        float speed = statusContainer.GetStatus().Calculate(StatusCategory.Speed);
        rb.linearVelocity = moveDirection * speed;

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;

        // ②Enemyのオブジェクトにぶつかったときに、ダメージを与えてから消滅
        if (collision.CompareTag("Enemy") &&
            collision.TryGetComponent<StatusContainer>(out var hasSC))
        {
            // DamageTokenの発行
            DamageToken dt = new();
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
