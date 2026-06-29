using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(StatusManager))]

public class HPBar : MonoBehaviour
{
    [SerializeField] private StatusManager _statusManager;
    [SerializeField] private Slider _hpSlider;
    
    private void Awake()
    {
        _statusManager = transform.parent.GetComponentInParent<StatusManager>(); //親コンポーネントのstatusManagerを取得

        // キャラの頭上（Y軸で上にずらす）に配置
        //transform.localPosition = new Vector3(0, 1f, 0); // 2.5は例（キャラの身長に合わせて調整）
    }

    private void Update()
    {
        if (_statusManager == null || _hpSlider == null) return;
        
        BaseStatus status = _statusManager.BaseStatus;
        float currentHP = status.CurrentHP;
        float maxHP = status.MaxHP;
        
        // MaxHPが0の場合は0を設定
        _hpSlider.value = maxHP > 0 ? currentHP / maxHP : 0;

        // HPバーの向きを固定
        // transform.LookAt(Camera.main.transform);
         transform.rotation = Camera.main.transform.rotation;
        //transform.Rotate(0, 180, 0);
    }

}