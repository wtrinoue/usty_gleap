using UnityEngine;
public class Damage : ComunicationRule
{
    public void Calculate(in StatusVector source, in StatusVector target)
    {
        float sourceAttack = source.Calculate(StatusCategory.Attack);
        float targetDefence = target.Calculate(StatusCategory.Defense);
        float damage = sourceAttack - targetDefence;
        if (damage < 0) { damage = 0; }
        source.Add(StatusCategory.HP, StatusMethod.Base, -damage);
    }
}
