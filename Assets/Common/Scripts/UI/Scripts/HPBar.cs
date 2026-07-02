using Mono.Cecil;
using UnityEngine;
using UnityEngine.UI;



public class HPBar : MonoBehaviour
{
    [SerializeField] private StatusManager _statusManager;
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    private bool IsDestroyed = false;

    private void Update()
    {
        // Debug.Log("まだ生きています");
        if (_statusManager == null || _hpSlider == null) return;
        if (IsDestroyed) return;

        BaseStatus status = _statusManager.BaseStatus;
        float currentHP = status.CurrentHP;
        float maxHP = status.MaxHP;
        if (currentHP <= 0)
        {
            Destroy(gameObject);
            IsDestroyed = true;
        }

        // MaxHPが0の場合は0を設定
        _hpSlider.value = maxHP > 0 ? currentHP / maxHP : 0;

        // HPバーの向きを固定
        // transform.LookAt(Camera.main.transform);
        transform.rotation = Camera.main.transform.rotation;
        transform.position = target.position + offset;
        //transform.Rotate(0, 180, 0);
    }
    public void SetTarget(Transform t)
    {
        target = t;
    }

    public void SetOffset(float height)
    {
        offset = new Vector3(0f, height, -3f);
    }

    public void Initialize()
    {
        _statusManager = target.GetComponentInParent<StatusManager>(); //親コンポーネントのstatusManagerを取得
    }

}