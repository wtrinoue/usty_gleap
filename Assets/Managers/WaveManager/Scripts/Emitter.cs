using UnityEngine;
using System.Collections;

public class Emitter : MonoBehaviour
{
    public GameObject[] waves;

    public int limitRemainingEnemies;

    public bool allWaveFinish = false;
    private int currentWave = 0;

    private int interval = 1;

    void Start()
    {
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        while (true)
        {
            InstantiateEnemy();
            yield return new WaitForSeconds(interval);
        }
    }

    void InstantiateEnemy()
    {
        if(currentWave == waves.Length){
            allWaveFinish = true;
            return;
        }
        int count = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if(count <= limitRemainingEnemies)
        {
            Instantiate(waves[currentWave]);
            currentWave += 1;
        }
    }
}
