using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Status/ModifierDefinition")]
public class ModifierDefinition : ScriptableObject
{
    [Header("種類")]
    public ModifierType type;

    [Header("持続時間")]
    public float duration = 5f;

    [Header("発動間隔")]
    public float interval = 0f;

    [Header("Statusへの補正")]
    public List<StatusModifier> modifiers = new();
}

[Serializable]
public struct StatusModifier
{
    public StatusCategory category;
    public StatusMethod method;
    public float value;
}

public enum ModifierType
{
    Buff,
    Effect
}