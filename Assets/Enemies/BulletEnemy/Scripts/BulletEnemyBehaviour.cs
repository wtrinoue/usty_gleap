using UnityEngine;

public class BulletEnemyBehaviour : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float stopDistance = 1f;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shotInterval = 1f;
    [SerializeField] private float fireDistance = 5f;
    [SerializeField] private float spawnOffset = 0.5f;

    private Transform player;
    private bool isGameOver = false;
    private StatusManager statusManager;
    private StatusActionHolder statusActionHolder;
    private SelfStatusAction deathAction;
    private float lastShotTime = -999f;

    private IEnemyState currentState;
    private BulletEnemyIdleState idleState;
    private BulletEnemyCombatState combatState;
    private BulletEnemyDeadState deadState;

    void Start()
    {
        statusManager = GetComponent<StatusManager>();
        statusActionHolder = GetComponent<StatusActionHolder>();
        if (statusActionHolder != null)
        {
            deathAction = statusActionHolder.GetSelfStatusActionFromIndex(0);
        }

        InitializeStates();
        ChangeState(idleState);
    }

    void Update()
    {
        ExecuteDeathAction();

        if (currentState is BulletEnemyDeadState)
        {
            currentState.Update();
            return;
        }

        CheckDeath();
        currentState.Update();
    }

    void OnCollisionEnter2D(Collision2D collision)
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
    }

    private void InitializeStates()
    {
        idleState = new BulletEnemyIdleState(this);
        combatState = new BulletEnemyCombatState(this);
        deadState = new BulletEnemyDeadState();
    }

    private void CheckDeath()
    {
        if (currentState is BulletEnemyDeadState) return;
        if (statusManager == null || statusManager.BaseStatus == null) return;
        if (statusManager.BaseStatus.CurrentHP > 0f) return;

        ChangeState(deadState);
    }

    private void ExecuteDeathAction()
    {
        if (deathAction == null) return;
        deathAction.Execute(this.gameObject);
    }

    public bool TryGetPlayer(out Transform target)
    {
        target = player;

        if (target != null && !isGameOver) return true;

        if (PlayerManager.Instance == null || PlayerManager.Instance.CurrentPlayer == null)
        {
            target = null;
            player = null;
            return false;
        }

        player = PlayerManager.Instance.CurrentPlayer;
        target = player;
        return true;
    }

    public void MoveTowards(Transform target)
    {
        if (target == null || statusManager == null) return;

        float speed = statusManager.GetSpeed();
        Vector3 direction = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance > stopDistance)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    public void LookAt(Transform target)
    {
        if (target == null) return;

        Vector2 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void TryShootAt(Transform target)
    {
        if (target == null) return;

        float dist = Vector2.Distance(transform.position, target.position);

        if (dist <= fireDistance && Time.time - lastShotTime >= shotInterval)
        {
            ShootBullet();
            lastShotTime = Time.time;
        }
    }

    private void ShootBullet()
    {
        if (bulletPrefab == null) return;

        Vector3 spawnPos = transform.position + transform.right * spawnOffset;
        Instantiate(bulletPrefab, spawnPos, transform.rotation);
    }

    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter();
    }

    public BulletEnemyCombatState GetCombatState() => combatState;
    public BulletEnemyIdleState GetIdleState() => idleState;
}
