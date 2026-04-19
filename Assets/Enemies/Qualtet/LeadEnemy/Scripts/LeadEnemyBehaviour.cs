using System.Collections;
using UnityEngine;

public class LeadEnemyBehaviour : QualtetEnemyBehaviour, IQualtetEnemyRole
{
    public Vector2 GetTargetPoint()
    {
        return PlayerManager.Instance.CurrentPlayer.transform.position;
    }

    public void ApplyBulletEffect(NoteBulletBehaviour note)
    {
        note.SetIsLeadTrue();
    }
}
