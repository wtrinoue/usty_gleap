using UnityEngine;

public class SinNoteBulletBehaviour : MonoBehaviour
{
    public float amplitude = 0.5f;   // 振動の大きさ
    public float frequency = 5f;     // 振動スピード
    private StatusActionHolder statusActionHolder;
    private TargetStatusAction attackAction;
    private NoteBulletBehaviour parent;

    private Vector3 baseLocalPos;
    private Vector3 baseScale;

    void Start()
    {
        // status関連を取得
        statusActionHolder = GetComponent<StatusActionHolder>();
        attackAction = statusActionHolder.GetTargetStatusActionFromIndex(0);
        // 親を核とした基準位置を保存
        baseLocalPos = transform.localPosition;
        // 最初の大きさを保存
        baseScale = new Vector3(0.5f, 0.5f, 0.5f);
        parent = GetComponentInParent<NoteBulletBehaviour>();
    }

    void Update()
    {
        SinMoveByLead(parent.GetIsLead());
        ScaleUpByBass(parent.GetIsBass());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //プレイヤーとぶつかったら
        if (collision.gameObject.CompareTag("Player"))
        {
            attackAction.Execute(gameObject, collision.gameObject);
            parent.DestroyBullet();
        }
    }

    private void SinMoveByLead(bool isLead)
    {
        if (isLead)
        {
            float sine = Mathf.Sin(Time.time * frequency);

            // 親の周囲でローカル方向に振動
            transform.localPosition =
                baseLocalPos + Vector3.up * sine * amplitude;
        }
        else
        {
            // 親の周囲でローカル方向に振動
            transform.localPosition =
                baseLocalPos;
        }
    }

    private void ScaleUpByBass(bool isBass)
    {
        if (isBass)
        {
            transform.localScale = baseScale * 1.5f;
        }
        else
        {
            transform.localScale = baseScale;
        }
    }
}
