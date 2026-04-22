using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class InteractionArea : MonoBehaviour, PlayerInputActions.IInteractorActions
{
    [SerializeField] private GameObject targetInteractableObject;

    private BoxCollider2D col;
    private SpriteRenderer sd;
    private bool isPlayerInRange = false;
    private PlayerInputActions inputActions;

    void Awake()
    {

        inputActions = new PlayerInputActions();
        inputActions.Interactor.AddCallbacks(this);

        col = GetComponent<BoxCollider2D>();
        sd = GetComponent<SpriteRenderer>();

        // Trigger設定
        col.isTrigger = true;
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

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
        Debug.Log("ぶつかりました");
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
        Debug.Log("ぶつかりました");
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (isPlayerInRange)
        {
            ExecuteInteraction();
        }
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