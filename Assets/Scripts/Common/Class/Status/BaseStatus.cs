public class BaseStatus
{
    //属性
    public float MaxHP {get; private set;}
    public float CurrentHP {get; private set;}
    public float BaseAttack {get; private set;}
    public float BaseSpeed {get; private set;}
    public float BaseDefense {get; private set;}

    //セットメソッド
    public void SetCurrentHP(float value)
    {
        CurrentHP = value;
    }

    public void SetBaseAttack(float value)
    {
        BaseAttack = value;
    }

    public void SetBaseSpeed(float value)
    {
        BaseSpeed = value;
    }

    public void SetBaseDefense(float value)
    {
        BaseDefense = value;
    }

    //コンストラクタ
    public  BaseStatus(BaseStatusSO baseStatusSO)
    {
        MaxHP = baseStatusSO.MaxHP;
        CurrentHP = baseStatusSO.MaxHP;
        BaseAttack = baseStatusSO.BaseAttack;
        BaseSpeed = baseStatusSO.BaseSpeed;
        BaseDefense = baseStatusSO.BaseDefense;
    }
}

//コンストラクタをデフォルト値にすることで、インスタンス作成時に引数を不要にする。
//statusを追加するには属性、セットメソッド、コンストラクタの初期値の三つを追加すること