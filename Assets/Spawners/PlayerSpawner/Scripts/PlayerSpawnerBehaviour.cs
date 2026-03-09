using UnityEngine;

public class PlayerSpawnerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    void Update()
    {
        // Playerタグのオブジェクトが存在しなければ生成
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogWarning("Player Prefab is not assigned!");
            return;
        }
        Instantiate(playerPrefab, transform.position, Quaternion.identity);
    }
}
