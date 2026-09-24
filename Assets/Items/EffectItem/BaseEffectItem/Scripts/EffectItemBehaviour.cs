using Unity.VisualScripting;
using UnityEngine;

public class EffectItemBehaviour : MonoBehaviour
{
    [Header("Effect Setting")]
    [SerializeField] private ModifierDefinition modifierDefinition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Playerタグのときだけ処理
        if (!other.CompareTag("Player")) return;

        Debug.Log("Playerに当たりました: " + other.name);

        // 例：ステータスアクションを適用
        if (modifierDefinition == null) return;
        StatusContainer playerSC = GetComponent<StatusContainer>();
        playerSC.AddModifier(new Modifier(modifierDefinition, gameObject));
        Destroy(gameObject);
    }
}
