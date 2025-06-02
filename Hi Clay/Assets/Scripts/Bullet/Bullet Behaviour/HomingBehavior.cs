using UnityEngine;

public class HomingBehavior : IBulletBehavior
{
    private Transform target;

    public HomingBehavior()
    {
        // Target akan dicari saat UpdateBehavior pertama kali
    }

    public void UpdateBehavior(BulletController bullet)
    {
        if (target == null)
        {
            target = FindClosestEnemy(bullet.transform.position);
            if (target == null)
            {
                // Kalau gak ada target, peluru tetap maju lurus
                bullet.transform.position += bullet.transform.right * bullet.Speed * Time.deltaTime;
                return;
            }
        }

        // Arah ke target
        Vector2 dir = (target.position - bullet.transform.position).normalized;

        // Interpolasi rotasi ke arah target
        float rotateSpeed = 200f;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        bullet.transform.rotation = Quaternion.RotateTowards(bullet.transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        // Maju ke depan
        bullet.transform.position += bullet.transform.right * bullet.Speed * Time.deltaTime;
    }

    private Transform FindClosestEnemy(Vector2 position)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDist = Mathf.Infinity;
        Transform closest = null;

        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy.transform;
            }
        }

        return closest;
    }
}
