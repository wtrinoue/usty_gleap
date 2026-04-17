using UnityEngine;

public class BulletEnemyDeadState : IEnemyState
{
    private BulletEnemyController controller;
    private SelfStatusAction deathAction;
    private GameObject gameObject;

    public BulletEnemyDeadState(BulletEnemyController controller, SelfStatusAction deathAction, GameObject gameObject)
    {
        this.controller = controller;
        this.deathAction = deathAction;
        this.gameObject = gameObject;
    }

    public void Enter()
    {
        // 死亡状態に入った時の処理
        if (deathAction != null)
        {
            deathAction.Execute(gameObject);
        }
    }

    public void Update()
    {
        // 死亡状態の更新処理
        // 必要に応じてアニメーションやエフェクト
    }

    public void Exit()
    {
        // 死亡状態から出る時の処理
    }
}