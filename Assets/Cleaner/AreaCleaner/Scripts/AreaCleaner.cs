using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class AreaCleaner : MonoBehaviour, IInteractable
{
    [Header("対象レイヤー")]
    public LayerMask targetLayer;

    [Header("見た目（任意）")]
    public SpriteRenderer spriteRenderer;

    private BoxCollider2D col;

    // List → HashSet（重複防止＆高速化）
    private HashSet<GameObject> targets = new HashSet<GameObject>();

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    // -----------------------------
    // Trigger管理
    // -----------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsInLayerMask(other.gameObject.layer, targetLayer))
        {
            targets.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        targets.Remove(other.gameObject);
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    // -----------------------------
    // 外部API
    // -----------------------------
    public void ClearArea()
    {
        foreach (var obj in targets)
        {
            if (obj != null)
                Destroy(obj);
        }

        targets.Clear();
    }

    public void DisableArea()
    {
        if (col != null)
            col.enabled = false;
    }

    public void ActivateArea()
    {
        if (col != null)
            col.enabled = true;
    }

    public void Deactivate()
    {
        ClearArea();
        DisableArea();
    }

    public void Active()
    {
        ActivateArea();
    }

    // -----------------------------
    // Sceneビュー可視化（重要）
    // -----------------------------
    private void OnDrawGizmos()
    {
        var box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        // ワイヤー（枠）
        Gizmos.color = new Color(1f, 0f, 0f, 0.8f);

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(box.offset, box.size);

        // 半透明面
        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Gizmos.DrawCube(box.offset, box.size);

        Gizmos.matrix = oldMatrix;
    }

    // Inspector変更時の安全性
    private void OnValidate()
    {
        if (col == null)
            col = GetComponent<BoxCollider2D>();

        if (col != null)
            col.isTrigger = true;
    }
}