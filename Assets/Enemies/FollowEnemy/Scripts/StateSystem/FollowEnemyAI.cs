using UnityEngine;

/// <summary>
/// Builds FollowEnemy states and provides the next state to StateMachine.
/// AIの中で、純粋なvoidで引数なしのメソッドにしてからStateの関数軍として入れるのはあり
/// </summary>
[RequireComponent(typeof(FollowEnemyMove))]
[RequireComponent(typeof(FollowEnemyAnimation))]
[RequireComponent(typeof(StatusManager))]
[RequireComponent(typeof(StatusActionHolder))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class FollowEnemyAI : MonoBehaviour, IStateProvider
{
    [SerializeField] private FollowEnemyMove moveComponent;
    [SerializeField] private FollowEnemyAnimation animationComponent;
    [SerializeField] private StatusManager statusManager;
    [SerializeField] private StatusActionHolder statusActionHolder;
    [SerializeField] private float stopDistance = 1f;
    [SerializeField] private string playerTag = "Player";

    private readonly StateMachine stateMachine = new();

    private State idleState;
    private State moveState;
    private State deadState;
    private State hurtState;
    private State attackState;

    private Transform player;
    private GameObject attackTarget;
    private float pastHp;
    private float hurtTimer;
    private float attackTimer;
    private float deadTimer;
    private bool hasHpCache;
    private FolloEnemyState loggedState;
    private bool hasLoggedState;

    private const float HurtDuration = 0.5f;
    private const float AttackDuration = 0.5f;
    private const float DestroyDelay = 2f;

    public FolloEnemyState currentState = FolloEnemyState.idle;

    public enum FolloEnemyState
    {
        idle,
        move,
        dead,
        hurt,
        attack
    }

    private void Awake()
    {
        if (moveComponent == null)
        {
            moveComponent = GetComponent<FollowEnemyMove>();
        }

        if (animationComponent == null)
        {
            animationComponent = GetComponent<FollowEnemyAnimation>();
        }

        if (statusManager == null)
        {
            statusManager = GetComponent<StatusManager>();
        }

        if (statusActionHolder == null)
        {
            statusActionHolder = GetComponent<StatusActionHolder>();
        }

        idleState = new StateBuilder()
            .Enter(() => animationComponent?.Idle())
            .Build();

        moveState = new StateBuilder()
            .Enter(() => animationComponent?.Run())
            .Update(MoveToPlayer)
            .Exit(() => animationComponent?.Idle())
            .Build();

        deadState = new StateBuilder()
            .Enter(() =>
            {
                animationComponent?.Death();
                deadTimer = 0f;
            })
            .Update(UpdateDead)
            .Build();

        hurtState = new StateBuilder()
            .Enter(() =>
            {
                animationComponent?.Hurt();
                hurtTimer = 0f;
            })
            .Update(UpdateHurt)
            .Build();

        attackState = new StateBuilder()
            .Enter(() =>
            {
                animationComponent?.Attack();
                attackTimer = 0f;
            })
            .Update(UpdateAttack)
            .Build();
    }

    private void Update()
    {
        stateMachine.Update(this, Time.deltaTime);
    }

    public State ProvideState()
    {
        InitializeHpCache();

        if (currentState == FolloEnemyState.dead)
        {
            return GetState(currentState);
        }

        CheckDeath();
        CheckHurt();
        CheckMove();
        LogStateIfChanged();

        return GetState(currentState);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        ChangeAttackState(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(playerTag)) return;

        ChangeAttackState(collision.gameObject);
    }

    private void ChangeAttackState(GameObject target)
    {
        if (currentState == FolloEnemyState.dead) return;

        attackTarget = target;
        currentState = FolloEnemyState.attack;
        LogStateIfChanged();
    }

    private void CheckDeath()
    {
        if (statusManager == null || statusManager.BaseStatus == null) return;
        if (statusManager.BaseStatus.CurrentHP > 0f) return;

        currentState = FolloEnemyState.dead;
    }

    private void CheckHurt()
    {
        if (statusManager == null || statusManager.BaseStatus == null) return;

        float currentHp = statusManager.BaseStatus.CurrentHP;
        if (currentHp >= pastHp) return;

        pastHp = currentHp;
        currentState = FolloEnemyState.hurt;
    }

    private void CheckMove()
    {
        if (currentState == FolloEnemyState.dead) return;
        if (currentState == FolloEnemyState.attack) return;
        if (currentState == FolloEnemyState.hurt) return;

        if (!TryGetPlayer(out player))
        {
            currentState = FolloEnemyState.idle;
            return;
        }

        float distance = Vector3.Distance(player.position, transform.position);
        currentState = distance > stopDistance
            ? FolloEnemyState.move
            : FolloEnemyState.idle;
    }

    private State GetState(FolloEnemyState state)
    {
        switch (state)
        {
            case FolloEnemyState.idle:
                return idleState;
            case FolloEnemyState.move:
                return moveState;
            case FolloEnemyState.dead:
                return deadState;
            case FolloEnemyState.hurt:
                return hurtState;
            case FolloEnemyState.attack:
                return attackState;
            default:
                return idleState;
        }
    }

    private void MoveToPlayer()
    {
        if (!TryGetPlayer(out player))
        {
            currentState = FolloEnemyState.idle;
            return;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance > stopDistance)
        {
            animationComponent?.Run();
            float speed = statusManager != null ? statusManager.GetSpeed() : 0f;
            transform.position += direction * speed * Time.deltaTime;
            return;
        }

        currentState = FolloEnemyState.idle;
    }

    private void UpdateHurt()
    {
        hurtTimer += Time.deltaTime;

        if (hurtTimer >= HurtDuration)
        {
            currentState = FolloEnemyState.move;
        }
    }

    private void UpdateAttack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer < AttackDuration) return;

        if (attackTarget != null && statusActionHolder != null)
        {
            TargetStatusAction attackAction = statusActionHolder.GetTargetStatusActionFromIndex(0);
            attackAction?.Execute(gameObject, attackTarget);
        }

        currentState = FolloEnemyState.move;
    }

    private void UpdateDead()
    {
        deadTimer += Time.deltaTime;

        if (deadTimer >= DestroyDelay)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeHpCache()
    {
        if (hasHpCache) return;
        if (statusManager == null || statusManager.BaseStatus == null) return;

        pastHp = statusManager.BaseStatus.CurrentHP;
        hasHpCache = true;
    }

    private bool TryGetPlayer(out Transform target)
    {
        target = player;

        if (target != null) return true;

        if (PlayerManager.Instance == null ||
            PlayerManager.Instance.CurrentPlayer == null)
        {
            return false;
        }

        player = PlayerManager.Instance.CurrentPlayer;
        target = player;
        return true;
    }

    private void LogStateIfChanged()
    {
        if (hasLoggedState && loggedState == currentState) return;

        loggedState = currentState;
        hasLoggedState = true;
        Debug.Log($"{nameof(FollowEnemyAI)} State: {currentState}", this);
    }
}
