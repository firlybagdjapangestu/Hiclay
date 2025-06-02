using UnityEngine;

public class StraightBehavior : IBulletBehavior
{
    public void UpdateBehavior(BulletController bullet)
    {
        bullet.transform.position += (Vector3)(bullet.MoveDirection * bullet.Speed * Time.deltaTime);
    }
}
