using UnityEngine;

public class OrbitEnemyBehaviour : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float stopDistance = 1f;

    [Header("Orbit (optional)")]
    [SerializeField] private OrbitCenterBehaviour orbitCenter;
    [SerializeField] private GameObject orbitCenterPrefab;

    private Transform player;
    private float speed;
    private bool isGameOver = false;
    private StatusManager statusManager;
    private StatusActionHolder statusActionHolder;
    private SelfStatusAction deathAction;

    void Awake()
    {
        if (orbitCenter == null)
        {
            orbitCenter = GetComponentInChildren<OrbitCenterBehaviour>();
        }

        if (orbitCenter == null && orbitCenterPrefab != null)
        {
            GameObject instance = Instantiate(orbitCenterPrefab, transform.position, Quaternion.identity, transform);
            orbitCenter = instance.GetComponent<OrbitCenterBehaviour>();
        }

        if (orbitCenter != null)
        {
            orbitCenter.SetupCenter(transform);
        }
    }

    void Start()
    {
        statusManager = GetComponent<StatusManager>();
        statusActionHolder = GetComponent<StatusActionHolder>();
        if (statusActionHolder != null)
        {
            deathAction = statusActionHolder.GetSelfStatusActionFromIndex(0);
        }
        SetPlayer();
    }

    void Update()
    {
        if (statusManager != null)
        {
            speed = statusManager.GetSpeed();
        }

        if (player == null || isGameOver)
        {
            SetPlayer();
            if (player == null) return;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance > stopDistance)
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        if (deathAction != null)
        {
            deathAction.Execute(this.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && false)
        {
            Debug.Log("Game Over!");
            isGameOver = true;

            if (collision.gameObject.TryGetComponent<PlayerController>(out var playerScript))
            {
                playerScript.enabled = false;
            }

            this.enabled = false;
        }
    }

    private void SetPlayer()
    {
        if (PlayerManager.Instance == null || PlayerManager.Instance.CurrentPlayer == null) return;
        player = PlayerManager.Instance.CurrentPlayer;
    }
}
