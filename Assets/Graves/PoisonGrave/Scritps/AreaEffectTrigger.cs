using UnityEngine;

public class AreaEffectTrigger : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField] private ModifierDefinition modifierDefinition;
    [SerializeField] private string targetTag = "Enemy";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // タグが一致しない場合は処理をスキップ
        if (!other.CompareTag(targetTag)) return;

        // StatusManagerを持つオブジェクトにエフェクトを適用
        StatusContainer statusContainer = other.GetComponent<StatusContainer>();
        if (statusContainer != null && modifierDefinition != null)
        {
            // エフェクトのコピーを作成して追加
            Modifier modifier = new Modifier(modifierDefinition, gameObject);
            statusContainer.AddModifier(modifier);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // エリアから出た時の処理が必要な場合はここに記述
        Debug.Log($"{other.name}がエフェクトエリアから出ました");
    }
}
