using UnityEngine;

public class BulletEnemyIdleState : IEnemyState
{
    private readonly BulletEnemyBehaviour controller;

    public BulletEnemyIdleState(BulletEnemyBehaviour controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
    }

    public void Update()
    {
        if (controller.TryGetPlayer(out _))
        {
            controller.ChangeState(controller.GetCombatState());
        }
    }

    public void Exit()
    {
    }
}
