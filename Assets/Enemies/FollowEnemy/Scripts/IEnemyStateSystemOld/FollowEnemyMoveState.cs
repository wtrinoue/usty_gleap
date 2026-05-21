using UnityEngine;

/// <summary>
/// 敵がプレイヤーに向かって移動する状態
/// </summary>
public class FollowEnemyMoveState : IEnemyState
{
    private readonly FollowEnemyController controller;
    private readonly StatusManager statusManager;
    private readonly FollowEnemyAnimation animation;
    private readonly Transform transform;
    private readonly float stopDistance;

    private Transform playerTarget;

    public FollowEnemyMoveState(
        FollowEnemyController controller,
        StatusManager statusManager,
        FollowEnemyAnimation animation,
        Transform transform,
        float stopDistance)
    {
        this.controller = controller;
        this.statusManager = statusManager;
        this.animation = animation;
        this.transform = transform;
        this.stopDistance = stopDistance;
    }

    public void Enter()
    {
        animation.Run();
    }

    public void Update()
    {
        if (!TryGetPlayer(out playerTarget))
        {
            controller.ChangeState(controller.GetIdleState());
            return;
        }

        Vector3 direction = (playerTarget.position - transform.position).normalized;
        float distance = Vector3.Distance(playerTarget.position, transform.position);

        if (distance > stopDistance)
        {
            float speed = statusManager.GetSpeed();
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    public void Exit()
    {
        animation.Idle();
    }

    private bool TryGetPlayer(out Transform target)
    {
        target = controller.GetPlayerTransform();

        if (target != null) return true;

        if (PlayerManager.Instance == null ||
            PlayerManager.Instance.CurrentPlayer == null)
        {
            return false;
        }

        target = PlayerManager.Instance.CurrentPlayer;
        controller.SetPlayerTransform(target);
        return true;
    }
}
