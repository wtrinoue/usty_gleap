using UnityEngine;

public class BulletEnemyIdleState : IEnemyState
{
    private BulletEnemyController controller;

    public BulletEnemyIdleState(BulletEnemyController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        // Idle状態に入った時の処理
    }

    public void Update()
    {
        // Idle状態の更新処理
        // 必要に応じて他の状態に遷移
    }

    public void Exit()
    {
        // Idle状態から出る時の処理
    }
}