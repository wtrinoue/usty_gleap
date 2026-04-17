using UnityEngine;

public class BulletEnemyAttackState : IEnemyState
{
    private BulletEnemyController controller;
    private GameObject bulletPrefab;
    private float shotInterval;
    private float fireDistance;
    private float spawnOffset;
    private Transform transform;
    private Transform player;

    public BulletEnemyAttackState(BulletEnemyController controller, GameObject bulletPrefab, float shotInterval, float fireDistance, float spawnOffset, Transform transform)
    {
        this.controller = controller;
        this.bulletPrefab = bulletPrefab;
        this.shotInterval = shotInterval;
        this.fireDistance = fireDistance;
        this.spawnOffset = spawnOffset;
        this.transform = transform;
    }

    public void Enter()
    {
        player = controller.GetPlayerTransform();
        ShotBullet();
        controller.SetLastShotTime(Time.time);
    }

    public void Update()
    {
        // 攻撃状態の更新処理
        // 攻撃後すぐに移動状態に戻す
        controller.ChangeState(controller.GetMoveState());
    }

    public void Exit()
    {
        // 攻撃状態から出る時の処理
    }

    private void ShotBullet()
    {
        if (bulletPrefab == null) return;

        Vector3 spawnPos = transform.position + transform.right * spawnOffset;
        Object.Instantiate(bulletPrefab, spawnPos, transform.rotation);
    }
}