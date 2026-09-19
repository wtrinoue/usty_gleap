using UnityEngine;

public class BulletCoreBehaviour : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float spawnDistance = 0.5f;

    void Start()
    {
        SpawnBullet();
        Invoke(nameof(DestroyMe), 0.5f);
    }

    public void SpawnBullet()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet Prefab is not assigned!");
            return;
        }

        // 自分の前方向（2D）
        Vector3 forward = transform.up;

        // 180度反転した方向 = 後ろ
        Vector3 spawnDir = -forward;

        // 出現位置
        Vector3 spawnPos = transform.position + spawnDir * spawnDistance;

        // 生成（回転は仮でidentity）
        GameObject bullet = Instantiate(
            bulletPrefab,
            spawnPos,
            transform.rotation * Quaternion.Euler(0f, 0f, -90f)
        );

        // 向き：自分 → bullet（外向き）
        Vector3 outwardDir = (spawnPos - transform.position).normalized;

        // 2Dなら up を合わせる
        bullet.transform.up = outwardDir;
        // 3Dなら下の行を使う
        // bullet.transform.forward = outwardDir;
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
