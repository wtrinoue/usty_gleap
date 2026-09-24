using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private StatusContainer statusContainer;
    private float speed;

    private Vector2 moveInput;

    // ���͒l��n��
    public void SetMoveInput(Vector2 input) => moveInput = input;

    void Start()
    {
        statusContainer = GetComponent<StatusContainer>();
    }

    // Update����Ă�
    public void TickMove()
    {
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0f);
        speed = statusContainer.GetStatus().Calculate(StatusCategory.Speed);
        transform.position += movement * speed * Time.deltaTime;
    }
}
