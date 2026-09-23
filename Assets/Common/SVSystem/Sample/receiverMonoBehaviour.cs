using UnityEngine;

public class receiverMonoBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DeadToken deadToken = new DeadToken();
        deadToken.SetAction(() => { Debug.Log("やられました！！"); Destroy(gameObject); });
        gameObject.GetComponent<StatusContainer>().ApplyEternalToken(deadToken);
    }
}

// 今回ここではDeadtokenを登録し、ModifierManagerのUpdateでStatusToken.Executeで処理してもらっている。
