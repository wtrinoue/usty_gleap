using UnityEngine;

/// <summary>
/// 敵が死亡状態
/// </summary>
public class FollowEnemyDeadState : IEnemyState
{
    private readonly FollowEnemyAnimation animation;
    private readonly GameObject gameObject;
    private readonly float destroyDelay = 2f;
    private float deadTimer = 0f;

    public FollowEnemyDeadState(FollowEnemyAnimation animation, GameObject gameObject)
    {
        this.animation = animation;
        this.gameObject = gameObject;
    }

    public void Enter()
    {
        animation.Death();
        deadTimer = 0f;
    }

    public void Update()
    {
        deadTimer += Time.deltaTime;

        if (deadTimer >= destroyDelay)
        {
            Object.Destroy(gameObject);
        }
    }

    public void Exit()
    {
    }
}
