using UnityEngine;

/// <summary>
/// 敵の状態インターフェース
/// </summary>
public interface IEnemyState
{
    void Enter();
    void Update();
    void Exit();
}
