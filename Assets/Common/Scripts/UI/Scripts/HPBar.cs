using Mono.Cecil;
using UnityEngine;
using UnityEngine.UI;



public class HPBar : MonoBehaviour
{

    [SerializeField] private StatusContainer _statusContainer;
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    private bool IsDestroyed = false;

    private void Update()
    {
        // Debug.Log("まだ生きています");
        if (_statusContainer == null || _hpSlider == null) return;
        if (IsDestroyed) return;

        float currentHP = _statusContainer.GetStatus().Get(StatusCategory.HP, StatusMethod.Base);
        float maxHP = _statusContainer.statusMatrix.Get(StatusCategory.HP, StatusMethod.Base);
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
        _statusContainer = target.GetComponentInParent<StatusContainer>(); //親コンポーネントのStatusContainerを取得
    }

}