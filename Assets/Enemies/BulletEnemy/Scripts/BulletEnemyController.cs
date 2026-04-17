using UnityEngine;

[RequireComponent(typeof(StatusManager))]
[RequireComponent(typeof(StatusActionHolder))]
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
    private float speed;
    private bool isGameOver = false;
    private StatusManager statusManager;
    private StatusActionHolder statusActionHolder;
    private SelfStatusAction deathAction;
    private float lastShotTime = -999f;

    // 状態管理
    private IEnemyState currentState;
    private BulletEnemyIdleState idleState;
    private BulletEnemyMoveState moveState;
    private BulletEnemyAttackState attackState;
    private BulletEnemyDeadState deadState;

    void Start()
    {
        statusManager = GetComponent<StatusManager>();
        statusActionHolder = GetComponent<StatusActionHolder>();
        if (statusActionHolder != null)
        {
            deathAction = statusActionHolder.GetSelfStatusActionFromIndex(0);
        }
        SetPlayer();

        Debug.Log("BulletEnemyController Start: player = " + player);

        // 各状態を初期化
        InitializeStates();

        // 初期状態を設定
        ChangeState(idleState);
    }

    private void InitializeStates()
    {
        idleState = new BulletEnemyIdleState(this);
        moveState = new BulletEnemyMoveState(this, statusManager, transform, stopDistance);
        attackState = new BulletEnemyAttackState(this, bulletPrefab, shotInterval, fireDistance, spawnOffset, transform);
        deadState = new BulletEnemyDeadState(this, deathAction, gameObject);
    }

    void Update()
    {
        if (statusManager != null)
        {
            speed = statusManager.GetSpeed();
        }

        if (player == null || isGameOver)
        {
            SetPlayer();
            if (player == null) return;
        }

        Debug.Log("BulletEnemy Update: currentState = " + currentState + ", player = " + player + ", speed = " + speed);

        // 死亡状態なら何もしない
        if (currentState is BulletEnemyDeadState)
        {
            currentState.Update();
            return;
        }

        // 状態確認と遷移処理
        CheckDeath();
        CheckMove();
        CheckAttack();

        // 現在の状態の更新処理を実行
        currentState.Update();
    }

    private void CheckMove()
    {
        // 移動可能かつ移動状態以外の場合、移動状態に遷移
        if (!(currentState is BulletEnemyMoveState) && 
            !(currentState is BulletEnemyDeadState) &&
            !(currentState is BulletEnemyAttackState))
        {
            ChangeState(moveState);
        }
    }

    private void CheckDeath()
    {
        if (statusManager.BaseStatus.CurrentHP > 0) return;

        ChangeState(deadState);
    }

    private void CheckAttack()
    {
        if (currentState is BulletEnemyDeadState) return;

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= fireDistance && Time.time - lastShotTime >= shotInterval)
        {
            ChangeState(attackState);
        }
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

    /// <summary>
    /// 状態を変更する
    /// </summary>
    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter();
    }

    /// <summary>
    /// 外部から状態取得用（状態クラスから利用）
    /// </summary>
    public BulletEnemyIdleState GetIdleState() => idleState;
    public BulletEnemyMoveState GetMoveState() => moveState;

    public Transform GetPlayerTransform() => player;
    public void SetPlayerTransform(Transform newPlayer) => player = newPlayer;
    public float GetSpeed() => speed;
    public void SetLastShotTime(float time) => lastShotTime = time;

    private void SetPlayer()
    {
        if (PlayerManager.Instance == null || PlayerManager.Instance.CurrentPlayer == null) 
        {
            Debug.Log("PlayerManager.Instance is null or CurrentPlayer is null");
            return;
        }
        player = PlayerManager.Instance.CurrentPlayer;
        Debug.Log("Player set to: " + player);
    }
}