using System;
using UnityEngine;

public class EnemyStatus : MonoBehaviour, IDamageable
{
    [Header("Enemy Data")]
    public EnemyData enemyData;

    private string enemyID;
    private string enemyName;
    private string enemyDescription;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer enemyHead;
    [SerializeField] private SpriteRenderer enemyBody;
    [SerializeField] private SpriteRenderer enemyLegs;
    [SerializeField] private SpriteRenderer enemyWeapon;

    [Header("Sound Effects")]
    public SoundEventChannelSO sfxChannel;
    public AudioClip attackClip;
    public AudioClip dieClip;
    public AudioClip hitClip;

    [Header("Stats")]
    public float health;
    public float fireRate;

    private ObjectPoolManager poolManager;

    public event Action onEnemyDie;

    private void Start()
    {
        poolManager = FindAnyObjectByType<ObjectPoolManager>();
        SetupFromData();
    }

    private void OnEnable()
    {
        SetupFromData();
    }

    private void OnDisable()
    {
        onEnemyDie = null; // prevent memory leak and duplicate invoke
    }

    private void SetupFromData()
    {
        if (enemyData == null)
        {
            Debug.LogError("EnemyData not assigned on " + gameObject.name);
            return;
        }

        enemyID = enemyData.enemyID;
        enemyName = enemyData.enemyName;
        enemyDescription = enemyData.enemyDescription;

        health = enemyData.health;
        fireRate = enemyData.bulletData.fireRate;

        if (enemyHead) enemyHead.sprite = enemyData.enemyHead;
        if (enemyBody) enemyBody.sprite = enemyData.enemyBody;
        if (enemyLegs) enemyLegs.sprite = enemyData.enemyLegs;
        if (enemyWeapon) enemyWeapon.sprite = enemyData.enemyWeapon;

        attackClip = enemyData.attackClip;
        dieClip = enemyData.dieClip;
        hitClip = enemyData.hitClip;
    }

    public void RegisterOnDie(Action callback)
    {
        onEnemyDie -= callback; // mencegah dobel
        onEnemyDie += callback;
    }

    public void UnregisterOnDie(Action callback)
    {
        onEnemyDie -= callback;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        GameEvents.Hit();
        sfxChannel.RaiseEvent(hitClip);

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        sfxChannel.RaiseEvent(dieClip);

        onEnemyDie?.Invoke(); // invoke dulu sebelum di-deactivate
        onEnemyDie = null;    // optional tambahan biar aman

        poolManager.DeactivateObject(gameObject);
    }

}
