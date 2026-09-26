using System.Runtime.CompilerServices;
using UnityEngine;

public class gravePowerBuffController : MonoBehaviour
{
    [Header("Buff Area Settings")]
    [SerializeField] private float buffRadius = 2f;
    [SerializeField] private ModifierDefinition modifierDefinition;

    private GameObject buffArea;

    void Start()
    {
        CreateBuffArea();
    }

    private void CreateBuffArea()
    {
        // バフエリア用のGameObjectを作成
        buffArea = new GameObject("PowerBuffArea");
        buffArea.transform.SetParent(transform);
        buffArea.transform.localPosition = Vector3.zero;

        // CircleCollider2Dを追加（トリガーとして設定）
        CircleCollider2D circleCollider = buffArea.AddComponent<CircleCollider2D>();
        circleCollider.radius = buffRadius;
        circleCollider.isTrigger = true;

        // AreaBuffTriggerコンポーネントを追加
        AreaBuffTrigger trigger = buffArea.AddComponent<AreaBuffTrigger>();

        // powerBuffをAreaBuffTriggerに設定するために、Reflectionを使用
        if (modifierDefinition != null)
        {
            var field = typeof(AreaBuffTrigger).GetField("modifierDefinition",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(trigger, modifierDefinition);
            }
        }

        Debug.Log($"graveの周りに半径{buffRadius}のPowerBuffAreaを作成しました");
    }

    // Gizmoで範囲を可視化（エディタ上で確認用）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // オレンジ色
        Gizmos.DrawSphere(transform.position, buffRadius);
    }
}
