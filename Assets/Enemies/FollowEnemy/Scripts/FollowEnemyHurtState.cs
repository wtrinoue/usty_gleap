using UnityEngine;

/// <summary>
/// 敵がダメージを受けた状態
/// </summary>
public class FollowEnemyHurtState : IEnemyState
{
    private readonly FollowEnemyController controller;
    private readonly FollowEnemyAnimation animation;
    private readonly StatusManager statusManager;

    private float hurtDuration = 0.5f;
    private float hurtTimer = 0f;

    public FollowEnemyHurtState(
        FollowEnemyController controller,
        FollowEnemyAnimation animation,
        StatusManager statusManager)
    {
        this.controller = controller;
        this.animation = animation;
        this.statusManager = statusManager;
    }

    public void Enter()
    {
        animation.Hurt();
        hurtTimer = 0f;
    }

    public void Update()
    {
        hurtTimer += Time.deltaTime;

        // ダメージアニメーション完了後、移動状態に戻る
        if (hurtTimer >= hurtDuration)
        {
            controller.ChangeState(controller.GetMoveState());
        }
    }

    public void Exit()
    {
    }
}
