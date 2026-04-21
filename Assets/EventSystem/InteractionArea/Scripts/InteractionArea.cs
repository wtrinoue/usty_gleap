using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class InteractionArea : MonoBehaviour
{
    [SerializeField] private GameObject targetInteractableObject;

    private BoxCollider2D col;
    private SpriteRenderer sd;

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        sd = GetComponent<SpriteRenderer>();

        // Trigger設定
        col.isTrigger = true;
        sd.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Playerが入りました");
        }
        Debug.Log("ぶつかりました");
    }

    public void ExecuteInteraction()
    {
        if (targetInteractableObject == null)
        {
            Debug.LogWarning("targetObject が設定されていません");
            return;
        }

        IInteractable interactable = targetInteractableObject.GetComponent<IInteractable>();

        if (interactable != null)
        {
            interactable.Active();
        }
        else
        {
            Debug.LogWarning("IInteractable を実装したコンポーネントが見つかりません");
        }
    }
}