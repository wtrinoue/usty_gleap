using System.Collections;
using UnityEngine;

public class BaritoneEnemyBehaviour : QualtetEnemyBehaviour, IQualtetEnemyRole
{
    private float offsetDistance = 6f;
    private Transform leadEnemy;
    public Vector2 GetTargetPoint()
    {
        if (leadEnemy == null)
        {
            leadEnemy = transform.parent;
            transform.parent = null;
        }

        float offset = offsetDistance;

        Vector2 targetPoint =
            (Vector2)leadEnemy.position +
            (Vector2)leadEnemy.up * offset;

        return targetPoint;
    }

    public void ApplyBulletEffect(NoteBulletBehaviour note)
    {
        note.SetIsBaritoneTrue();
    }
}
