using UnityEngine;
using UnityEngine.Rendering.Universal;
[RequireComponent(typeof(StatusManager))]
public class HpBarGenerator : MonoBehaviour
{

    public GameObject hpBarPrefab;
    public float height = 1f;
    private HPBar hpBar;

    void Start()
    {
        GameObject hpBarObject = Instantiate(hpBarPrefab);
        hpBar = hpBarObject.GetComponent<HPBar>();
        hpBar.SetTarget(transform);
        hpBar.SetOffset(height);
        hpBar.Initialize();
    }
}