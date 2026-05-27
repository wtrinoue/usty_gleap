using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class AreaCleaner : MonoBehaviour, IInteractable
{
    [Header("Tag判定（優先）")]
    [SerializeField] private string targetTag = "Enemy";

    [Header("Layer判定（必要なら併用）")]
    [SerializeField] private LayerMask targetLayer;

    [Header("見た目")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private BoxCollider2D col;

    private HashSet<GameObject> targets = new HashSet<GameObject>();

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    // -----------------------------
    // 判定ロジック（Tag + Layer両対応）
    // -----------------------------
    private bool IsTarget(GameObject obj)
    {
        // Tag判定（メイン）
        if (!string.IsNullOrEmpty(targetTag))
        {
            if (obj.CompareTag(targetTag))
                return true;
        }

        // Layer判定（サブ）
        if (targetLayer.value != 0)
        {
            int objLayer = obj.layer;
            if ((targetLayer.value & (1 << objLayer)) != 0)
                return true;
        }

        return false;
    }

    // -----------------------------
    // Trigger
    // -----------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsTarget(other.gameObject))
        {
            Debug.Log("消すためのオブジェクトを追加しました");
            targets.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        targets.Remove(other.gameObject);
    }

    // -----------------------------
    // 外部API
    // -----------------------------
    public void ClearArea()
    {
        Debug.Log("削除開始");

        var copy = new List<GameObject>(targets);
        foreach (var obj in copy)
        {
            if (obj != null)
                Destroy(obj);
        }

        targets.Clear();
    }

    public void Active()
    {
        ClearArea();
    }

    // -----------------------------
    // Gizmo可視化
    // -----------------------------
    private void OnDrawGizmos()
    {
        var box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = new Color(1f, 0f, 0f, 0.8f);
        Gizmos.DrawWireCube(box.offset, box.size);

        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Gizmos.DrawCube(box.offset, box.size);

        Gizmos.matrix = old;
    }
}