using UnityEngine;

public class BulletEnemyCombatState : IEnemyState
{
    private readonly BulletEnemyBehaviour controller;

    public BulletEnemyCombatState(BulletEnemyBehaviour controller)
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
