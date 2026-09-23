using UnityEngine;

public class BulletEnemyController : MonoBehaviour
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
    private StatusContainer statusContainer;
    private float lastShotTime = -999f;

    private IEnemyState currentState;
    private BulletEnemyIdleState idleState;
    private BulletEnemyCombatState combatState;
    private BulletEnemyDeadState deadState;

    protected virtual void Start()
    {
        statusContainer = GetComponent<StatusContainer>();

        InitializeStates();
        DeadToken dt = new DeadToken();
        dt.SetAction(() => { ChangeState(deadState); });
        statusContainer.ApplyEternalToken(dt);
        ChangeState(idleState);
    }

    protected virtual void Update()
    {
        if (currentState is BulletEnemyDeadState)
        {
            currentState.Update();
            return;
        }
        currentState.Update();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
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
        if (target == null || statusContainer == null) return;

        float speed = statusContainer.GetStatus().Calculate(StatusCategory.Speed);
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