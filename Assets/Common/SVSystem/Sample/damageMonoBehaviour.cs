using UnityEngine;

public class damageMonoBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 currentPosition = transform.position;
        currentPosition.x -= 0.02f;
        transform.position = currentPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("衝突しました！！");
        StatusContainer mm = collision.gameObject.GetComponent<StatusContainer>();
        if (mm == null) return;
        Debug.Log("ダメージが入りました！");
        DamageToken damageToken = new DamageToken();
        damageToken.ExtractStatus(gameObject.GetComponent<StatusContainer>().GetStatus()); // 自分の補正適用済みのステータスを入れる。
        damageToken.SetAction(() => { });
        mm.ApplyOneTimeToken(damageToken);
    }
}

// ここではぶつかったときにDamageTokenを生成し、ぶつかった相手のModifierManagerに登録し、処理してもらっている。
