using UnityEngine;
using System.Collections.Generic;

public class Modifier
{
    public GameObject Source { get; }

    public ModifierDefinition Definition { get; }

    public ModifierType Type => Definition.type;

    public bool IsBuff => Type == ModifierType.Buff;

    public bool IsEffect => Type == ModifierType.Effect;

    public float RemainingTime { get; private set; }

    public float IntervalTimer { get; private set; }

    public Modifier(ModifierDefinition definition, GameObject source)
    {
        Definition = definition;
        Source = source;
        RemainingTime = definition.duration;
        IntervalTimer = definition.interval;
    }

    public bool Update(float deltaTime)
    {
        RemainingTime -= deltaTime;

        if (Definition.interval > 0f)
        {
            IntervalTimer -= deltaTime;
        }

        return RemainingTime <= 0f;
    }

    public bool CanInvoke()
    {
        return Definition.interval == 0f || IntervalTimer <= 0f;
    }

    public void ResetInterval()
    {
        IntervalTimer = Definition.interval;
    }

    public List<StatusModifier> GetModifiers()
    {
        return Definition.modifiers;
    }

    public void Apply(StatusVector statusVector)
    {
        foreach (var modifier in Definition.modifiers)
        {
            statusVector.Add(
                modifier.category,
                modifier.method,
                modifier.value
            );
        }
    }
}