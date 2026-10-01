using UnityEngine;
using System.Collections.Generic;

public class RandomItemGenerator : MonoBehaviour
{
    public List<PrefabSet> items;
    private int allWeight = 1;
    private int selectedNumber = 0;

    void Start()
    {
        foreach (PrefabSet i in items)
        {
            allWeight += i.weight;
        }
    }

    public GameObject GetRandom()
    {
        selectedNumber = Random.Range(1, allWeight);
        foreach (PrefabSet i in items)
        {
            if (i.weight <= 0) continue;
            selectedNumber -= i.weight;
            if (selectedNumber <= 0)
            {
                return i.prefab;
            }
        }
        return items[0].prefab;
    }
}

[System.Serializable]
public class PrefabSet
{
    public int weight;
    public GameObject prefab;
}