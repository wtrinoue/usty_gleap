using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class WaveTrigger : MonoBehaviour
{
    [Header("Set Waves")]
    [SerializeField] private Wave[] waves;
    [Header("Set Target Tag")]
    [SerializeField] private string targetTag;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D col;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        spriteRenderer.enabled = false;
        col.isTrigger = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            Debug.Log("Playerが入りました");
            GenerateAllWaves();
        }
    }

    private void GenerateAllWaves()
    {
        foreach (var wave in waves)
        {
            Instantiate(wave);
        }
    }
}
