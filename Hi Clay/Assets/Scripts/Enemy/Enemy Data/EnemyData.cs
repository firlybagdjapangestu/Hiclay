using UnityEngine;

[CreateAssetMenu(menuName = "New Enemy/New Enemy")]
public class EnemyData : ScriptableObject
{

    [Header("Enemy Stats")]
    public string enemyID;
    public string enemyName;
    [TextArea] public string enemyDescription;
    public Sprite enemyHead;
    public Sprite enemyBody;
    public Sprite enemyLegs;
    public Sprite enemyWeapon;
    public float health;

    [Header("Range Settings")]
    public float detectRange;
    public float attackRange;
    public float retreatRange;

    [Header("Bullet")]
    public BulletData bulletData; // Reference to the bullet data for the enemy

    [Header("Sound Effects")]
    public AudioClip hitClip; // Sound effect for when the enemy is hit
    public AudioClip attackClip; // Sound effect for when the enemy attacks
    public AudioClip dieClip; // Sound effect for when the enemy dies


    [Header("Behaviour Enemy")]
    public BaseIdleData idleData; 
    public BasePatrolData patrolData; 
    public BaseChaseData chaseData; 
    public BaseAttackData attackData; 
    public BaseDieData dieData; 
}

public abstract class BaseIdleData : ScriptableObject
{
    public abstract void Initialize(Transform origin);
    public abstract void Idle();
}

public abstract class BasePatrolData : ScriptableObject
{
    public abstract void Initialize(Transform origin);
    public abstract void Patrol();
}

public abstract class BaseChaseData : ScriptableObject
{
    public abstract void Chase(Transform target, Transform player);
}

public abstract class BaseDieData : ScriptableObject
{
    public abstract void Initialize(Transform origin);
    public abstract void Die();
}


public abstract class BaseAttackData : ScriptableObject
{
    public abstract void Attack(
        Transform origin,
        Transform startBulletPosition,
        Transform target,
        BulletData bulletData = null,
        Transform aimTransform = null
        );
}


