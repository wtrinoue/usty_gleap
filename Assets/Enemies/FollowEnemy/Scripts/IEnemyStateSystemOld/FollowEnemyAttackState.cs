using UnityEngine;

/// <summary>
/// 敵が攻撃している状態
/// </summary>
public class FollowEnemyAttackState : IEnemyState
{
    private readonly FollowEnemyController controller;
    private readonly FollowEnemyAnimation animation;
    private readonly StatusContainer statusContainer;
    private GameObject targetObject;
    private float attackDuration = 0.5f;
    private float attackTimer = 0f;

    public FollowEnemyAttackState(
        FollowEnemyController controller,
        FollowEnemyAnimation animation,
        StatusContainer statusContainer)
    {
        this.controller = controller;
        this.animation = animation;
        this.statusContainer = statusContainer;
    }

    public void Enter()
    {
        animation.Attack();
        attackTimer = 0f;
    }

    public void Update()
    {
        attackTimer += Time.deltaTime;

        // 攻撃アニメーション完了後、移動状態に戻る
        if (attackTimer >= attackDuration)
        {
            if (targetObject != null)
            {
                DamageToken damageToken = new();
                damageToken.ExtractStatus(statusContainer.GetStatus());
                targetObject.GetComponent<StatusContainer>().ApplyOneTimeToken(damageToken);
            }
            controller.ChangeState(controller.GetMoveState());
        }
    }

    public void Exit()
    {
    }

    public void SetTargetObject(GameObject target)
    {
        targetObject = target;
    }
}
