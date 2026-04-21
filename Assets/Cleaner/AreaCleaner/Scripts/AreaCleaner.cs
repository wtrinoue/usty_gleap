using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class AreaCleaner : MonoBehaviour, IInteractable
{
    [Header("対象レイヤー")]
    public LayerMask targetLayer;

    [Header("枠線（見た目）")]
    public SpriteRenderer spriteRenderer;

    private BoxCollider2D col;
    private List<GameObject> targets = new List<GameObject>();

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
        spriteRenderer.enabled = false;
    }

    // -----------------------------
    // 範囲管理
    // -----------------------------

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            if (!targets.Contains(other.gameObject))
                targets.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        targets.Remove(other.gameObject);
    }

    // -----------------------------
    // 外部公開API
    // -----------------------------

    /// <summary>
    /// 範囲内のオブジェクトを削除する
    /// </summary>
    public void ClearArea()
    {
        foreach (var obj in targets)
        {
            if (obj != null)
                Destroy(obj);
        }

        targets.Clear();
    }

    /// <summary>
    /// コライダーを無効化（範囲も消す）
    /// </summary>
    public void DisableArea()
    {
        if (col != null)
            col.enabled = false;
    }

    /// <summary>
    /// 完全に非アクティブ化（まとめ処理）
    /// </summary>
    public void Deactivate()
    {
        ClearArea();
        DisableArea();
    }
    public void Active()
    {
        ClearArea();
        DisableArea();
    }
}