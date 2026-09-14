using System;
using System.Diagnostics;

public abstract class StatusToken
{
    private StatusVector source;// ここに渡し手のステータスを入れる。
    public Action action;
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
        float targetDefence = modified.Calculate(StatusCategory.Defense);
        float damage = sourceAttack - targetDefence;
        if (damage < 0) { damage = 0; }
        target.Add(StatusCategory.HP, StatusMethod.Base, -damage);
    }
}

public class DeadToken : StatusToken
{
    public override void Execute(in StatusVector target, StatusVector modified)
    {
        float myHP = target.Get(StatusCategory.HP, StatusMethod.Base);
        if (myHP <= 0)
        {
            action.Invoke();// ここにやられた時の処理を入れておく。
        }
    }
}