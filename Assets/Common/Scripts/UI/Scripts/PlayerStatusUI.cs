using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("Status Manager Reference")]
    [SerializeField] private StatusContainer _playerStatusContainer;

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
        if (_playerStatusContainer == null) return;

        // BaseStatusから現在の値を取得
        float currentHP = _playerStatusContainer.GetStatus().Get(StatusCategory.HP, StatusMethod.Base);

        // HPの表示（現在HP / 最大HP）
        float maxHP = _playerStatusContainer.statusMatrix.Get(StatusCategory.HP, StatusMethod.Base); // 初期値がmaxHPとして使用されている
        _hpText.text = $"HP: {currentHP:F0}";

        // 攻撃力の表示（バフ適用後）
        float attackPower = _playerStatusContainer.GetStatus().Calculate(StatusCategory.Attack);
        _attackText.text = $"ATK: {attackPower:F1}";

        // 速度の表示（バフ適用後）
        float speed = _playerStatusContainer.GetStatus().Calculate(StatusCategory.Speed);
        _speedText.text = $"SPD: {speed:F1}";

        // 防御力の表示
        _defenseText.text = $"DEF: {currentHP:F1}";
    }
}
