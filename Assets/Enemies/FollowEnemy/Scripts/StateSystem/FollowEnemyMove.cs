using UnityEngine;

/// <summary>
/// プレイヤーに向かって移動する純粋な移動コンポーネント
/// </summary>
public class FollowEnemyMove : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float stopDistance = 1.5f;

    private Transform playerTarget;

    /// <summary>
    /// プレイヤーに向かって移動する（条件を満たさない場合は何もしない）
    /// </summary>
    public void Move()
    {
        // プレイヤーがいなければ移動処理をスキップ
        if (!TryGetPlayer(out playerTarget))
        {
            return;
        }

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = playerTarget.position;

        // ターゲットとの距離を計算
        float distance = Vector3.Distance(targetPosition, currentPosition);

        // 停止距離より離れている場合のみ移動を実行
        if (distance > stopDistance)
        {
            Vector3 direction = (targetPosition - currentPosition).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// プレイヤーのTransformの取得とキャッシュを行う
    /// </summary>
    private bool TryGetPlayer(out Transform target)
    {
        if (playerTarget != null)
        {
            target = playerTarget;
            return true;
        }

        if (PlayerManager.Instance == null || PlayerManager.Instance.CurrentPlayer == null)
        {
            target = null;
            return false;
        }

        playerTarget = PlayerManager.Instance.CurrentPlayer;
        target = playerTarget;
        return true;
    }
}