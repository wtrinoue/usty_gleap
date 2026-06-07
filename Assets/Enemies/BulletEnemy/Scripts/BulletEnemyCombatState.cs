using UnityEngine;

public class BulletEnemyCombatState : IEnemyState
{
    private readonly BulletEnemyController controller;

    public BulletEnemyCombatState(BulletEnemyController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
    }

    public void Update()
    {
        if (!controller.TryGetPlayer(out Transform target))
        {
            controller.ChangeState(controller.GetIdleState());
            return;
        }

        controller.MoveTowards(target);
        controller.LookAt(target);
        controller.TryShootAt(target);
    }

    public void Exit()
    {
    }
}
