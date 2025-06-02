using UnityEngine;

public class MechaStatus : MonoBehaviour, IDamageable
{
    [Header("Mecha Data")]
    public MechaData mechaData;

    [Header("Sound Event Channel")]
    public SoundEventChannelSO mechaSFXChannel;

    [Header("Visuals")]
    public SpriteRenderer mechaHead;
    public SpriteRenderer mechaBody;
    public SpriteRenderer mechaRightShoulder;
    public SpriteRenderer mechaRightArm;
    public SpriteRenderer mechaLeftShoulder;
    public SpriteRenderer mechaLeftArm;
    public SpriteRenderer mechaRightThigh;
    public SpriteRenderer mechaRightLegs;
    public SpriteRenderer mechaLeftThigh;
    public SpriteRenderer mechaLeftLegs;

    [Header("Sound Effects")]
    [HideInInspector]public AudioClip attackClip;
    [HideInInspector]public AudioClip dieClip;
    [HideInInspector]public AudioClip hitClip;

    [Header("VFX")]
    public GameObject HitEffect;
    private string hitAnimationName = "MechaHit";

    [Header("Animation Settings")]
    public Animator animator;
    [HideInInspector] public AnimationClip hitAnimation;

    [Header("Stats")]
    [HideInInspector] public float health;
    [HideInInspector] public float speed;
    [HideInInspector] public float dashForce;

    [Header("Weapon Stats")]
    [HideInInspector] public BulletData bulletData;
    [HideInInspector] public float fireRate;

    [Header("Pool Manager")]
    [HideInInspector] public ObjectPoolManager objectPoolManager;

    [HideInInspector] public bool isHurting;

    private void Awake()
    {
        if (mechaData != null)
        {
            // Assign visual sprites
            mechaHead.sprite = mechaData.mechaHead;
            mechaBody.sprite = mechaData.mechaBody;
            mechaRightShoulder.sprite = mechaData.mechaShoulder;
            mechaRightArm.sprite = mechaData.mechaArm;
            mechaLeftShoulder.sprite = mechaData.mechaShoulder; // Assuming both shoulders are the same
            mechaLeftArm.sprite = mechaData.mechaArm; // Assuming both arms are the same
            mechaRightThigh.sprite = mechaData.mechaThigh;
            mechaRightLegs.sprite = mechaData.mechaLegs;
            mechaLeftThigh.sprite = mechaData.mechaThigh; // Assuming both thighs are the same
            mechaLeftLegs.sprite = mechaData.mechaLegs; // Assuming both legs are the same


            // Assign SFX
            attackClip = mechaData.attackClip;
            dieClip = mechaData.dieClip;
            hitClip = mechaData.hitClip;

            // Assign stats
            health = mechaData.health;
            speed = mechaData.speed;
            dashForce = mechaData.dashForce;
            fireRate = mechaData.bulletData.fireRate;

            // Assign hit animation
            hitAnimation = mechaData.hitAnimation;
            bulletData = mechaData.bulletData;

            objectPoolManager = Object.FindAnyObjectByType<ObjectPoolManager>();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isHurting) return;
        isHurting = true;

        health -= damage;
        Debug.Log($"{mechaData.mechaName} took {damage} damage. Remaining health: {health}");

        GameObject vfxGO = objectPoolManager.ActiveObject(HitEffect, transform.position, Quaternion.identity);
        var vfx = vfxGO.GetComponent<VFXController>();
        if (vfx != null)
        {
            vfx.PlayAnimation(hitAnimationName);
        }

        if (health <= 0)
        {
            Die();
        }
        else
        {
            mechaSFXChannel.RaiseEvent(hitClip);
            animator.Play(hitAnimation.name);
            Invoke(nameof(ResetHurt), hitAnimation.length);
        }
    }

    private void ResetHurt()
    {
        isHurting = false;
    }

    public void Die()
    {
        mechaSFXChannel.RaiseEvent(dieClip);
        Debug.Log($"{mechaData.mechaName} has been destroyed.");
        // Logic after mecha destroyed, bisa panggil keluar dari mecha, dll.
    }
}
