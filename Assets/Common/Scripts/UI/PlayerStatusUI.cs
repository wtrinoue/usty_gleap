using UnityEngine;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("Status Manager Reference")]
    [SerializeField] private StatusManager _playerStatusManager;

    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _attackText;
    [SerializeField] private TextMeshProUGUI _speedText;
    [SerializeField] private TextMeshProUGUI _defenseText;

    private void Update()
    {
        UpdateStatusDisplay();
    }

    private void UpdateStatusDisplay()
    {
        if (_playerStatusManager == null) return;

        // BaseStatusから現在の値を取得
        BaseStatus baseStatus = _playerStatusManager.BaseStatus;
        
        // HPの表示（現在HP / 最大HP）
        float maxHP = _playerStatusManager.BaseStatus.CurrentHP; // 初期値がmaxHPとして使用されている
        _hpText.text = $"HP: {baseStatus.CurrentHP:F0}";

        // 攻撃力の表示（バフ適用後）
        float attackPower = _playerStatusManager.GetAttackPower();
        _attackText.text = $"ATK: {attackPower:F1}";

        // 速度の表示（バフ適用後）
        float speed = _playerStatusManager.GetSpeed();
        _speedText.text = $"SPD: {speed:F1}";

        // 防御力の表示
        _defenseText.text = $"DEF: {baseStatus.BaseDefense:F1}";
    }
}
