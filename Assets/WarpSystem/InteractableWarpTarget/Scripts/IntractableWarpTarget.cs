using UnityEngine;

public class IntractableWarpTarget : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Active()
    {
        Transform player = PlayerManager.Instance.CurrentPlayer;

        if (player == null)
        {
            Debug.LogWarning("Playerが見つかりません");
            return;
        }

        // ワープ実行
        player.position = transform.position;

        Debug.Log("ワープしました");
    }
}
