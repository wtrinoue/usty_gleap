using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterRegister", menuName = "ScriptableObjects/CharacterRegister")]
public class CharacterRegister : ScriptableObject
{
    public List<UstyObjectData> objects;

}

[System.Serializable]
public class UstyObjectData
{
    public UstyCategory category;
    public GameObject prefab;
}

public enum UstyCategory
{
    Enemy,
    WeaponItem,
    GraveItem,
    ModifierItem,
}



