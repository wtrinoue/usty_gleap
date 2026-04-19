using System.Collections;
using UnityEngine;

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
        shotRoutine = StartCoroutine(ShotLoop());
    }

    protected virtual void Update()
    {
        Move();
        LookAtPlayer();
        deathAction.Execute(this.gameObject);
    }

    private void OnDestroy()
    {
        StopCoroutine(shotRoutine);
    }

    private void Move()
    {
        speed = statusManager.GetSpeed();
        Vector2 targetPoint = role.GetTargetPoint();
        Vector3 direction = CalculateMoveDirection(transform.position, targetPoint);
        float distance = Vector2.Distance(player.position, transform.position);

        if (distance > stopDistance)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private Vector2 CalculateMoveDirection(Vector2 selfPoint, Vector2 targetPoint)
    {
        return (targetPoint - selfPoint).normalized;
    }
    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private bool CanShootPlayer()
    {
        if (player == null) return false;

        float dist = Vector2.Distance(transform.position, player.position);
        return dist <= fireDistance;
    }

    private IEnumerator ShotLoop()
    {
        while (true)
        {
            if (CanShootPlayer())
            {
                ShotNoteBullet();
            }
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
