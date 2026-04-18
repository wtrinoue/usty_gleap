using System.Collections;
using UnityEngine;

public interface IQualtetEnemyRole
{
    Vector2 CalculateMoveDirection(Transform self, Transform player);
    void ApplyBulletEffect(NoteBulletBehaviour note);
}

public abstract class QualtetEnemyBehaviour : MonoBehaviour
{
    public GameObject noteBulletCenter;
    public float shotInterval = 0.5f;
    public float effectInterval = 0.5f;
    public float fireDistance = 5f;
    public float effectRadius = 3f;
    public LayerMask targetLayer;
    public float stopDistance = 1f;

    private Transform player;
    private float speed;
    private bool isGameOver = false;
    private StatusManager statusManager;
    private StatusActionHolder statusActionHolder;
    private SelfStatusAction deathAction;
    private Coroutine shotRoutine;
    private Coroutine effectRoutine;
    private IQualtetEnemyRole role;

    protected virtual void Awake()
    {
        role = this as IQualtetEnemyRole;
        if (role == null)
        {
            Debug.LogError($"{GetType().Name} must implement IQualtetEnemyRole.");
        }
    }

    protected virtual void Start()
    {
        statusManager = GetComponent<StatusManager>();
        statusActionHolder = GetComponent<StatusActionHolder>();
        deathAction = statusActionHolder.GetSelfStatusActionFromIndex(0);
        player = GameObject.FindWithTag("Player")?.transform;
        effectRoutine = StartCoroutine(ApplyEffectLoop());
    }

    protected virtual void Update()
    {
        speed = statusManager.GetSpeed();

        if (player == null || isGameOver)
        {
            if (GameObject.FindWithTag("Player") == null) return;
            player = GameObject.FindWithTag("Player").transform;
            return;
        }

        Vector3 direction = role.CalculateMoveDirection(transform, player);
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance > stopDistance)
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        HandleShootingByDistance();
        LookAtPlayer();
        deathAction.Execute(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && false)
        {
            Debug.Log("Game Over!");
            isGameOver = true;

            if (collision.gameObject.TryGetComponent<PlayerController>(out var playerScript))
            {
                playerScript.enabled = false;
            }

            this.enabled = false;
        }

        float currentAttack = statusManager.GetAttackPower();
        Debug.Log("敵の現在の攻撃力: " + currentAttack);
    }

    protected Vector2 GetDirectionToLeadEnemy(LayerMask leadEnemyLayer, float offsetDistance)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            1000f,
            leadEnemyLayer
        );

        if (hits.Length == 0)
            return Vector2.zero;

        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }

        Vector2 targetPoint = (Vector2)nearest.position + (Vector2)nearest.up * offsetDistance;
        return (targetPoint - (Vector2)transform.position).normalized;
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void HandleShootingByDistance()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= fireDistance)
        {
            if (shotRoutine == null)
            {
                shotRoutine = StartCoroutine(ShotLoop());
            }
        }
        else if (shotRoutine != null)
        {
            StopCoroutine(shotRoutine);
            shotRoutine = null;
        }
    }

    private IEnumerator ShotLoop()
    {
        while (true)
        {
            ShotNoteBullet();
            yield return new WaitForSeconds(shotInterval);
        }
    }

    private void ShotNoteBullet()
    {
        float spawnOffset = 0.5f;
        Vector3 spawnPos = transform.position + transform.right * spawnOffset;
        Instantiate(noteBulletCenter, spawnPos, transform.rotation);
    }

    private IEnumerator ApplyEffectLoop()
    {
        while (true)
        {
            ApplyEffectInRadius();
            yield return new WaitForSeconds(effectInterval);
        }
    }

    private void ApplyEffectInRadius()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            effectRadius,
            targetLayer
        );

        foreach (var hit in hits)
        {
            Debug.Log($"[QualtetEnemy] 検出: {hit.name} / Tag:{hit.tag} / Layer:{LayerMask.LayerToName(hit.gameObject.layer)}");
            if (hit.GetComponentInParent<NoteBulletBehaviour>() is { } note)
            {
                role.ApplyBulletEffect(note);
            }
        }
    }
}
