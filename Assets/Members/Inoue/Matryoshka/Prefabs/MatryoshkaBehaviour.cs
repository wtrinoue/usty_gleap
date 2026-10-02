using UnityEngine;
using System.Collections;

public class MatryoshkaBehaviour : MonoBehaviour
{
    private Transform playerTransform;
    public StatusContainer statusContainer;
    private bool isAttacking = false;
    private StatusContainer playerSC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ChasePlayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isAttacking = true;
            playerSC = collision.gameObject.GetComponent<StatusContainer>();
            StartCoroutine(AttackIntervalCoroutine());
        }
    }

    private void OnCollisionExit2D()
    {
        isAttacking = false;
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

    private IEnumerator AttackIntervalCoroutine()
    {
        while (isAttacking && playerSC != null)
        {
            // Perform attack logic here
            yield return new WaitForSeconds(1f); // Adjust the interval as needed
            DamageToken dt = new();
            dt.ExtractStatus(statusContainer.GetStatus());
            playerSC.ApplyOneTimeToken(dt);
        }
    }
}
