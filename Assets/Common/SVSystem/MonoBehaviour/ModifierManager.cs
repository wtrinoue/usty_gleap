using System.Collections.Generic;
using UnityEngine;

public class ModifierManager : MonoBehaviour
{
    [Header("StatusMatrix (ステータスの初期値)")]
    public StatusMatrix statusMatrix;
    public StatusVector statusVector { get; private set; }

    public ModifierContainer modifierContainer { get; } = new ModifierContainer();
    private Queue<StatusToken> statusTokens;

    private void Awake()
    {
        statusVector = new StatusVector(statusMatrix);
        ProcessAllTokens();
    }

    private void Update()
    {
        modifierContainer.Update(Time.deltaTime);
        modifierContainer.ApplyEffect(statusVector);
    }

    public StatusVector GetStatus()
    {
        return statusVector.Offset(modifierContainer.CalculateBuff());
    }

    public void AddModifier(Modifier modifier)
    {
        modifierContainer.Add(modifier);
    }

    public void RemoveModifier(Modifier modifier)
    {
        modifierContainer.Remove(modifier);
    }

    public void ApplyEffects()
    {
        modifierContainer.ApplyEffect(statusVector);
    }

    public void AddStatusToken(StatusToken token)
    {
        statusTokens.Enqueue(token);
    }

    public void ProcessAllTokens()
    {
        while (statusTokens.Count > 0)
        {
            StatusToken token = statusTokens.Dequeue();//　キューを用いることで使い終わったら消去
            token.Execute(statusVector, GetStatus());
        }
    }
}

/*
現在のStatusのやり取りは関数の引数にStatusVectorを渡すことで処理をしていたが、そうではなくて、元なるステータスだけを受け取って
*/