using UnityEngine;
public interface IQualtetEnemyRole
{
    public Vector2 GetTargetPoint();
    public void ApplyBulletEffect(NoteBulletBehaviour note);
}