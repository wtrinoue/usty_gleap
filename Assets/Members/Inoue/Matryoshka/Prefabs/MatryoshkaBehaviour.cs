using UnityEngine;

public class MatryoshkaBehaviour : MonoBehaviour
{
    private Transform playerTransform;
    public StatusContainer statusContainer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        if (playerTransform == null)
        {
            playerTransform = PlayerManager.Instance.CurrentPlayer;
            return;
        }

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * statusContainer.GetStatus().Calculate(StatusCategory.Speed) * Time.deltaTime;
    }
}
