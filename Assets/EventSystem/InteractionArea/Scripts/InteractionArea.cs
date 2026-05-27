using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractionArea : MonoBehaviour, PlayerInputActions.IInteractorActions
{
    [Header("対象インタラクト先（Inspector用）")]
    [SerializeField] private List<MonoBehaviour> interactableObjects = new List<MonoBehaviour>();

    private BoxCollider2D col;
    private SpriteRenderer sd;

    private PlayerInputActions inputActions;

    // Player管理（複数対応）
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
    // Trigger（Playerのみ管理）
    // -----------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playersInRange.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playersInRange.Remove(other.gameObject);
    }

    // -----------------------------
    // Input
    // -----------------------------
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playersInRange.Count == 0) return;

        ExecuteInteraction();
    }

    // -----------------------------
    // 実行処理（Interactable実行）
    // -----------------------------
    public void ExecuteInteraction()
    {
        if (interactableObjects == null || interactableObjects.Count == 0)
        {
            Debug.LogWarning("インタラクト対象が設定されていません");
            return;
        }

        foreach (var obj in interactableObjects)
        {
            if (obj is IInteractable interactable)
            {
                interactable.Active();
            }
        }
    }

    // -----------------------------
    // Gizmo
    // -----------------------------
    private void OnDrawGizmos()
    {
        var box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = new Color(0f, 0.6f, 1f, 0.8f);
        Gizmos.DrawWireCube(box.offset, box.size);

        Gizmos.color = new Color(0f, 0.6f, 1f, 0.15f);
        Gizmos.DrawCube(box.offset, box.size);

        Gizmos.matrix = old;
    }

    private void OnValidate()
    {
        col = GetComponent<BoxCollider2D>();
        if (col != null) col.isTrigger = true;
    }
}