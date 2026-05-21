using UnityEngine;

/// <summary>
/// コリジョンでプレイヤーを検知したときに自動で攻撃を実行するコンポーネント
/// 受動的なイベント駆動の部分はStateには含めないのがきれいな設計であるらしい
/// </summary>
[RequireComponent(typeof(Collider))] // 当たり判定が必要なため必須とする
public class FollowEnemyAttack : MonoBehaviour
{
    [Header("判定するプレイヤーのタグ")]
    [SerializeField] private string playerTag = "Player";

    private StatusActionHolder statusActionHolder;
    private TargetStatusAction attackAction;

    private void Awake()
    {
        statusActionHolder = GetComponent<StatusActionHolder>();
        if (statusActionHolder != null)
        {
            attackAction = statusActionHolder.GetTargetStatusActionFromIndex(0);
        }
    }

    /// <summary>
    /// トリガー（コライダー）に何かが侵入したときのUnity固有イベント
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 攻撃アクションがセットされていない場合は何もしない
        if (attackAction == null) return;

        // 接触した相手のタグが「Player」かどうかをチェック
        if (other.CompareTag(playerTag))
        {
            // 相手（プレイヤー）の GameObject をターゲットとして攻撃を実行
            attackAction.Execute(gameObject, other.gameObject);
        }
    }

    // ※もし「トリガー（Is Trigger）」ではなく「物理的な衝突」を使う場合は、
    // 以下の OnCollisionEnter を使用してください。
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (attackAction == null) return;

        if (collision.gameObject.CompareTag(playerTag))
        {
            attackAction.Execute(gameObject, collision.gameObject);
        }
    }
    */
}