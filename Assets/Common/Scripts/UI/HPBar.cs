using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private StatusManager _statusManager;
    [SerializeField] private Slider _hpSlider;
    
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
        transform.Rotate(0, 180, 0);
    }

}