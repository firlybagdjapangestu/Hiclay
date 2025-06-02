using UnityEngine;

public class BulletController : MonoBehaviour, IDamageable
{
    public BulletData bulletData;
    public IBulletBehavior behavior;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private float currentHealth;
    [HideInInspector] public float Speed;
    [HideInInspector] public float Damage;
    [HideInInspector] public float Lifetime;
    [HideInInspector] public float KnockbackForce;
    [HideInInspector] public float AreaOfEffect;
    [HideInInspector] public bool IsPiercing;
    private string hitAnimationName = "explosion"; 
    [HideInInspector] public GameObject HitEffect;

    private float lifeTimer;
    private ObjectPoolManager objectPoolManager;

    private bool isActive = false;

    [HideInInspector] public Vector2 MoveDirection = Vector2.right; // Default aman


    void Awake()
    {
        objectPoolManager = Object.FindAnyObjectByType<ObjectPoolManager>();
    }

    void Update()
    {
        if (!isActive) return;

        behavior?.UpdateBehavior(this);

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(Damage);

            if (collision.TryGetComponent(out Rigidbody2D rb))
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                rb.AddForce(knockbackDir * KnockbackForce, ForceMode2D.Impulse);
            }

            if (HitEffect != null)
            {
                GameObject vfxGO = objectPoolManager.ActiveObject(HitEffect, transform.position, Quaternion.identity);

                // 🔥 Aktifkan animasinya (kalau pakai VFXController)
                var vfx = vfxGO.GetComponent<VFXController>();
                if (vfx != null)
                {
                    vfx.PlayAnimation(hitAnimationName); // Ganti nama animasi sesuai kebutuhan kamu
                }
            }


            if (!IsPiercing)
            {
                Deactivate();
            }
        }
    }

    public void SetDirection(Vector2 direction)
    {
        MoveDirection = direction.normalized;
        transform.right = direction.normalized; // untuk rotasi visual
    }


    public void ApplyBulletData(BulletData newData)
    {
        bulletData = newData;
    }

    public void Activate()
    {
        if (bulletData == null)
        {
            Debug.LogWarning("BulletData kosong saat Activate!");
            return;
        }

        spriteRenderer.sprite = bulletData.bulletSprite;
        Speed = bulletData.speed;
        Damage = bulletData.damage;
        Lifetime = bulletData.lifetime;
        KnockbackForce = bulletData.knockbackForce;
        AreaOfEffect = bulletData.areaOfEffect;
        IsPiercing = bulletData.isPiercing;
        HitEffect = bulletData.hitEffect;
        currentHealth = bulletData.health;
        hitAnimationName = bulletData.hitAnimation.name;

        switch (bulletData.bulletType)
        {
            case BulletType.Straight:
                behavior = new StraightBehavior();
                break;
            case BulletType.Homing:
                behavior = new HomingBehavior();
                break;
            default:
                Debug.LogWarning("Bullet type tidak dikenali");
                break;
        }

        Debug.Log($"Activating bullet: {bulletData.bulletName}, Speed: {Speed}, Damage: {Damage}, Lifetime: {Lifetime} hitEffect : {HitEffect?.name}");
        lifeTimer = Lifetime;
        isActive = true;
    }


    public void Deactivate()
    {
        isActive = false;
        objectPoolManager.DeactivateObject(gameObject);
    }

    public void TakeDamage(float damage)
    {
        if (!isActive) return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Deactivate();
        }
    }

}
