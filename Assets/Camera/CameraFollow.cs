using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;

    [SerializeField] private float followSpeed = 5f;

    void Update()
    {
        // playerが未設定または再生成された場合
        if (player == null)
        {
            player = PlayerManager.Instance.CurrentPlayer;
            if (player == null) return;

            // カメラをプレイヤー中心に即合わせる
            transform.position = new Vector3(
                player.position.x,
                player.position.y,
                transform.position.z
            );

            return;
        }

        // 遅れて追従
        Vector3 targetPosition = new Vector3(
            player.position.x,
            player.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }
}