using Unity.VisualScripting;
using UnityEngine;

public class EffectItemBehaviour : MonoBehaviour
{
    [Header("Effect Setting")]
    [SerializeField] private Effect[] effects;
    private StatusActionHolder statusActionHolder;
    private EffectAction effectAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statusActionHolder = GetComponent<StatusActionHolder>();
        effectAction = statusActionHolder.GetTargetStatusActionFromIndex(0) as EffectAction;
        effectAction.SetEffects(effects);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Playerタグのときだけ処理
        if (!other.CompareTag("Player")) return;

        Debug.Log("Playerに当たりました: " + other.name);

        // 例：ステータスアクションを適用
        if (effectAction == null) return;
        effectAction.Execute(gameObject, other.gameObject);
        Destroy(gameObject);
    }
}
