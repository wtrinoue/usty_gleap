using UnityEngine;

public class ModifierManager : MonoBehaviour
{
    [Header("StatusMatrix (ステータスの初期値)")]
    public StatusMatrix statusMatrix;
    public StatusVector statusVector { get; private set; }

    public ModifierContainer modifierContainer { get; } = new ModifierContainer();

    private void Awake()
    {
        statusVector = new StatusVector(statusMatrix);
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
}

/*
現在のStatusのやり取りは関数の引数にStatusVectorを渡すことで処理をしていたが、そうではなくて、元なるステータスだけを受け取って
*/