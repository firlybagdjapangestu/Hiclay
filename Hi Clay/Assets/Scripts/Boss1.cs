using UnityEngine;
using System.Collections;

public class Boss1 : MonoBehaviour
{
    [SerializeField] private Transform spawnPositionShootgun;
    [SerializeField] private Transform spawnPositionRocketLauncher;

    [SerializeField] private BulletData Shootgun;
    [SerializeField] private BulletData RocketLauncher;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private ObjectPoolManager objectPoolManager;

    private Coroutine shootCoroutine;

    private void OnEnable()
    {
        // Mulai tembakan otomatis saat Boss aktif
        shootCoroutine = StartCoroutine(ShootGunLoop());
    }

    private void OnDisable()
    {
        // Hentikan tembakan saat Boss tidak aktif
        if (shootCoroutine != null)
            StopCoroutine(shootCoroutine);
    }

    private IEnumerator ShootGunLoop()
    {
        while (true)
        {
            ShootGun();
            yield return new WaitForSeconds(Shootgun.fireRate);
        }
    }

    public void ShootRocketLauncher()
    {
        // Implementasikan logika serangan roket di sini jika diperlukan
    }

    public void ShootGun()
    {
        GameObject bulletGO = objectPoolManager.ActiveObject(bulletPrefab, spawnPositionShootgun.position, Quaternion.identity);
        bulletGO.layer = LayerMask.NameToLayer("Enemy");
        BulletController bullet = bulletGO.GetComponent<BulletController>();
        if (bullet != null)
        {
            bullet.ApplyBulletData(Shootgun);
            bullet.SetDirection(spawnPositionShootgun.transform.right);
            bullet.Activate();
        }
    }
}
