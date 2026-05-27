using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class InteractionArea : MonoBehaviour, PlayerInputActions.IInteractorActions
{
    [Header("対象インタラクト先")]
    [SerializeField] private GameObject targetInteractableObject;

    private BoxCollider2D col;
    private SpriteRenderer sd;

    private bool isPlayerInRange = false;

    private PlayerInputActions inputActions;

    // 将来拡張用（複数プレイヤー・NPC対応）
    private HashSet<GameObject> playersInRange = new HashSet<GameObject>();

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Interactor.AddCallbacks(this);

        col = GetComponent<BoxCollider2D>();
        sd = GetComponent<SpriteRenderer>();

        col.isTrigger = true;

        if (sd != null)
            sd.enabled = false;
    }

    void OnEnable()
    {
        inputActions.Interactor.Enable();
    }

    void OnDisable()
    {
        inputActions.Interactor.Disable();
    }

    // -----------------------------
    // Trigger管理
    // -----------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playersInRange.Add(other.gameObject);
        isPlayerInRange = playersInRange.Count > 0;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playersInRange.Remove(other.gameObject);
        isPlayerInRange = playersInRange.Count > 0;
    }

    // -----------------------------
    // Input
    // -----------------------------
    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interactされました！");

        if (isPlayerInRange)
        {
            Debug.Log("ExecuteInteractionされました！");
            ExecuteInteraction();
        }
    }

    // -----------------------------
    // 実行処理
    // -----------------------------
    public void ExecuteInteraction()
    {
        if (targetInteractableObject == null)
        {
            Debug.LogWarning("targetInteractableObject が未設定です");
            return;
        }

        if (targetInteractableObject.TryGetComponent<IInteractable>(out var interactable))
        {
            Debug.Log("IInteractableを発動しました");
            interactable.Active();
        }
        else
        {
            Debug.LogWarning("IInteractable が見つかりません");
        }
    }

    // -----------------------------
    // Gizmo可視化（重要）
    // -----------------------------
    private void OnDrawGizmos()
    {
        var box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        // 範囲表示（青）
        Gizmos.color = new Color(0f, 0.6f, 1f, 0.8f);
        Gizmos.DrawWireCube(box.offset, box.size);

        Gizmos.color = new Color(0f, 0.6f, 1f, 0.15f);
        Gizmos.DrawCube(box.offset, box.size);

        Gizmos.matrix = old;
    }

    // -----------------------------
    // 安全化
    // -----------------------------
    private void OnValidate()
    {
        if (col == null)
            col = GetComponent<BoxCollider2D>();

        if (col != null)
            col.isTrigger = true;
    }
}