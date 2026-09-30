using UnityEngine;

/// <summary>
/// Executes an attack when this enemy collides with the player.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class FollowEnemyAttack : MonoBehaviour
{
    [Header("Player tag")]
    [SerializeField] private string playerTag = "Player";

    public StatusContainer statusContainer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(playerTag)) return;

        StatusContainer playerSC = collision.gameObject.GetComponent<StatusContainer>();

        if (playerSC == null) return;

        Debug.Log("FollowEnemyからPlayerに攻撃をしました。");

        DamageToken dt = new();
        dt.ExtractStatus(statusContainer.GetStatus());
        playerSC.ApplyOneTimeToken(dt);
    }
}
