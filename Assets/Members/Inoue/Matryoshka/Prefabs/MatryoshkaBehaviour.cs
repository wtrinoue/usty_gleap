using UnityEngine;
using System.Collections;

public class MatryoshkaBehaviour : MonoBehaviour
{
    public StatusContainer statusContainer;
    public GameObject childPrefab;
    private Transform playerTransform;
    private bool isAttacking = false;
    private StatusContainer playerSC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DeadToken deadToken = new();
        deadToken.SetAction(() =>
        {
            Instantiate(childPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        });
        statusContainer.ApplyEternalToken(deadToken);
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
            DamageToken dt = new();
            dt.ExtractStatus(statusContainer.GetStatus());
            playerSC.ApplyOneTimeToken(dt);
            yield return new WaitForSeconds(1f); // Adjust the interval as needed
        }
    }
}
