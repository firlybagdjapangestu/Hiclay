using UnityEngine;

[CreateAssetMenu(menuName = "EnemyBehaviour/Attack/Default Shoot")]
public class EnemyDefaultShoot : BaseAttackData
{
    public GameObject bulletPrefab;

    public override void Attack(
        Transform origin,
        Transform startBulletPosition,
        Transform target,
        BulletData bulletData = null,
        Transform aimTransform = null
    )
    {
        if (origin == null || target == null || bulletData == null || bulletPrefab == null)
            return;

        // Dapatkan ObjectPoolManager dari scene
        ObjectPoolManager objectPoolManager = Object.FindAnyObjectByType<ObjectPoolManager>();
        if (objectPoolManager == null)
        {
            Debug.LogWarning("ObjectPoolManager not found in scene.");
            return;
        }

        Vector2 shootDirection = (target.position - startBulletPosition.position).normalized;

        if (aimTransform != null)
        {
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
            aimTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        GameObject bulletGO = objectPoolManager.ActiveObject(bulletPrefab, startBulletPosition.position, Quaternion.identity);
        bulletGO.layer = LayerMask.NameToLayer("Enemy");
        BulletController bullet = bulletGO.GetComponent<BulletController>();
        if (bullet != null)
        {
            bullet.ApplyBulletData(bulletData);
            bullet.SetDirection(startBulletPosition.transform.right);
            bullet.Activate();
        }
    }
}
