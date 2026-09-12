using System.Collections.Generic;
using UnityEngine;

public class ModifierContainer
{
    private readonly Dictionary<ModifierKey, Modifier> buffs = new();
    private readonly Dictionary<ModifierKey, Modifier> effects = new();

    public void Add(Modifier modifier)
    {
        var key = CreateKey(modifier);

        if (modifier.IsBuff)
        {
            if (buffs.ContainsKey(key))
                return;

            buffs.Add(key, modifier);
        }
        else if (modifier.IsEffect)
        {
            if (effects.ContainsKey(key))
                return;

            effects.Add(key, modifier);
        }
    }

    public void Remove(Modifier modifier)
    {
        var key = CreateKey(modifier);

        if (modifier.IsBuff)
        {
            buffs.Remove(key);
        }
        else if (modifier.IsEffect)
        {
            effects.Remove(key);
        }
    }

    public void Update(float deltaTime)
    {
        UpdateDictionary(buffs, deltaTime);
        UpdateDictionary(effects, deltaTime);
    }

    private void UpdateDictionary(Dictionary<ModifierKey, Modifier> dict, float deltaTime)
    {
        var keysToRemove = new List<ModifierKey>();

        foreach (var pair in dict)
        {
            if (pair.Value.Update(deltaTime))
            {
                keysToRemove.Add(pair.Key);
            }
        }

        for (int i = 0; i < keysToRemove.Count; i++)
        {
            dict.Remove(keysToRemove[i]);
        }
    }

    public StatusVector CalculateBuff()
    {
        StatusVector result = new StatusVector();

        foreach (var modifier in buffs.Values)
        {
            if (!modifier.CanInvoke())
                continue;

            modifier.Apply(result);
            modifier.ResetInterval();
        }

        return result;
    }

    public void ApplyEffect(StatusVector status)
    {
        foreach (var modifier in effects.Values)
        {
            if (!modifier.CanInvoke())
                continue;

            modifier.Apply(status);
            modifier.ResetInterval();
        }
    }

    private ModifierKey CreateKey(Modifier modifier)
    {
        return new ModifierKey
        {
            Source = modifier.Source,
            Definition = modifier.Definition
        };
    }
}

public struct ModifierKey
{
    public GameObject Source;
    public ModifierDefinition Definition;

    public override int GetHashCode()
    {
        return Source.GetHashCode() ^ Definition.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is not ModifierKey other) return false;
        return Source == other.Source && Definition == other.Definition;
    }
}