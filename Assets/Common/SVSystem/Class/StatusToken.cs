using System.Diagnostics;

public abstract class StatusToken
{
    private StatusVector source;

    public void SetSource(StatusVector s)
    {
        source = new StatusVector(s);
    }

    public StatusVector GetSource()
    {
        return source;
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