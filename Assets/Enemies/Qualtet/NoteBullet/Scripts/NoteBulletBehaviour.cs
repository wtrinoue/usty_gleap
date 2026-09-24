using UnityEngine;

public class NoteBulletBehaviour : MonoBehaviour
{
    [SerializeField] private float lifeTimeSeconds = 5f; // 生存時間（秒）
    private StatusContainer statusContainer;
    private Rigidbody2D rb;
    private float lifeTimer;
    private bool isDestroyed;
    private Transform player;
    private Vector2 direction;
    private bool isChangingDirection = true;
    private bool isGameOver = false;

    private bool isTenor = false;
    private bool isLead = false;
    private bool isBaritone = false;
    private bool isBass = false;

    void Awake()
    {
        statusContainer = GetComponent<StatusContainer>();
        rb = GetComponent<Rigidbody2D>();

        // 寿命カウントをコルーチンで開始
        StartCoroutine(LifeTimeRoutine());
    }

    void Update()
    {
        if (isDestroyed) return;

        // ①毎フレームごとにTransform.rotationの方向に進む
        float speed = GetSpeedByTenor(isTenor);
        direction = GetDirectionByBaritone(isBaritone);
        rb.linearVelocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;

        // ②Enemyのオブジェクトにぶつかったときに、ダメージを与えてから消滅
        if (collision.CompareTag("Enemy") &&
            collision.TryGetComponent<StatusContainer>(out var hasSC))
        {
            // AttackActionを用いて攻撃
            DamageToken dt = new();
            dt.ExtractStatus(statusContainer.GetStatus());

            // 相手のStatusContainerに登録
            hasSC.ApplyOneTimeToken(dt);


            // Bullet自身を破壊
            DestroyBullet();
        }
    }

    public void DestroyBullet()
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

    private float GetSpeedByTenor(bool isTenor)
    {
        if (isTenor)
        {
            return 5f;
        }
        else
        {
            return 2f;
        }
    }

    private Vector2 GetDirectionByBaritone(bool isBaritone)
    {
        Vector2 direction;

        if (isBaritone)
        {
            if (player == null || isGameOver)
            {
                Debug.Log("バリトンによって方向をPlayerにしました");
                GameObject p = GameObject.FindWithTag("Player");
                if (p == null) return rb.transform.right;
                player = p.transform;
            }

            // playerへの方向
            direction = ((Vector2)player.position - rb.position).normalized;
        }
        else
        {
            direction = rb.transform.right;
        }

        // 角度に変換
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rigidbody2Dで物理的に回転させる
        rb.MoveRotation(angle);

        return direction;
    }


    public void SetIsTenorTrue()
    {
        isTenor = true;
    }

    public void SetIsLeadTrue()
    {
        Debug.Log("IsLeadがTrueになりました。");
        isLead = true;
    }

    public void SetIsBaritoneTrue()
    {
        Debug.Log("バリトンがセットしました");
        isBaritone = true;
    }

    public void SetIsBassTrue()
    {
        isBass = true;
    }

    public bool GetIsLead()
    {
        return isLead;
    }

    public bool GetIsBass()
    {
        return isBass;
    }
}
