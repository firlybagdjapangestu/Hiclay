using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // ========== [ References ] ==========
    private EnemyStatus enemyStatus;
    private EnemyData enemyData;
    private BulletData bulletData;
    private float fireRate;

    [SerializeField] private Transform startBulletPosition;
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private Transform target;

    [SerializeField] private Transform visuals;


    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Dead
    }

    public EnemyState currentState;
    private EnemyState lastState;


    private void SetFromData()
    {
        enemyStatus = GetComponent<EnemyStatus>();
        enemyData = enemyStatus.enemyData;
        bulletData = enemyData.bulletData;
        fireRate = bulletData.fireRate;
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentState = EnemyState.Chase;
    }

    private void Start()
    {
        SetFromData();
        InitializeCurrentState();
    }

    private void OnEnable()
    {
        SetFromData();
        InitializeCurrentState();
    }

    private void Update()
    {
        FlipToTarget();
        AimWeapon();

        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrolState();
                break;

            case EnemyState.Chase:
                HandleChaseState();
                break;

            case EnemyState.Attack:
                HandleAttackState();
                break;
        }
    }

    public void SetState(EnemyState newState)
    {
        if (currentState == newState) return;

        lastState = currentState;
        currentState = newState;
        InitializeCurrentState();
    }

    private void InitializeCurrentState()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                enemyData.patrolData?.Initialize(transform);
                break;

            case EnemyState.Chase:
                // Optional: in case chase data needs setup
                break;

            case EnemyState.Attack:
                // Reset fire cooldown when starting attack state
                fireRate = 0f;
                break;
        }
    }

    private void HandlePatrolState()
    {
        enemyData.patrolData?.Patrol();

        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);
            if (distanceToTarget <= enemyData.detectRange)
            {
                SetState(EnemyState.Chase);
            }
        }
    }

    private void HandleChaseState()
    {
        if (target == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget <= enemyData.attackRange)
        {
            SetState(EnemyState.Attack);
        }
        else
        {
            enemyData.chaseData?.Chase(transform, target);
        }
    }

    private void HandleAttackState()
    {
        if (target == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        if (distanceToTarget > enemyData.attackRange)
        {
            SetState(EnemyState.Chase);
            return;
        }

        fireRate -= Time.deltaTime;
        if (fireRate > 0f) return;

        enemyStatus.sfxChannel.RaiseEvent(enemyData.attackClip);
        enemyData.attackData?.Attack(transform, startBulletPosition, target, bulletData, weaponPivot);
        fireRate = bulletData.fireRate;
    }

    private void FlipToTarget()
    {
        if (target == null || visuals == null) return;

        bool isTargetOnLeft = target.position.x < transform.position.x;

        Vector3 localScale = visuals.localScale;
        localScale.x = isTargetOnLeft ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
        visuals.localScale = localScale;
    }

    private void AimWeapon()
    {
        if (target == null || weaponPivot == null) return;

        // Hitung arah dari senjata ke target
        Vector2 direction = (target.position - weaponPivot.position).normalized;

        // Hitung sudut dalam derajat
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotasi pivot senjata
        weaponPivot.rotation = Quaternion.Euler(0f, 0f, angle);

        // Flip localScale Y kalau arah X ke kiri
        Vector3 pivotScale = weaponPivot.localScale;
        pivotScale.y = direction.x < 0 ? -1f : 1f;
        weaponPivot.localScale = pivotScale;
    }



    private void FlipToDirection(Vector2 moveDirection)
    {
        if (moveDirection.x == 0) return;

        Vector3 scale = transform.localScale;
        scale.x = moveDirection.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}
