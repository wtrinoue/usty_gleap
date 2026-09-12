using UnityEngine;

public class ModifierManager : MonoBehaviour
{
    public StatusVector Status { get; private set; }

    public ModifierContainer Modifiers { get; } = new ModifierContainer();

    private void Awake()
    {
        Status = new StatusVector();
    }

    private void Update()
    {
        Modifiers.Update(Time.deltaTime);
        Modifiers.ApplyEffect(Status);
    }

    public StatusVector GetStatus()
    {
        return Status.Offset(Modifiers.CalculateBuff());
    }

    public void AddModzifier(Modifier modifier)
    {
        Modifiers.Add(modifier);
    }

    public void RemoveModifier(Modifier modifier)
    {
        Modifiers.Remove(modifier);
    }

    public void ApplyEffects()
    {
        Modifiers.ApplyEffect(Status);
    }
}