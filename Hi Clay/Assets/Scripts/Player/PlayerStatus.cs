using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour, IDamageable
{
    public PlayerData playerData;
    public SoundEventChannelSO playerSFXChannel;

    [Header("Player Stats")]
    [HideInInspector] public string characterID;
    private string characterName;
    private string characterDescription;

    [Header("Visuals")]
    public SpriteRenderer characterHead;
    public SpriteRenderer characterBody;
    public SpriteRenderer characterLegs;
    public SpriteRenderer characterWeapon;

    [Header("Sound Effects")]
    private float idleTalkTimer;
    [SerializeField] private float idleTalkInterval = 8f;
    [HideInInspector] public AudioClip attackClip;
    [HideInInspector] public AudioClip dieClip;
    [HideInInspector] public AudioClip hitClip;
    [HideInInspector] public AudioClip[] idleclip;

    [Header("VFX")]
    public GameObject HitEffect;
    private string hitAnimationName = "Blood";

    [Header("Animation Settings")]
    [HideInInspector] public Animator animator;
    [HideInInspector] public AnimationClip idleAnimation;
    [HideInInspector] public AnimationClip runAnimation;
    [HideInInspector] public AnimationClip jumpAnimation;
    [HideInInspector] public AnimationClip hurtAnimation;

    [Header("Human Stats")]
    [HideInInspector] public float health;
    [HideInInspector] public float speed;
    [HideInInspector] public float jumpForce;
    [HideInInspector] public float dashForce;

    [Header("Weapon Stats")]
    [HideInInspector] public float fireRate;

    [Header("Pool Manager")]
    [HideInInspector] public ObjectPoolManager objectPoolManager;

    [Header("UI")]
    [SerializeField] private GameObject buttonEnterMecha;
    [SerializeField] private GameObject buttonRespawn;

    private bool isHurting;
    private bool isDead = false;
    private Vector3 lastDeathPosition;
    private InputSystem_Actions inputActions;
    private IInteractable currentInteractable;

    // Backup stats
    private float originalHealth;
    private float originalSpeed;
    private float originalJumpForce;
    private float originalDashForce;
    private float originalFireRate;

    private void Awake()
    {
        if (playerData != null)
        {
            // Basic info
            characterID = playerData.characterID;
            characterName = playerData.characterName;
            characterDescription = playerData.characterDescription;

            // Visuals
            characterHead.sprite = playerData.characterHead;
            characterBody.sprite = playerData.characterBody;
            characterLegs.sprite = playerData.characterLegs;
            characterWeapon.sprite = playerData.characterWeapon;

            // Stats
            health = playerData.health;
            speed = playerData.speed;
            jumpForce = playerData.jumpForce;
            dashForce = playerData.dashForce;

            originalHealth = health;
            originalSpeed = speed;
            originalJumpForce = jumpForce;
            originalDashForce = dashForce;

            // Animations
            animator = GetComponent<Animator>();
            idleAnimation = playerData.idleAnimation;
            runAnimation = playerData.runAnimation;
            jumpAnimation = playerData.jumpAnimation;
            hurtAnimation = playerData.hurtAnimation;

            // Sound
            attackClip = playerData.attackClip;
            dieClip = playerData.dieClip;
            hitClip = playerData.hitClip;
            idleclip = playerData.idleClips;

            // Weapon
            fireRate = playerData.bulletData.fireRate;
            originalFireRate = fireRate;

            // Pool
            objectPoolManager = Object.FindAnyObjectByType<ObjectPoolManager>();

            // UI
            buttonEnterMecha.SetActive(false);
            buttonRespawn.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
        }
        inputActions.Player.Interact.performed += ctx => HandleInteract();
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Interact.performed -= ctx => HandleInteract();
        inputActions.Disable();
    }

    private void HandleInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isHurting || isDead) return;

        isHurting = true;
        health -= damage;
        Debug.Log($"{characterName} took {damage} damage. Remaining health: {health}");

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
            playerSFXChannel.RaiseEvent(hitClip);
            animator.Play(hurtAnimation.name);
            Invoke(nameof(ResetHurt), hurtAnimation.length);
        }
    }

    private void ResetHurt()
    {
        isHurting = false;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        lastDeathPosition = transform.position;

        playerSFXChannel.RaiseEvent(dieClip);
        Debug.Log($"{characterName} has died.");

        gameObject.SetActive(false);
        buttonRespawn.SetActive(true);
    }

    public void Respawn()
    {
        transform.position = lastDeathPosition;

        health = originalHealth;
        speed = originalSpeed;
        jumpForce = originalJumpForce;
        dashForce = originalDashForce;
        fireRate = originalFireRate;

        isDead = false;
        isHurting = false;
        idleTalkTimer = idleTalkInterval;

        gameObject.SetActive(true);
        buttonRespawn.SetActive(false);

        animator.Play(idleAnimation.name);
        Debug.Log($"{characterName} has respawned.");
    }

    public void OnRespawnButtonClicked()
    {
        Respawn();
    }

    private void Update()
    {
        // Idle talk
        idleTalkTimer -= Time.deltaTime;
        if (idleTalkTimer <= 0f && idleclip.Length > 0)
        {
            AudioClip clip = idleclip[Random.Range(0, idleclip.Length)];
            playerSFXChannel.RaiseEvent(clip);
            idleTalkTimer = idleTalkInterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteractable>(out var interactable))
        {
            buttonEnterMecha.SetActive(true);
            currentInteractable = interactable;
            Debug.Log("Interactable found: " + interactable.GetInteractPrompt());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteractable>(out var interactable) && interactable == currentInteractable)
        {
            buttonEnterMecha.SetActive(false);
            currentInteractable = null;
            Debug.Log("Left interactable range.");
        }
    }
}
