using UnityEngine;

public class BulletEnemyMoveState : IEnemyState
{
    private BulletEnemyController controller;
    private StatusManager statusManager;
    private Transform transform;
    private float stopDistance;
    private Transform player;

    public BulletEnemyMoveState(BulletEnemyController controller, StatusManager statusManager, Transform transform, float stopDistance)
    {
        this.controller = controller;
        this.statusManager = statusManager;
        this.transform = transform;
        this.stopDistance = stopDistance;
    }

    public void Enter()
    {
        player = controller.GetPlayerTransform();
    }

    public void Update()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance > stopDistance)
        {
            float speed = controller.GetSpeed();
            transform.position += direction * speed * Time.deltaTime;
        }

        LookAtPlayer();
    }

    public void Exit()
    {
        // 移動状態から出る時の処理
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}