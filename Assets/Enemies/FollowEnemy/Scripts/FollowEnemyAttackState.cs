using UnityEngine;

/// <summary>
/// 敵が攻撃している状態
/// </summary>
public class FollowEnemyAttackState : IEnemyState
{
    private readonly FollowEnemyController controller;
    private readonly FollowEnemyAnimation animation;
    private readonly StatusActionHolder statusActionHolder;

    private TargetStatusAction attackAction;
    private GameObject targetObject;
    private float attackDuration = 0.5f;
    private float attackTimer = 0f;

    public FollowEnemyAttackState(
        FollowEnemyController controller,
        FollowEnemyAnimation animation,
        StatusActionHolder statusActionHolder)
    {
        this.controller = controller;
        this.animation = animation;
        this.statusActionHolder = statusActionHolder;
        attackAction = statusActionHolder.GetTargetStatusActionFromIndex(0);
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
                attackAction.Execute(controller.gameObject, targetObject);
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
