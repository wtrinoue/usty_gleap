using System.Collections.Generic;
using UnityEngine;

public class ModifierContainer
{
    private readonly Dictionary<ModifierKey, Modifier> buffs = new();
    private readonly Dictionary<ModifierKey, Modifier> effects = new();
    private readonly StatusVector buffResult = new StatusVector();

    public void Add(Modifier modifier)
    {
        var key = CreateKey(modifier);

        if (modifier.IsBuff)
        {
            // もし同じkeyを持っていたら除外（Buffは排他的である）
            if (buffs.ContainsKey(key))
                return;

            buffs.Add(key, modifier);
        }
        else if (modifier.IsEffect)
        {
            // もし同じkeyを持っていたら除外（Effectは排他的である）
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


    // Modifierの更新メソッド（寿命が来たら削除する）
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

    // Buffを計算して返すところ（Buffは一時的なものなのでbuffResultにまとめている。インターバルのリセットも担う。）
    public StatusVector CalculateBuff()
    {
        buffResult.InitializeDefaults();

        foreach (var modifier in buffs.Values)
        {
            if (!modifier.CanInvoke())
                continue;

            modifier.Apply(buffResult);
            modifier.ResetInterval();
        }

        return buffResult;
    }

    // EffectをStatusに適応するところ（Effectは永続効果なのでStatusを直接変更する。インターバルのリセットも担う。）
    public void ApplyEffect(in StatusVector status)
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

// 「どのようなパラメータを変更するか」と「どこのオブジェクトからか」の情報を詰め込む構造体
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