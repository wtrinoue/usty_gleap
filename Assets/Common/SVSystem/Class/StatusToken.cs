using System;
using System.Diagnostics;
using NUnit.Framework.Internal.Filters;

public abstract class StatusToken
{
    public Action action = () => { };
    public abstract void ExtractStatus(in StatusVector s);

    public void SetAction(Action a)
    {
        action = a;
    }

    public void Execute(in StatusVector target, StatusVector modified)
    {
        modified.Validate();
        Calculate(target, modified);
    }
    abstract public void Calculate(in StatusVector target, StatusVector modified);
}

// 数値を指定し、targetのStatusのパラメータに加算する特殊なToken
public class CustomAddToken : StatusToken
{
    private StatusCategory category = StatusCategory.HP;
    private StatusMethod method = StatusMethod.Base;
    private float value = 0f;

    public override void ExtractStatus(in StatusVector s) { }

    public CustomAddToken(StatusCategory c, StatusMethod m, float v)
    {
        category = c;
        method = m;
        value = v;
    }
    public override void Calculate(in StatusVector target, StatusVector modified)
    {
        target.Add(category, method, value);
    }
}

public class DamageToken : StatusToken
{
    float sourceAttack = 0f;
    public override void ExtractStatus(in StatusVector s)
    {
        sourceAttack = s.Calculate(StatusCategory.Attack);
    }
    public override void Calculate(in StatusVector target, StatusVector modified)
    {
        float targetDefense = modified.Calculate(StatusCategory.Defense);
        float damage = sourceAttack - targetDefense;
        if (damage < 0) { damage = 0; }
        target.Add(StatusCategory.HP, StatusMethod.Base, -damage);
    }
}

public class DeadToken : StatusToken
{
    public override void ExtractStatus(in StatusVector s) { }
    public override void Calculate(in StatusVector target, StatusVector modified)
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