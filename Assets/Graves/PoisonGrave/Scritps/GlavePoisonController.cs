using UnityEngine;

public class gravePoisonController : MonoBehaviour
{
    [Header("Effect Area Settings")]
    [SerializeField] private float effectRadius = 2f;
    [SerializeField] private ModifierDefinition modifierDefinition;

    private GameObject effectArea;

    void Start()
    {
        CreateEffectArea();
    }

    private void CreateEffectArea()
    {
        // エフェクトエリア用のGameObjectを作成
        effectArea = new GameObject("PoisonEffectArea");
        effectArea.transform.SetParent(transform);
        effectArea.transform.localPosition = Vector3.zero;

        // CircleCollider2Dを追加（トリガーとして設定）
        CircleCollider2D circleCollider = effectArea.AddComponent<CircleCollider2D>();
        circleCollider.radius = effectRadius;
        circleCollider.isTrigger = true;

        // AreaEffectTriggerコンポーネントを追加
        AreaEffectTrigger trigger = effectArea.AddComponent<AreaEffectTrigger>();

        // poisonEffectをAreaEffectTriggerに設定するために、Reflectionを使用
        if (modifierDefinition != null)
        {
            var field = typeof(AreaEffectTrigger).GetField("modifierDefinition",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(trigger, modifierDefinition);
            }
        }

        Debug.Log($"graveの周りに半径{effectRadius}のPoisonEffectAreaを作成しました");
    }

    // Gizmoで範囲を可視化（エディタ上で確認用）
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, effectRadius);
    }
}
