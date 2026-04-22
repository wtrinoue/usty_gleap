using UnityEngine;

public class WarpPortal : MonoBehaviour
{
    [SerializeField] private Transform warpPoint;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        collision.transform.position = warpPoint.position;
    }
}