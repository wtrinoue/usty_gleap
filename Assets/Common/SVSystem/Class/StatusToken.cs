using System;
using System.Diagnostics;

public abstract class StatusToken
{
    private StatusVector source = new();// ここに渡し手のステータスを入れる。
    public Action action = () => { };
    public void SetSource(StatusVector s)
    {
        source = new StatusVector(s);
    }

    public StatusVector GetSource()
    {
        return source;
    }

    public void SetAction(Action a)
    {
        action = a;
    }

    abstract public void Execute(in StatusVector target, StatusVector modified);
}

public class DamageToken : StatusToken
{
    public override void Execute(in StatusVector target, StatusVector modified)
    {
        float sourceAttack = GetSource().Calculate(StatusCategory.Attack);
        float targetDefense = modified.Calculate(StatusCategory.Defense);
        float damage = sourceAttack - targetDefense;
        if (damage < 0) { damage = 0; }
        target.Add(StatusCategory.HP, StatusMethod.Base, -damage);
    }
}

public class DeadToken : StatusToken
{
    public override void Execute(in StatusVector target, StatusVector modified)
    {
        UnityEngine.Debug.Log("ダメージ計算中！！");
        float myHP = target.Get(StatusCategory.HP, StatusMethod.Base);
        UnityEngine.Debug.Log(myHP);
        if (myHP <= 0)
        {
            action.Invoke();// ここにやられた時の処理を入れておく。
        }
    }
}