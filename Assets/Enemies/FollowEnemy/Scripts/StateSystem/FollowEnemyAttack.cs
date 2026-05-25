using UnityEngine;

/// <summary>
/// Executes an attack when this enemy collides with the player.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class FollowEnemyAttack : MonoBehaviour
{
    [Header("Player tag")]
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (attackAction == null) return;
        if (!collision.gameObject.CompareTag(playerTag)) return;

        attackAction.Execute(gameObject, collision.gameObject);
    }
}
