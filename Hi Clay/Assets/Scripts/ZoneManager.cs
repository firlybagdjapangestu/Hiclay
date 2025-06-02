using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public enum ZoneType { KillAllEnemies, AutoClear }

    [Header("Zone Settings")]
    public ZoneType zoneType;
    [SerializeField] private int spawnCount;
    [SerializeField] private float spawnDelay = 0.5f;

    [Header("References")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private CinemachinePositionComposer positionComposer;
    [SerializeField] private Transform topBarrier;
    [SerializeField] private Transform bottomBarrier;
    [SerializeField] private Transform leftBarrier;
    [SerializeField] private Transform rightBarrier;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject enemyPrefab;

    private ObjectPoolManager poolManager;

    private int enemiesKilled = 0;
    public bool zoneClear = false;
    private bool playerEntered = false;

    public static bool IsZoneActive = false;

    public int level;

    private void Start()
    {
        poolManager = Object.FindFirstObjectByType<ObjectPoolManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerEntered || zoneClear) return;

        if (collision.CompareTag("Player"))
        {
            playerEntered = true;
            ActivateZone();
        }
    }

    private void Update()
    {
        if (playerEntered && !zoneClear)
        {
            ClampPlayerToZone();
        }
    }

    private void ClampPlayerToZone()
    {
        if (!player || !leftBarrier || !rightBarrier || !topBarrier || !bottomBarrier) return;

        Vector3 pos = player.position;
        float clampedX = Mathf.Clamp(pos.x, leftBarrier.position.x, rightBarrier.position.x);
        float clampedY = Mathf.Clamp(pos.y, bottomBarrier.position.y, topBarrier.position.y);
        player.position = new Vector3(clampedX, clampedY, pos.z);
    }

    private void ActivateZone()
    {
        ChangeCameraTarget(transform);
        IsZoneActive = true;

        if (zoneType == ZoneType.AutoClear)
        {
            MarkZoneCleared();
        }
        else
        {
            enemiesKilled = 0;
            SpawnEnemies();
        }
    }

    private void ChangeCameraTarget(Transform target)
    {
        if (cinemachineCamera)
        {
            cinemachineCamera.LookAt = target;
            cinemachineCamera.Follow = target;
            positionComposer.Composition.DeadZone.Enabled = (target == player);
        }
    }

    public void MarkZoneCleared()
    {
        if (zoneClear) return;

        zoneClear = true;
        IsZoneActive = false;
        ChangeCameraTarget(player);
    }

    public void SpawnEnemies()
    {
        if (!poolManager || !enemyPrefab)
        {
            Debug.LogWarning("Enemy prefab atau PoolManager belum di-set!");
            return;
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            GameObject enemyGO = poolManager.ActiveObject(enemyPrefab, spawnPos, Quaternion.identity);

            if (enemyGO.TryGetComponent(out EnemyStatus enemyStatus))
            {
                enemyStatus.RegisterOnDie(OnEnemyKilled);
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void OnEnemyKilled()
    {
        enemiesKilled++;

        if (enemiesKilled >= spawnCount)
        {
            MarkZoneCleared();
            level++;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        int side = Random.Range(0, 3);
        return side switch
        {
            0 => new Vector3(Random.Range(leftBarrier.position.x, rightBarrier.position.x), topBarrier.position.y, 0f),
            1 => new Vector3(leftBarrier.position.x, Random.Range(bottomBarrier.position.y, topBarrier.position.y), 0f),
            2 => new Vector3(rightBarrier.position.x, Random.Range(bottomBarrier.position.y, topBarrier.position.y), 0f),
            _ => transform.position
        };
    }
}
