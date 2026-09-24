using UnityEngine;

public class BladeBehaviour : MonoBehaviour
{
    [SerializeField] private StatusContainer statusContainer;
    [SerializeField] private Knockback knockback;
    // 当たったオブジェクトを検出
    private void OnTriggerEnter2D(Collider2D other)
    {
        knockback.DoKnockback(other.gameObject);
        StatusContainer otherSC = other.gameObject.GetComponent<StatusContainer>();
        if (otherSC == null) return;
        DamageToken dt = new();
        dt.ExtractStatus(statusContainer.GetStatus());
        otherSC.ApplyOneTimeToken(dt);
    }

}
